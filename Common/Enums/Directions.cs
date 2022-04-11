namespace HM.MiniGames {
    [Flags]
    public enum Directions {
#pragma warning disable format
        None  = 0b_0000_0000,
        Up    = 0b_0000_0001,
        Down  = 0b_0000_0010,
        Left  = 0b_0000_0100,
        Right = 0b_0000_1000,
#pragma warning restore format
    }
}