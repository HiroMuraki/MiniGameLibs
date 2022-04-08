namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public record GameStartInfo {
        public int RowSize { get; set; }
        public int ColumnSize { get; set; }
        public int MineCount {
            get {
                return _mineCount;
            }
            set {
                int maxMineCount = (RowSize - 1) * (ColumnSize - 1);
                if (_mineCount > maxMineCount) {
                    throw new ArgumentException($"Invalid mine count, the max mine count should be smaller than {maxMineCount}");
                }
                _mineCount = value;
            }
        }
        public Coordinate StartCoordinate { get; set; }

        private int _mineCount;
    }
}