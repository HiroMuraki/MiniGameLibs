namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public class BlockActedEventArgs : EventArgs {
        public IBlock Block { get; }
        public BlockAction Action { get; }
        public Coordinate Coordinate { get; }

        public BlockActedEventArgs(IBlock block, BlockAction action, Coordinate coord) {
            Block = block;
            Action = action;
            Coordinate = coord;
        }
    }
}