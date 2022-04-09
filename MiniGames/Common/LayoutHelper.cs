namespace HM.MiniGames {
    public class LayoutHelper<TCell> {
        #region Properties
        public TCell this[int x, int y] {
            get {
                return Layout[y, x];
            }
            set {
                Layout[y, x] = value;
            }
        }
        public TCell this[Coordinate coord] {
            get {
                return Layout[coord.Y, coord.X];
            }
            set {
                Layout[coord.Y, coord.X] = value;
            }
        }
        public TCell[,] Layout { get; }
        public int RowSize => Layout.GetLength(0);
        public int ColumnSize => Layout.GetLength(1);
        public IEnumerable<Coordinate> Coordinates => GetCoordinates(Layout);
        #endregion

        #region Methods
        public Coordinate[] FindCoordinates(Predicate<TCell> predicate) {
            return FindCoordinates(predicate);
        }
        public bool TryFindCoordinates(Predicate<TCell> predicate, out Coordinate[] coord) {
            return TryFindCoordinates(predicate, out coord);
        }
        public Coordinate[] GetAroundCoordinates(Coordinate center) {
            return GetAroundCoordinates(Layout, center);
        }
        public void Shuffle() {
            Shuffle(Layout);
        }
        public void Shuffle(Coordinate[] fixedCoordinates) {
            Shuffle(Layout, fixedCoordinates);
        }
        public bool IsValidCoordinate(Coordinate coord) {
            return IsValidCoordinate(Layout, coord);
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
        private LayoutHelper(int rowSize, int columnSize, TCell initValue) {
            Layout = new TCell[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    Layout[y, x] = initValue;
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