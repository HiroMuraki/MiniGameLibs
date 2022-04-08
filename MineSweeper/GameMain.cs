using System.Linq;

namespace HM.MiniGames.Minesweeper {
    public sealed class GameMain {
        #region Events
        public event EventHandler<LayoutUpdatedEventArgs>? LayoutUpdated;
        public event EventHandler<BlockActedEventArgs>? BlockActed;
        #endregion

        #region Properties
        public GameLayout Layout { get; private set; } = GameLayout.Create(0, 0, 0);
        public Grid<IBlock> Blocks { get; set; } = Grid<IBlock>.Create(0, 0);
        public bool Started { get; private set; }
        #endregion

        #region Methods
        public GameMain Prepare(int rowSize, int columnSize, int mineCount) {
            if (_blockGenerator is null) {
                throw new InvalidBlockGeneratorException();
            }

            Started = false;
            Layout = GameLayout.Create(rowSize, columnSize, mineCount);
            Blocks = Grid<IBlock>.Create(Layout.RowSize, Layout.ColumnSize);
            foreach (var coord in Layout.Coordinates) {
                Blocks[coord] = _blockGenerator.Create();
                Blocks[coord].Coordinate = coord;
            }
            return this;
        }
        public GameMain Start(GameStartInfo gameInfo) {
            var protectedCoords = new List<Coordinate>();
            protectedCoords.Add(gameInfo.StartCoordinate);
            protectedCoords.AddRange(Layout.GetAroundCoordinates(gameInfo.StartCoordinate));
            Layout.Shuffle(protectedCoords.ToArray());
            UpdateLayout();
            OnLayoutUpdated();
            Started = true;
            return this;
        }
        public GameMain Pause() {
            return this;
        }
        public GameMain RestartGame() {
            return this;
        }
        public GameMain RestoreGame() {
            return this;
        }
        public GameMain Open(Coordinate coord) {
            // �򿪱���
            CheckCoordinate(coord);
            if (Blocks[coord].IsOpen || Blocks[coord].IsFlagged) {
                return this;
            }
            Blocks[coord].IsOpen = true;
            OnBlockActed(coord, BlockAction.Open);
            return this;
        }
        public GameMain OpenRecursively(Coordinate coord) {
            Open(coord);
            // �����ǰ����Χ���ײ�����Χ�ı������С����Χ����������
            if (Blocks[coord].NearbyMines > 0 && CountNearby(coord, b => b.IsFlagged) < Blocks[coord].NearbyMines) {
                return this;
            }
            // ��Χ���ף��ݹ����Χ�ķ���
            foreach (var aCoord in Layout.GetAroundCoordinates(coord)) {
                // �����ѿ����ǵķ���
                if (Blocks[aCoord].IsOpen || Blocks[aCoord].IsFlagged) {
                    continue;
                }
                OpenRecursively(aCoord);
            }
            return this;
        }
        public GameMain Close(Coordinate coord) {
            CheckCoordinate(coord);
            if (!Blocks[coord].IsOpen) {
                return this;
            }
            Blocks[coord].IsOpen = false;
            OnBlockActed(coord, BlockAction.Close);
            return this;
        }
        public GameMain Flag(Coordinate coord) {
            CheckCoordinate(coord);
            if (Blocks[coord].IsFlagged || Blocks[coord].IsOpen) {
                return this;
            }
            Blocks[coord].IsFlagged = true;
            OnBlockActed(coord, BlockAction.Flagged);
            return this;
        }
        public GameMain Unflag(Coordinate coord) {
            CheckCoordinate(coord);
            if (!Blocks[coord].IsFlagged) {
                return this;
            }
            Blocks[coord].IsFlagged = false;
            OnBlockActed(coord, BlockAction.Unflagged);
            return this;
        }
        public bool IsGameCompleted() {
            foreach (var block in Blocks) {
                if (block.Type == BlockType.Blank && !block.IsOpen) {
                    return false;
                }
            }
            return true;
        }
        #endregion

        public static GameMain Create(IBlockGenerator generator) {
            return new GameMain() {
                _blockGenerator = generator
            };
        }

        #region Helper
        private GameMain() {

        }
        private IBlockGenerator _blockGenerator = new NoBlockGenerator();
        private int CountNearby(Coordinate coord, Predicate<IBlock> predicate) {
            int count = 0;
            foreach (var aCoord in Layout.GetAroundCoordinates(coord)) {
                if (predicate(Blocks[aCoord])) {
                    count++;
                }
            }
            return count;
        }
        private void UpdateLayout() {
            foreach (var coord in Layout.Coordinates) {
                Blocks[coord].Type = Layout[coord] switch {
                    0 => BlockType.Blank,
                    1 => BlockType.Mine,
                    _ => BlockType.Unknow,
                };
            }

            foreach (var coord in Layout.Coordinates) {
                Blocks[coord].NearbyMines = CountNearby(coord, c => c.Type == BlockType.Mine);
            }
        }
        private void CheckCoordinate(Coordinate coord) {
            if (coord.X < 0 || coord.X >= Layout.ColumnSize || coord.Y < 0 || coord.Y >= Layout.RowSize) {
                throw new InvalidCoordinateException(coord);
            }
        }
        private void OnLayoutUpdated() {
            LayoutUpdated?.Invoke(this, new LayoutUpdatedEventArgs());
        }
        private void OnBlockActed(Coordinate coord, BlockAction action) {
            BlockActed?.Invoke(this, new BlockActedEventArgs(action, coord));
        }
        #endregion
    }
}