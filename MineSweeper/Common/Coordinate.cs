namespace HM.MiniGames.Minesweeper {
    [Serializable]
    public readonly struct Coordinate : IEquatable<Coordinate> {
        /// <summary>
        /// 获取该坐标的正上方坐标
        /// </summary>
        public Coordinate Up {
            get {
                return new Coordinate(X, Y + 1);
            }
        }
        /// <summary>
        /// 获取该坐标的正下方坐标
        /// </summary>
        public Coordinate Down {
            get {
                return new Coordinate(X, Y - 1);
            }
        }
        /// <summary>
        /// 获取该坐标的正左方坐标
        /// </summary>
        public Coordinate Left {
            get {
                return new Coordinate(X - 1, Y);
            }
        }
        /// <summary>
        /// 获取该坐标的正上方坐标
        /// </summary>
        public Coordinate Right {
            get {
                return new Coordinate(X + 1, Y);
            }
        }

        public int X { get; }
        public int Y { get; }

        public Coordinate(int x, int y) {
            X = x;
            Y = y;
        }

        public static bool operator ==(Coordinate a, Coordinate b) {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Coordinate a, Coordinate b) {
            return !(a == b);
        }
        public static Coordinate operator +(Coordinate a, Coordinate b) {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }
        public static Coordinate operator -(Coordinate a, Coordinate b) {
            return new Coordinate(a.X - b.X, a.Y - b.Y);
        }
        public static Coordinate operator *(Coordinate a, int b) {
            return new Coordinate(a.X * b, a.Y * b);
        }
        public static Coordinate operator /(Coordinate a, int b) {
            return new Coordinate(a.X / b, a.Y / b);
        }
        public static Coordinate operator -(Coordinate coordinate) {
            return new Coordinate(-coordinate.X, -coordinate.Y);
        }
        public static float SqrDistance(Coordinate a, Coordinate b) {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }
        public override bool Equals(object? obj) {
            if (obj is null) {
                return false;
            }
            if (obj.GetType() != typeof(Coordinate)) {
                return false;
            }
            return Equals((Coordinate)obj);
        }
        public override int GetHashCode() {
            return X ^ Y;
        }
        public bool Equals(Coordinate other) {
            return this == other;
        }
        public override string ToString() {
            return $"({X}, {Y})";
        }
    }
}