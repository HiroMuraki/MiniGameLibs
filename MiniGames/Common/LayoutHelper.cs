using System.Text;

namespace HM.MiniGames {
    public sealed class LayoutHelper<TCell>
        : IEquatable<LayoutHelper<TCell>>
        where TCell : notnull {
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
        public LayoutHelper<TCell> GetDeepCopy() {
            return new LayoutHelper<TCell>(GetDeepCopy(_layout));
        }
        public int CountIf(Predicate<TCell> predicate) {
            return CountIf(_layout, predicate);
        }
        public Coordinate[] FindCoordinates(Predicate<TCell> predicate) {
            return FindCoordinates(_layout, predicate);
        }
        public bool TryFindCoordinates(Predicate<TCell> predicate, out Coordinate[] coord) {
            return TryFindCoordinates(_layout, predicate, out coord);
        }
        public Coordinate[] GetAroundCoordinates(Coordinate center) {
            return GetAroundCoordinates(_layout, center);
        }
        public void RandomAssign(TCell[] values) {
            RandomAssign(_layout, values);
        }
        public void RandomAssign(TCell[] values, Coordinate[] fixedCoords) {
            RandomAssign(_layout, values, fixedCoords);
        }
        public void RandomAssign(TCell value, int count) {
            RandomAssign(_layout, value, count);
        }
        public void RandomAssign(TCell value, int count, Coordinate[] fixedCoords) {
            RandomAssign(_layout, value, count, fixedCoords);
        }
        public void Shuffle() {
            Shuffle(_layout);
        }
        public void Shuffle(Coordinate[] fixedCoords) {
            Shuffle(_layout, fixedCoords);
        }
        public bool IsValidCoordinate(Coordinate coord) {
            return IsValidCoordinate(_layout, coord);
        }
        public override string ToString() {
            var sb = new StringBuilder(RowSize * ColumnSize);

            for (int y = RowSize - 1; y >= 0; y--) {
                for (int x = 0; x < ColumnSize; x++) {
                    sb.Append(_layout[y, x]);
                    if (x < ColumnSize - 1) {
                        sb.Append(' ');
                    }
                }
                if (y > 0) {
                    sb.AppendLine();
                }
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

            return Equals(_layout, other._layout);
        }
        #endregion

        #region Static Methods
        public static LayoutHelper<TCell> Create(int rowSize, int columnSize, TCell initValue) {
            return new LayoutHelper<TCell>(rowSize, columnSize, initValue);
        }
        public static LayoutHelper<TCell> Create(int rowSize, int columnSize) {
            return new LayoutHelper<TCell>(rowSize, columnSize, default!);
        }
        public static LayoutHelper<TCell> Create(TCell[,] layout) {
            return new LayoutHelper<TCell>(layout);
        }
        public static T[,] GetDeepCopy<T>(T[,] layout) {
            int rowSize = layout.GetLength(0);
            int columnSize = layout.GetLength(1);

            var copy = new T[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    copy[y, x] = layout[y, x];
                }
            }
            return copy;
        }
        public static int CountIf<T>(T[,] layout, Predicate<T> predicate) where T : notnull {
            int count = 0;
            foreach (var coord in GetCoordinates(layout)) {
                if (predicate(layout[coord.Y, coord.X])) {
                    count++;
                }
            }
            return count;
        }
        public static Coordinate[] GetCoordinates<T>(T[,] layout) where T : notnull {
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
        public static bool TryFindCoordinates<T>(T[,] layout, Predicate<T> predicate, out Coordinate[] result) where T : notnull {
            result = (from i in GetCoordinates(layout)
                      where predicate(layout[i.Y, i.X])
                      select i).ToArray();

            return result.Length != 0;
        }
        public static Coordinate[] FindCoordinates<T>(T[,] layout, Predicate<T> predicate) where T : notnull {
            TryFindCoordinates(layout, predicate, out var result);
            return result;
        }
        public static Coordinate[] GetAroundCoordinates<T>(T[,] layout, Coordinate center) where T : notnull {
            return (from i in _aroundDelta
                    let aCoord = center + i
                    where IsValidCoordinate(layout, aCoord)
                    select aCoord).ToArray();
        }
        public static void RandomAssign<T>(T[,] layout, T[] values) where T : notnull {
            RandomAssign(layout, values, Array.Empty<Coordinate>());
        }
        public static void RandomAssign<T>(T[,] layout, T[] values, Coordinate[] fixedCoords) where T : notnull {
            if (layout.Length - fixedCoords.Length < values.Length) {
                throw new ArgumentException($"Values size({values.Length}) could not be larger than target coords size({fixedCoords.Length})");
            }

            var rnd = new Random();
            var coordList = (from coord in GetCoordinates(layout)
                             where !fixedCoords.Contains(coord)
                             select coord).ToList();
            var valList = values.ToList();
            while (valList.Count > 0 && coordList.Count > 0) {
                var posID = rnd.Next(0, coordList.Count);
                var valID = rnd.Next(0, valList.Count);
                layout[coordList[posID].Y, coordList[posID].X] = valList[valID];
                coordList.RemoveAt(posID);
                valList.RemoveAt(valID);
            }
        }
        public static void RandomAssign<T>(T[,] layout, T value, int count) where T : notnull {
            RandomAssign(layout, value, count, Array.Empty<Coordinate>());
        }
        public static void RandomAssign<T>(T[,] layout, T value, int count, Coordinate[] fixedCoords) where T : notnull {
            if (layout.Length - fixedCoords.Length < count) {
                throw new ArgumentException($"Values size({count}) could not be larger than target coords size({fixedCoords.Length})");
            }

            var rnd = new Random();
            var coordList = (from coord in GetCoordinates(layout)
                             where !fixedCoords.Contains(coord)
                             select coord).ToList();
            for (int i = 0; i < count; i++) {
                int posID = rnd.Next(0, coordList.Count);
                layout[coordList[posID].Y, coordList[posID].X] = value;
                coordList.RemoveAt(posID);
            }
        }
        public static void Shuffle<T>(T[,] layout) where T : notnull {
            ShuffleHelper(layout, Array.Empty<Coordinate>());
        }
        public static void Shuffle<T>(T[,] layout, Coordinate[] fixedCoords) where T : notnull {
            ShuffleHelper(layout, fixedCoords);
        }
        public static bool IsValidCoordinate<T>(T[,] layout, Coordinate coord) where T : notnull {
            return !(coord.X < 0 || coord.X >= layout.GetLength(1) || coord.Y < 0 || coord.Y >= layout.GetLength(0));
        }
        public static bool Equals<T>(T[,] a, T[,] b) where T : notnull {
            int aRowSize = a.GetLength(0);
            int aColumnSize = a.GetLength(1);
            int bRowSize = b.GetLength(0);
            int bColumnSize = b.GetLength(1);

            if (aRowSize != bRowSize || aColumnSize != bColumnSize) {
                return false;
            }
            for (int y = 0; y < aRowSize; y++) {
                for (int x = 0; x < aColumnSize; x++) {
                    if (!a[y, x].Equals(b[y, x])) {
                        return false;
                    }
                }
            }
            return true;
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
        private LayoutHelper(TCell[,] layout) {
            _layout = new TCell[layout.GetLength(0), layout.GetLength(1)];
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    _layout[y, x] = layout[y, x];
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
        private static void ShuffleHelper<T>(T[,] layout, Coordinate[] fixedCoordinates) where T : notnull {
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