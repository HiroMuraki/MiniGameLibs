namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public class BlockActedEventArgs : EventArgs {
        public BlockAction Action { get; }
        public Coordinate Coordinate { get; }

        public BlockActedEventArgs(BlockAction action, Coordinate coord) {
            Action = action;
            Coordinate = coord;
        }
    }
}