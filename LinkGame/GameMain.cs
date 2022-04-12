namespace HM.MiniGames.LinkGame {
    public class GameMain {
        class NoToken : IToken {
            public int ContentID { get; set; } = 0;
            public TokenStatus Status { get; set; } = TokenStatus.None;
            public Coordinate Coordinate { get; set; }
        }

        #region Events
        public event EventHandler<GameStatusChangedEventArgs>? GameStatusChanged;
        public event EventHandler<TokenMatchedEventArgs>? TokenMatched;
        #endregion

        #region Properties
        public int RowSize => _contentIDLayout.RowSize;
        public int ColumnSize => _contentIDLayout.ColumnSize;
        public Grid<IToken> Grid { get; private set; } = Grid<IToken>.Empty;
        #endregion

        #region Methods    
        public void Prepare(int rowSize, int columnSize, int[] contentIDs) {
            int totalTokens = rowSize * columnSize;
            if (totalTokens % 2 != 0) {
                throw new ArgumentException($"Invalid layout size({rowSize} x {columnSize})");
            }

            var rnd = new Random();

            _contentIDLayout = Layout<int>.Create(rowSize, columnSize);
            while (_contentIDLayout.CountIf(c => c == 0) != 0) {
                int contentID = contentIDs[rnd.Next(0, contentIDs.Length)];
                var fixedCoords = _contentIDLayout.FindCoordinates(c => c != 0);
                _contentIDLayout.RandomFill(contentID, 2, fixedCoords);
            }

            Grid = Grid<IToken>.Create(RowSize, ColumnSize);
            foreach (var coord in _contentIDLayout.Coordinates) {
                Grid[coord] = _tokenGenerator.Create();
                Grid[coord].Status = TokenStatus.Idle;
                Grid[coord].Coordinate = coord;
                Grid[coord].ContentID = _contentIDLayout[coord];
            }
            _gameHelper = new LinkGameHelper(Grid);

            OnGameStatusChanged(GameStatus.Prepared);
        }
        public void Start() {
            OnGameStatusChanged(GameStatus.Started);
        }
        public void Pause() {
            OnGameStatusChanged(GameStatus.Paused);
        }
        public void SelectToken(Coordinate coord) {
            _gameHelper.CheckCoordinate(coord);

            if (Grid[coord].Status == TokenStatus.Idle) {
                if (ReferenceEquals(_heldToken, _noToken)) {
                    _heldToken = Grid[coord];
                    _heldToken.Status = TokenStatus.Selected;
                }
                else {
                    if (_gameHelper.TryConnect(_heldToken.Coordinate, coord, out var nodes)) {
                        if (_gameHelper.TryMatch(_heldToken.Coordinate, coord)) {
                            _heldToken.Status = TokenStatus.Matched;
                            Grid[coord].Status = TokenStatus.Matched;
                            OnTokenMatched(_heldToken, Grid[coord], nodes);
                        }
                        else {
                            _heldToken.Status = TokenStatus.Idle;
                            Grid[coord].Status = TokenStatus.Idle;
                        }
                    }
                    _heldToken = _noToken;
                }
            }
            else if (_heldToken.Coordinate == coord) {
                _heldToken = _noToken;
                _heldToken.Status = TokenStatus.Idle;
            }
        }
        public bool IsGameCompleted() {
            return _gameHelper.IsGameCompleted();
        }
        #endregion

        public static GameMain Create(ITokenGenerator tokenGenerator) {
            return new GameMain(tokenGenerator);
        }

        private GameMain(ITokenGenerator tokenGenerator) {
            _tokenGenerator = tokenGenerator;
        }
        private static readonly IToken _noToken = new NoToken();
        private readonly ITokenGenerator _tokenGenerator = new NoTokenGenerator();
        private Layout<int> _contentIDLayout = Layout<int>.Empty;
        private LinkGameHelper _gameHelper = new(Grid<IToken>.Empty);
        private IToken _heldToken = _noToken;
        private void OnTokenMatched(IToken a, IToken b, Coordinate[] nodes) {
            TokenMatched?.Invoke(this, new TokenMatchedEventArgs(a, b, nodes));
        }
        private void OnGameStatusChanged(GameStatus status) {
            GameStatusChanged?.Invoke(this, new GameStatusChangedEventArgs(status));
        }
    }
}
