namespace HM.MiniGames.Minesweeper {
    public sealed class Block : IBlock {
        [Flags]
        enum BlockStatus {
#pragma warning disable format
            Default = 0b_0000_0000,
            Open =    0b_0000_0001,
            Flagged = 0b_0000_0010
#pragma warning restore format
        }

        public int ContentID { get; set; }
        public Coordinate Coordinate { get; set; }
        public BlockType Type { get; set; } = BlockType.Unknow;
        public int NearbyMines { get; set; }
        public bool IsOpen {
            get {
                return HasStatus(BlockStatus.Open);
            }
            set {
                SetStatus(BlockStatus.Open, value);
            }
        }
        public bool IsFlagged {
            get {
                return HasStatus(BlockStatus.Flagged);
            }
            set {
                SetStatus(BlockStatus.Flagged, value);
            }
        }

        public void ResetStatus() {
            Type = BlockType.Unknow;
            NearbyMines = 0;
            _status = BlockStatus.Default;
        }

        private BlockStatus _status = BlockStatus.Default;
        private void SetStatus(BlockStatus status, bool enabled) {
            if (enabled) {
                _status |= status;
            }
            else {
                _status &= ~status;
            }
        }
        private bool HasStatus(BlockStatus status) {
            return (_status & status) == status;
        }
    }
}