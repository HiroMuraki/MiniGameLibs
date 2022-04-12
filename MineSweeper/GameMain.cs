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
        public Grid<IBlock> Grid { get; set; } = Grid<IBlock>.Empty;
        #endregion

        #region Methods
        public GameMain Prepare(int rowSize, int columnSize, int mineCount) {
            if (_blockGenerator is null) {
                throw new InvalidBlockGeneratorException();
            }

            MineCount = mineCount;
            _layoutHelper = Layout<BlockType>.Create(rowSize, columnSize, BlockType.Blank);
            Grid = Grid<IBlock>.Create(rowSize, columnSize);
            foreach (var coord in _layoutHelper.Coordinates) {
                Grid[coord] = _blockGenerator.Create();
                Grid[coord].Coordinate = coord;
            }
            _gameHelper = new MinesweeperHelper(Grid);
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
            if (Grid[coord].IsOpen || Grid[coord].IsFlagged) {
                return this;
            }
            Grid[coord].IsOpen = true;
            OnBlockActed(coord, BlockAction.Open);
            return this;
        }
        public GameMain OpenRecursively(Coordinate coord, out Coordinate[] nodes) {
            _gameHelper.OpenRecursively(coord, (c) => Open(c), out nodes);
            return this;
        }
        public GameMain Close(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (!Grid[coord].IsOpen) {
                return this;
            }
            Grid[coord].IsOpen = false;
            OnBlockActed(coord, BlockAction.Close);
            return this;
        }
        public GameMain Flag(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (Grid[coord].IsFlagged || Grid[coord].IsOpen) {
                return this;
            }
            Grid[coord].IsFlagged = true;
            OnBlockActed(coord, BlockAction.Flagged);
            return this;
        }
        public GameMain Unflag(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);
            if (!Grid[coord].IsFlagged) {
                return this;
            }
            Grid[coord].IsFlagged = false;
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
        private Layout<BlockType> _layoutHelper = Layout<BlockType>.Empty;
        private MinesweeperHelper _gameHelper = new(Grid<IBlock>.Empty);
        private readonly IBlockGenerator _blockGenerator = new NoBlockGenerator();
        private void UpdateLayout() {
            foreach (var coord in _layoutHelper.Coordinates) {
                Grid[coord].Type = _layoutHelper[coord];
            }

            foreach (var coord in _layoutHelper.Coordinates) {
                Grid[coord].NearbyMines = _gameHelper.CountNearby(coord, c => c.Type == BlockType.Mine);
            }
        }
        private void OnLayoutUpdated() {
            LayoutUpdated?.Invoke(this, new LayoutUpdatedEventArgs());
        }
        private void OnBlockActed(Coordinate coord, BlockAction action) {
            BlockActed?.Invoke(this, new BlockActedEventArgs(Grid[coord], action, coord));
        }
        private void OnGameStatusChanged(GameStatus stage) {
            GameStageChanged?.Invoke(this, new GameStatusChangedEventArgs(stage));
        }
        #endregion
    }
}