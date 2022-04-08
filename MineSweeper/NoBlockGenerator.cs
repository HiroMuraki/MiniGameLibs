namespace HM.MiniGames.Minesweeper {
    public class NoBlockGenerator : IBlockGenerator {
        public IBlock Create() {
            throw new InvalidBlockGeneratorException();
        }
    }
}