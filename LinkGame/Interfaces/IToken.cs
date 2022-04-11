namespace HM.MiniGames.LinkGame {
    public interface IToken {
        int ContentID { get; set; }
        TokenStatus Status { get; set; }
        Coordinate Coordinate { get; set; }
    }
}
