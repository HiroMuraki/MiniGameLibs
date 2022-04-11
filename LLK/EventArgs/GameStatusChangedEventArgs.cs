namespace HM.MiniGames.LLK {
    public class GameStatusChangedEventArgs : EventArgs {
        public GameStatus Status { get; }

        public GameStatusChangedEventArgs(GameStatus status) {
            Status = status;
        }
    }
}
