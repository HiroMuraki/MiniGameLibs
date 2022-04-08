using System.Runtime.Serialization;

namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public class InvalidCoordinateException : Exception {
        public Coordinate Coordinate { get; }

        public InvalidCoordinateException(Coordinate coordinate) : this($"{coordinate} is out of layout") {
            Coordinate = coordinate;
        }
        public InvalidCoordinateException(string message) : base(message) { }
        public InvalidCoordinateException(string message, Exception inner) : base(message, inner) { }

        protected InvalidCoordinateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}