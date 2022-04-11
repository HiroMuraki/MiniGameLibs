namespace HM.MiniGames.LinkGame {
    public class NoTokenGenerator : ITokenGenerator {
        public IToken Create() {
            throw new InvalidOperationException();
        }
    }
}
