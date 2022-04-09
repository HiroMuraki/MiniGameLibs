namespace HM.MiniGames.Minesweeper {
    public class GameStageChangedEventArgs : EventArgs {
        public GameStage Stage { get; private set; }

        public GameStageChangedEventArgs(GameStage stage) {
            Stage = stage;
        }
    }
}