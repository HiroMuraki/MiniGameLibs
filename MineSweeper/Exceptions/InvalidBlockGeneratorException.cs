using System.Runtime.Serialization;

namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public class InvalidBlockGeneratorException : Exception {
        public InvalidBlockGeneratorException() : this("Block generator is null or unable to use") { }
        public InvalidBlockGeneratorException(string message) : base(message) { }
        public InvalidBlockGeneratorException(string message, Exception inner) : base(message, inner) { }

        protected InvalidBlockGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}