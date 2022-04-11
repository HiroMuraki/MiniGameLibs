namespace HM.MiniGames.Minesweeper {

    public sealed class GameMain {
        #region Events
        public event EventHandler<LayoutUpdatedEventArgs>? LayoutUpdated;
        public event EventHandler<BlockActedEventArgs>? BlockActed;
        public event EventHandler<GameStatusChangedEventArgs>? GameStageChanged;
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
            _gameHelper = new MinesweeperHelper(Blocks);
            OnGameStatusChanged(GameStatus.Prepared);
            return this;
        }
        public GameMain Start(GameStartInfo gameInfo) {
            var fixedCoords = new List<Coordinate>();
            fixedCoords.Add(gameInfo.StartCoordinate);
            fixedCoords.AddRange(_layoutHelper.GetAroundCoordinates(gameInfo.StartCoordinate));
            _layoutHelper.RandomFill(BlockType.Mine, MineCount, fixedCoords.ToArray());
            UpdateLayout();
            OnLayoutUpdated();
            OnGameStatusChanged(GameStatus.Started);
            return this;
        }
        public GameMain Pause() {
            OnGameStatusChanged(GameStatus.Paused);
            return this;
        }
        public GameMain RestartGame() {
            return this;
        }
        public GameMain RestoreGame() {
            return this;
        }
        public GameMain Open(Coordinate coord) {
            // ´ò¿ª±¾¿é
            _gameHelper.CheckCoordinate(coord);
            if (Blocks[coord].IsOpen || Blocks[coord].IsFlagged) {
                return this;
            }
            Blocks[coord].IsOpen = true;
            OnBlockActed(coord, BlockAction.Open);
            return this;
        }
        public GameMain OpenRecursively(Coordinate coord) {
            _gameHelper.OpenRecursively(coord, (c) => Open(c), out _);
            return this;
        }
        public GameMain Close(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (!Blocks[coord].IsOpen) {
                return this;
            }
            Blocks[coord].IsOpen = false;
            OnBlockActed(coord, BlockAction.Close);
            return this;
        }
        public GameMain Flag(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (Blocks[coord].IsFlagged || Blocks[coord].IsOpen) {
                return this;
            }
            Blocks[coord].IsFlagged = true;
            OnBlockActed(coord, BlockAction.Flagged);
            return this;
        }
        public GameMain Unflag(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (!Blocks[coord].IsFlagged) {
                return this;
            }
            Blocks[coord].IsFlagged = false;
            OnBlockActed(coord, BlockAction.Unflagged);
            return this;
        }
        public bool IsGameCompleted() {
            return _gameHelper.IsGameCompleted();
        }
        #endregion

        public static GameMain Create(IBlockGenerator generator) {
            return new GameMain(generator);
        }

        #region Helper
        private GameMain(IBlockGenerator generator) {
            _blockGenerator = generator;
        }
        private Layout<BlockType> _layoutHelper = Layout<BlockType>.Create(0, 0, BlockType.None);
        private MinesweeperHelper _gameHelper = new(Grid<IBlock>.Create(0, 0));
        private readonly IBlockGenerator _blockGenerator = new NoBlockGenerator();
        private void UpdateLayout() {
            foreach (var coord in _layoutHelper.Coordinates) {
                Blocks[coord].Type = _layoutHelper[coord];
            }

            foreach (var coord in _layoutHelper.Coordinates) {
                Blocks[coord].NearbyMines = _gameHelper.CountNearby(coord, c => c.Type == BlockType.Mine);
            }
        }
        private void OnLayoutUpdated() {
            LayoutUpdated?.Invoke(this, new LayoutUpdatedEventArgs());
        }
        private void OnBlockActed(Coordinate coord, BlockAction action) {
            BlockActed?.Invoke(this, new BlockActedEventArgs(Blocks[coord], action, coord));
        }
        private void OnGameStatusChanged(GameStatus stage) {
            GameStageChanged?.Invoke(this, new GameStatusChangedEventArgs(stage));
        }
        #endregion
    }
}