using System.Text;

namespace HM.MiniGames {
    public sealed class LayoutHelper<TCell>
        : IEquatable<LayoutHelper<TCell>>
        where TCell : notnull, IEquatable<TCell> {
        #region Properties
        public TCell this[int x, int y] {
            get {
                return _layout[y, x];
            }
            set {
                _layout[y, x] = value;
            }
        }
        public TCell this[Coordinate coord] {
            get {
                return _layout[coord.Y, coord.X];
            }
            set {
                _layout[coord.Y, coord.X] = value;
            }
        }
        public int RowSize => _layout.GetLength(0);
        public int ColumnSize => _layout.GetLength(1);
        public IEnumerable<Coordinate> Coordinates => GetCoordinates(_layout);
        #endregion

        #region Methods
        public Coordinate[] FindCoordinates(Predicate<TCell> predicate) {
            return FindCoordinates(predicate);
        }
        public bool TryFindCoordinates(Predicate<TCell> predicate, out Coordinate[] coord) {
            return TryFindCoordinates(predicate, out coord);
        }
        public Coordinate[] GetAroundCoordinates(Coordinate center) {
            return GetAroundCoordinates(_layout, center);
        }
        public void Shuffle() {
            Shuffle(_layout);
        }
        public void Shuffle(Coordinate[] fixedCoordinates) {
            Shuffle(_layout, fixedCoordinates);
        }
        public bool IsValidCoordinate(Coordinate coord) {
            return IsValidCoordinate(_layout, coord);
        }
        public override string ToString() {
            var sb = new StringBuilder(RowSize * ColumnSize);

            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    sb.Append(_layout[x, y]);
                    if (x != ColumnSize - 1) {
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        public override int GetHashCode() {
            return RowSize ^ ColumnSize;
        }
        public override bool Equals(object? obj) {
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj is null) {
                return false;
            }
            if (obj.GetType() != typeof(LayoutHelper<TCell>)) {
                return false;
            }
            return Equals((LayoutHelper<TCell>)obj);
        }
        public bool Equals(LayoutHelper<TCell>? other) {
            if (ReferenceEquals(this, other)) {
                return true;
            }
            if (other is null) {
                return false;
            }
            if (RowSize != other.RowSize || ColumnSize != other.ColumnSize) {
                return false;
            }

            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    if (!_layout[y, x].Equals(other[y, x])) {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Static Methods
        public static LayoutHelper<TCell> Create(int rowSize, int columnSize, TCell initValue) {
            return new LayoutHelper<TCell>(rowSize, columnSize, initValue);
        }
        public static LayoutHelper<TCell> Create(int rowSize, int columnSize) {
            return new LayoutHelper<TCell>(rowSize, columnSize, default!);
        }
        public static Coordinate[] GetCoordinates<T>(T[,] layout) {
            int rowSize = layout.GetLength(0);
            int columnSize = layout.GetLength(1);

            var coords = new Coordinate[rowSize * columnSize];
            for (int x = 0; x < columnSize; x++) {
                for (int y = 0; y < rowSize; y++) {
                    coords[x * columnSize + y] = new Coordinate(x, y);
                }
            }

            return coords;
        }
        public static bool TryFindCoordinates<T>(T[,] layout, Predicate<T> predicate, out Coordinate[] result) {
            result = (from i in GetCoordinates(layout)
                      where predicate(layout[i.Y, i.X])
                      select i).ToArray();

            return result.Length != 0;
        }
        public static Coordinate[] FindCoordinates<T>(T[,] layout, Predicate<T> predicate) {
            TryFindCoordinates(layout, predicate, out var result);
            return result;
        }
        public static Coordinate[] GetAroundCoordinates<T>(T[,] layout, Coordinate center) {
            return (from i in _aroundDelta
                    let aCoord = center + i
                    where IsValidCoordinate(layout, aCoord)
                    select aCoord).ToArray();
        }
        public static void Shuffle<T>(T[,] layout) {
            ShuffleHelper(layout, Array.Empty<Coordinate>());
        }
        public static void Shuffle<T>(T[,] layout, Coordinate[] fixedCoordinates) {
            ShuffleHelper(layout, fixedCoordinates);
        }
        public static bool IsValidCoordinate<T>(T[,] layout, Coordinate coord) {
            return !(coord.X < 0 || coord.X >= layout.GetLength(1) || coord.Y < 0 || coord.Y >= layout.GetLength(0));
        }
        #endregion

        #region Helpers
        private readonly TCell[,] _layout;
        private LayoutHelper(int rowSize, int columnSize, TCell initValue) {
            _layout = new TCell[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    _layout[y, x] = initValue;
                }
            }
        }
        private static readonly List<Coordinate> _aroundDelta = new() {
#pragma warning disable format
                (-1, -1), (0, -1), (1, -1),
                (-1,  0),          (1,  0),
                (-1,  1), (0,  1), (1,  1)
#pragma warning restore format
        };
        private static void ShuffleHelper<T>(T[,] layout, Coordinate[] fixedCoordinates) {
            var rnd = new Random();

            Coordinate[] allowedCoords;
            // 选择可进行随机化的坐标组
            if (fixedCoordinates.Length == 0) {
                allowedCoords = GetCoordinates(layout);
            }
            else {
                allowedCoords = (from i in GetCoordinates(layout)
                                 where !fixedCoordinates.Contains(i)
                                 select i).ToArray();
            }

            // 打乱除了保护坐标外的其他坐标序列，算法为洗牌算法
            for (int i = 0; i < allowedCoords.Length; i++) {
                var coord1 = allowedCoords[i];
                var coord2 = allowedCoords[rnd.Next(i, allowedCoords.Length)];
                var t = layout[coord1.Y, coord1.X];
                layout[coord1.Y, coord1.X] = layout[coord2.Y, coord2.X];
                layout[coord2.Y, coord2.X] = t;
            }
        }
        #endregion
    }
}