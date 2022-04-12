namespace HM.MiniGames.LinkGame {
    internal class NoTokenGenerator : ITokenGenerator {
        public IToken Create() {
            throw new InvalidOperationException();
        }
    }
}
