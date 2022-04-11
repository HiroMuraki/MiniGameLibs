namespace HM.MiniGames.Minesweeper {
    public class GameStatusChangedEventArgs : EventArgs {
        public GameStatus Stage { get; private set; }

        public GameStatusChangedEventArgs(GameStatus stage) {
            Stage = stage;
        }
    }
}