namespace HM.MiniGames.LLK {
    public class NoTokenGenerator : ITokenGenerator {
        public IToken Create() {
            throw new InvalidOperationException();
        }
    }
}
