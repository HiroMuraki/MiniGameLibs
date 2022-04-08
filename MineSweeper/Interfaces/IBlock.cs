namespace HM.MiniGames.Minesweeper {
    public interface IBlock {
        Coordinate Coordinate { get; set; }
        BlockType Type { get; set; }
        bool IsOpen { get; set; }
        bool IsFlagged { get; set; }
        int NearbyMines { get; set; }
    }
}