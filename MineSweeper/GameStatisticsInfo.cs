namespace HM.MiniGames.Minesweeper {
    /// <summary>
    /// 统计信息控制器
    /// </summary>
    public record GameStatisticsInfo {
        public int TimeSeconds { get; private set; }
        public int Flagged { get; set; }
        public int TotalMines { get; set; }
        public int RestMines => TotalMines - Flagged;

        public void StartTimer() {

        }
        public void StopTimer() {

        }
        public void ResetTimer() {

        }
    }
}