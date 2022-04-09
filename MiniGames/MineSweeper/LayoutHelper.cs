namespace HM.MiniGames.Minesweeper {
    /// <summary>
    /// 布局信息控制器
    /// </summary>
    internal class LayoutHelper {
        #region Properties
        internal BlockType this[Coordinate coord] {
            get {
                return _layout[coord.Y, coord.X];
            }
        }
        internal BlockType this[int x, int y] {
            get {
                return _layout[y, x];
            }
        }
        internal IEnumerable<Coordinate> Coordinates {
            get {
                for (int y = 0; y < RowSize; y++) {
                    for (int x = 0; x < ColumnSize; x++) {
                        yield return new Coordinate(x, y);
                    }
                }
            }
        }
        internal int RowSize => _layout.GetLength(0);
        internal int ColumnSize => _layout.GetLength(1);
        internal int MineCount { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// 创建布局信息
        /// </summary>
        /// <param name="rowSize"></param>
        /// <param name="columnSize"></param>
        /// <param name="mineCount"></param>
        /// <returns></returns>
        internal static LayoutHelper Create(int rowSize, int columnSize, int mineCount) {
            return new LayoutHelper(rowSize, columnSize, mineCount);
        }
        /// <summary>
        /// 打乱布局信息
        /// </summary>
        /// <returns></returns>
        internal LayoutHelper Shuffle(Coordinate[] protectedCoords) {
            Shuffle(_layout, protectedCoords);
            return this;
        }
        internal LayoutHelper Shuffle() {
            Shuffle(_layout, Array.Empty<Coordinate>());
            return this;
        }
        /// <summary>
        /// 获取布局深复制副本
        /// </summary>
        /// <returns></returns>
        internal BlockType[,] GetLayoutCopy() {
            var copy = new BlockType[RowSize, ColumnSize];
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    copy[y, x] = _layout[y, x];
                }
            }
            return copy;
        }
        /// <summary>
        /// 获取坐标周围八个方向的坐标
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        internal IEnumerable<Coordinate> GetAroundCoordinates(Coordinate center) {
            foreach (var (x, y) in _aroundDelta) {
                var coord = new Coordinate(center.X + x, center.Y + y);
                if (IsValidCoordinate(coord.X, coord.Y)) {
                    yield return coord;
                }
            }
        }
        /// <summary>
        /// 获取符合条件的坐标
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal IEnumerable<Coordinate> GetCoordinates(Predicate<BlockType> predicate) {
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    if (predicate(_layout[y, x])) {
                        yield return new Coordinate(x, y);
                    }
                }
            }
        }
        #endregion

        private LayoutHelper(int rowSize, int columnSize, int mineCount) {
            _layout = CreateLayout(rowSize, columnSize, mineCount);
            MineCount = mineCount;
        }
        private readonly BlockType[,] _layout;
        /// <summary>
        /// 指示四周八方块的相对坐标
        /// </summary>
        private static readonly List<Coordinate> _aroundDelta = new() {
#pragma warning disable format
                (-1, -1), (0, -1), (1, -1),
                (-1,  0),          (1,  0),
                (-1,  1), (0,  1), (1,  1)
#pragma warning restore format
            };
        /// <summary>
        /// 计数指定坐标周围符合条件的方块
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private int CountNearby(int x, int y, Predicate<BlockType> predicate) {
            int count = 0;
            foreach (var aCoord in GetAroundCoordinates(new Coordinate(x, y))) {
                if (predicate(_layout[aCoord.Y, aCoord.X])) {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取一个顺序分布的布局信息
        /// </summary>
        /// <param name="rowSize">布局行数</param>
        /// <param name="columnSize">布局列数</param>
        /// <param name="mineCount">雷数</param>
        /// <returns>布局信息，其中以0标识空块，1标识雷</returns>
        private static BlockType[,] CreateLayout(int rowSize, int columnSize, int mineCount) {
            var layout = new BlockType[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    if (y * columnSize + x < mineCount) {
                        layout[y, x] = BlockType.Mine;
                    }
                    else {
                        layout[y, x] = BlockType.Blank;
                    }
                }
            }
            return layout;
        }
        /// <summary>
        /// 打乱布局
        /// </summary>
        /// <param name="layout"></param>
        private void Shuffle(BlockType[,] layout, Coordinate[] protectedCoords) {
            var rnd = new Random();

            var allowedCoords = (from i in Coordinates
                                 where !protectedCoords.Contains(i)
                                 select i).ToArray();

            var unsafeCoords = (from i in protectedCoords
                                where layout[i.Y, i.X] == BlockType.Mine
                                select i).ToArray();

            // 打乱除了保护坐标外的其他坐标序列
            // 然后将保护坐标的不安全块设置为安全块，并从其他坐标序列中挑选等同数量的空块设为不安全块
            for (int i = 0; i < allowedCoords.Length; i++) {
                var coord1 = allowedCoords[i];
                var coord2 = allowedCoords[rnd.Next(i, allowedCoords.Length)];
                var t = layout[coord1.Y, coord1.X];
                layout[coord1.Y, coord1.X] = layout[coord2.Y, coord2.X];
                layout[coord2.Y, coord2.X] = t;
            }

            foreach (var coord in unsafeCoords) {
                layout[coord.Y, coord.X] = BlockType.Blank;
            }

            var blankCoords = (from i in allowedCoords
                               where layout[i.Y, i.X] == BlockType.Blank
                               select i).ToList();

            for (int i = 0; i < unsafeCoords.Length; i++) {
                int next = rnd.Next(0, blankCoords.Count);
                layout[blankCoords[next].Y, blankCoords[next].X] = BlockType.Mine;
                blankCoords.RemoveAt(next);
            }
        }
        /// <summary>
        /// 检查坐标的有效性
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsValidCoordinate(int x, int y) {
            return !(x < 0 || x >= ColumnSize || y < 0 || y >= RowSize);
        }
    }
}