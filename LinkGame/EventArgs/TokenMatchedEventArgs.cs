namespace HM.MiniGames.LinkGame {
    public class TokenMatchedEventArgs : EventArgs {
        public IToken A { get; }
        public IToken B { get; }
        public Coordinate[] Nodes { get; }

        public TokenMatchedEventArgs(IToken a, IToken b, Coordinate[] nodes) {
            A = a;
            B = b;
            Nodes = nodes;
        }
    }
}
