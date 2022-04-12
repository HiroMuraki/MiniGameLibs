namespace HM.MiniGames.Minesweeper {
    internal class NoBlockGenerator : IBlockGenerator {
        public IBlock Create() {
            throw new InvalidBlockGeneratorException();
        }
    }
}