namespace HM.MiniGames.Minesweeper {
    public class GameStageChangedEventArgs : EventArgs {
        public GameStatus Stage { get; private set; }

        public GameStageChangedEventArgs(GameStatus stage) {
            Stage = stage;
        }
    }
}