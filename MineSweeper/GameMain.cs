namespace HM.MiniGames.Minesweeper {
    public sealed class GameMain {
        #region Events
        public event EventHandler<LayoutUpdatedEventArgs>? LayoutUpdated;
        public event EventHandler<BlockActedEventArgs>? BlockActed;
        public event EventHandler<GameStageChangedEventArgs>? GameStageChanged;
        #endregion

        #region Properties
        public int RowSize => _layoutHelper.RowSize;
        public int ColumnSize => _layoutHelper.ColumnSize;
        public int MineCount { get; private set; }
        public Grid<IBlock> Blocks { get; set; } = Grid<IBlock>.Create(0, 0);
        #endregion

        #region Methods
        public GameMain Prepare(int rowSize, int columnSize, int mineCount) {
            if (_blockGenerator is null) {
                throw new InvalidBlockGeneratorException();
            }

            MineCount = mineCount;
            _layoutHelper = Layout<BlockType>.Create(rowSize, columnSize, BlockType.Blank);
            Blocks = Grid<IBlock>.Create(_layoutHelper.RowSize, _layoutHelper.ColumnSize);
            foreach (var coord in _layoutHelper.Coordinates) {
                Blocks[coord] = _blockGenerator.Create();
                Blocks[coord].Coordinate = coord;
            }
            OnGameStageChanged(GameStage.Prepared);
            return this;
        }
        public GameMain Start(GameStartInfo gameInfo) {
            var fixedCoords = new List<Coordinate>();
            fixedCoords.Add(gameInfo.StartCoordinate);
            fixedCoords.AddRange(_layoutHelper.GetAroundCoordinates(gameInfo.StartCoordinate));
            _layoutHelper.RandomFill(BlockType.Mine, MineCount, fixedCoords.ToArray());
            UpdateLayout();
            OnLayoutUpdated();
            OnGameStageChanged(GameStage.Started);
            return this;
        }
        public GameMain Pause() {
            OnGameStageChanged(GameStage.Paused);
            return this;
        }
        public GameMain RestartGame() {
            return this;
        }
        public GameMain RestoreGame() {
            return this;
        }
        public GameMain Open(Coordinate coord) {
            // 打开本块
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
            // 如果当前块周围有雷并且周围的标记数量小于周围雷数，跳过
            if (Blocks[coord].NearbyMines > 0 && CountNearby(coord, b => b.IsFlagged) < Blocks[coord].NearbyMines) {
                return this;
            }
            // 周围无雷，递归打开周围的方格
            foreach (var aCoord in _layoutHelper.GetAroundCoordinates(coord)) {
                // 跳过已开或标记的方格
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
        private Layout<BlockType> _layoutHelper = Layout<BlockType>.Create(0, 0, BlockType.None);
        private IBlockGenerator _blockGenerator = new NoBlockGenerator();
        private int CountNearby(Coordinate coord, Predicate<IBlock> predicate) {
            int count = 0;
            foreach (var aCoord in _layoutHelper.GetAroundCoordinates(coord)) {
                if (predicate(Blocks[aCoord])) {
                    count++;
                }
            }
            return count;
        }
        private void UpdateLayout() {
            foreach (var coord in _layoutHelper.Coordinates) {
                Blocks[coord].Type = _layoutHelper[coord];
            }

            foreach (var coord in _layoutHelper.Coordinates) {
                Blocks[coord].NearbyMines = CountNearby(coord, c => c.Type == BlockType.Mine);
            }
        }
        private void CheckCoordinate(Coordinate coord) {
            if (coord.X < 0 || coord.X >= _layoutHelper.ColumnSize || coord.Y < 0 || coord.Y >= _layoutHelper.RowSize) {
                throw new CoordinateOutOfRangeException(coord);
            }
        }
        private void OnLayoutUpdated() {
            LayoutUpdated?.Invoke(this, new LayoutUpdatedEventArgs());
        }
        private void OnBlockActed(Coordinate coord, BlockAction action) {
            BlockActed?.Invoke(this, new BlockActedEventArgs(Blocks[coord], action, coord));
        }
        private void OnGameStageChanged(GameStage stage) {
            GameStageChanged?.Invoke(this, new GameStageChangedEventArgs(stage));
        }
        #endregion
    }
}