namespace HM.MiniGames.Minesweeper {
    public class MinesweeperHelper {
        public Layout<IBlock> Layout { get; }

        public bool OpenRecursively(Coordinate coord, Action<Coordinate> openCallBack, out Coordinate[] nodes) {
            var nodeList = new List<Coordinate>();
            bool result = OpenRecursivelyCore(coord, openCallBack, ref nodeList);
            nodes = nodeList.ToArray();
            return result;
        }
        public int CountNearby(Coordinate coord, Predicate<IBlock> predicate) {
            int count = 0;
            foreach (var aCoord in Layout.GetAroundCoordinates(coord)) {
                if (predicate(Layout[aCoord])) {
                    count++;
                }
            }
            return count;
        }
        public void CheckCoordinate(Coordinate coord) {
            if (!Layout.IsValidCoordinate(coord)) {
                throw new CoordinateOutOfRangeException(coord);
            }
        }
        public bool IsGameCompleted() {
            foreach (var block in Layout) {
                if (block.Type == BlockType.Blank && !block.IsOpen) {
                    return false;
                }
            }
            return true;
        }

        public MinesweeperHelper(Layout<IBlock> layout) {
            Layout = layout;
        }
        public MinesweeperHelper(Grid<IBlock> grid) {
            Layout = Layout<IBlock>.Create(grid.RowSize, grid.ColumnSize);
            foreach (var coord in grid.Coordinates) {
                Layout[coord] = grid[coord];
            }
        }

        private bool OpenRecursivelyCore(Coordinate coord, Action<Coordinate> openCallBack, ref List<Coordinate> nodes) {
            openCallBack(coord);
            nodes.Add(coord);
            // 如果当前块周围有雷并且周围的标记数量小于周围雷数，跳过
            if (Layout[coord].NearbyMines > 0 && CountNearby(coord, b => b.IsFlagged) < Layout[coord].NearbyMines) {
                return false;
            }
            // 周围无雷，递归打开周围的方格
            foreach (var aCoord in Layout.GetAroundCoordinates(coord)) {
                // 跳过已开或标记的方格
                if (Layout[aCoord].IsOpen || Layout[aCoord].IsFlagged) {
                    continue;
                }
                OpenRecursivelyCore(aCoord, openCallBack, ref nodes);
            }
            return true;
        }
    }
}