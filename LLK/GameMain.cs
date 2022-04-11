namespace HM.MiniGames.LLK {
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
        public Grid<IToken> Tokens { get; private set; } = Grid<IToken>.Create(0, 0);
        #endregion

        #region Methods    
        public void Prepare(int rowSize, int columnSize, int[] contentIDs) {
            int totalTokens = rowSize * columnSize;
            if (totalTokens % 2 != 0) {
                throw new ArgumentException($"Invalid layout size({rowSize} x {columnSize})");
            }

            var rnd = new Random();

            _llkHelper = LLKHelper.Create(rowSize, columnSize);
            _contentIDLayout = Layout<int>.Create(rowSize, columnSize);
            while (_contentIDLayout.CountIf(c => c == 0) != 0) {
                int contentID = contentIDs[rnd.Next(0, contentIDs.Length)];
                var fixedCoords = _contentIDLayout.FindCoordinates(c => c != 0);
                _contentIDLayout.RandomFill(contentID, 2, fixedCoords);
            }

            UpdateLayout();
            OnGameStatusChanged(GameStatus.Prepared);
        }
        public void Start() {
            OnGameStatusChanged(GameStatus.Started);
        }
        public void Pause() {
            OnGameStatusChanged(GameStatus.Paused);
        }
        public void SelectToken(Coordinate coord) {
            CheckCoordinate(coord);

            if (Tokens[coord].Status != TokenStatus.Idle) {
                return;
            }
            else if (ReferenceEquals(_heldToken, _noToken)) {
                _heldToken = Tokens[coord];
                _heldToken.Status = TokenStatus.Selected;
            }
            else if (_heldToken.Coordinate == coord) {
                return;
            }
            else {
                if (TryConnect(_heldToken.Coordinate, coord, out var nodes)) {
                    if (TryMatch(_heldToken.Coordinate, coord)) {
                        _heldToken.Status = TokenStatus.Matched;
                        Tokens[coord].Status = TokenStatus.Matched;
                        OnTokenMatched(_heldToken, Tokens[coord], nodes);
                    }
                    else {
                        _heldToken.Status = TokenStatus.Idle;
                        Tokens[coord].Status = TokenStatus.Idle;
                    }
                }
                _heldToken.Status = TokenStatus.Idle;
                _heldToken = _noToken;
            }
        }
        public bool IsGameCompleted() {
            foreach (var token in Tokens) {
                if (token.Status == TokenStatus.Idle) {
                    return false;
                }
            }
            return true;
        }
        #endregion

        public static GameMain Create(ITokenGenerator tokenGenerator) {
            return new GameMain(tokenGenerator);
        }

        private GameMain(ITokenGenerator tokenGenerator) {
            _tokenGenerator = tokenGenerator;
        }
        private static readonly IToken _noToken = new NoToken();
        private Layout<int> _contentIDLayout = Layout<int>.Create(0, 0);
        private LLKHelper _llkHelper = LLKHelper.Create(0, 0);
        private ITokenGenerator _tokenGenerator = new NoTokenGenerator();
        private IToken _heldToken = _noToken;
        private void UpdateLayout() {
            Tokens = Grid<IToken>.Create(RowSize, ColumnSize);
            foreach (var coord in _contentIDLayout.Coordinates) {
                Tokens[coord] = _tokenGenerator.Create();
                Tokens[coord].Status = TokenStatus.Idle;
                Tokens[coord].Coordinate = coord;
                Tokens[coord].ContentID = _contentIDLayout[coord];
            }
        }
        private bool TryConnect(Coordinate start, Coordinate target, out Coordinate[] nodes) {
            foreach (var coord in _llkHelper.Layout.Coordinates) {
                _llkHelper.Layout[coord] = Tokens[coord].Status switch {
                    TokenStatus.Idle => NodeType.Block,
                    TokenStatus.None => NodeType.Road,
                    TokenStatus.Matched => NodeType.Road,
                    _ => NodeType.Road
                };
            }
            return _llkHelper.TryConnect(start, target, out nodes);
        }
        private bool TryMatch(Coordinate start, Coordinate target) {
            return Tokens[start].ContentID == Tokens[target].ContentID;
        }
        private void CheckCoordinate(Coordinate coord) {
            if (!_contentIDLayout.IsValidCoordinate(coord)) {
                throw new ArgumentException();
            };
        }
        private void OnTokenMatched(IToken a, IToken b, Coordinate[] nodes) {
            TokenMatched?.Invoke(this, new TokenMatchedEventArgs(a, b, nodes));
        }
        private void OnGameStatusChanged(GameStatus status) {
            GameStatusChanged?.Invoke(this, new GameStatusChangedEventArgs(status));
        }
    }
}
