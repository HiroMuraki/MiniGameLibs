using System.Runtime.Serialization;

namespace HM.MiniGames {
    [Serializable]
    public class CoordinateOutOfRangeException : Exception {
        public Coordinate Coordinate { get; }

        public CoordinateOutOfRangeException(Coordinate coordinate) : this($"{coordinate} is out of layout") {
            Coordinate = coordinate;
        }
        public CoordinateOutOfRangeException(string message) : base(message) { }
        public CoordinateOutOfRangeException(string message, Exception inner) : base(message, inner) { }

        protected CoordinateOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}