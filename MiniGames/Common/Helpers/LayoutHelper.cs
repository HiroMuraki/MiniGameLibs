using System.Text;

namespace HM.MiniGames {
    internal static class LayoutHelper {
        internal static T[] ToArray<T>(T[,] layout) {
            var result = new T[layout.Length];
            for (int y = 0; y < layout.GetLength(0); y++) {
                int offsetY = y * layout.GetLength(1);
                for (int x = 0; x < layout.GetLength(1); x++) {
                    result[offsetY + x] = layout[y, x];
                }
            }
            return result;
        }
        internal static T[,] CreateLayoutFromArray<T>(T[] array, int rowSize, int columnSize) {
            if (array.Length != rowSize * columnSize) {
                throw new ArgumentException($"size of origin array not equals rowSize * columnSize");
            }

            var result = new T[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                int offsetY = y * columnSize;
                for (int x = 0; x < columnSize; x++) {
                    result[y, x] = array[offsetY + x];
                }
            }
            return result;
        }
        internal static T[,] GetDeepCopy<T>(T[,] layout) {
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
        internal static int CountIf<T>(T[,] layout, Predicate<T> predicate) {
            int count = 0;
            foreach (var coord in GetCoordinates(layout)) {
                if (predicate(layout[coord.Y, coord.X])) {
                    count++;
                }
            }
            return count;
        }
        internal static Coordinate[] GetCoordinates<T>(T[,] layout) {
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
        internal static bool TryFindCoordinates<T>(T[,] layout, Predicate<T> predicate, out Coordinate[] result) {
            result = (from i in GetCoordinates(layout)
                      where predicate(layout[i.Y, i.X])
                      select i).ToArray();

            return result.Length != 0;
        }
        internal static Coordinate[] FindCoordinates<T>(T[,] layout, Predicate<T> predicate) {
            TryFindCoordinates(layout, predicate, out var result);
            return result;
        }
        internal static Coordinate[] GetAroundCoordinates<T>(T[,] layout, Coordinate center) {
            return (from i in _aroundDelta
                    let aCoord = center + i
                    where IsValidCoordinate(layout, aCoord)
                    select aCoord).ToArray();
        }
        internal static void RandomAssign<T>(T[,] layout, T[] values) {
            RandomAssign(layout, values, Array.Empty<Coordinate>());
        }
        internal static void RandomAssign<T>(T[,] layout, T[] values, Coordinate[] fixedCoords) {
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
        internal static void RandomAssign<T>(T[,] layout, T value, int count) {
            RandomAssign(layout, value, count, Array.Empty<Coordinate>());
        }
        internal static void RandomAssign<T>(T[,] layout, T value, int count, Coordinate[] fixedCoords) {
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
        internal static void Shuffle<T>(T[,] layout) {
            ShuffleHelper(layout, Array.Empty<Coordinate>());
        }
        internal static void Shuffle<T>(T[,] layout, Coordinate[] fixedCoords) {
            ShuffleHelper(layout, fixedCoords);
        }
        internal static bool IsValidCoordinate<T>(T[,] layout, Coordinate coord) {
            return !(coord.X < 0 || coord.X >= layout.GetLength(1) || coord.Y < 0 || coord.Y >= layout.GetLength(0));
        }
        internal static string Format2DArrays<T>(this T[,] layout, string? format) {
            string lArgs = format?.ToLower() ?? "";
            bool align = lArgs.Contains('a');
            bool matrixStyle = lArgs.Contains('m');
            int rowSize = layout.GetLength(0);
            int columnSize = layout.GetLength(1);

            int maxCellLen = -1;
            foreach (var item in layout) {
                int sLen = item?.ToString()?.Length ?? 0;
                if (sLen > maxCellLen) {
                    maxCellLen = sLen;
                }
            }

            var sb = new StringBuilder();
            if (matrixStyle) {
                for (int y = 0; y < rowSize; y++) {
                    sb.Append(CreateRow(y));
                    if (y < rowSize - 1) {
                        sb.AppendLine();
                    }
                }
            }
            else {
                for (int y = rowSize - 1; y >= 0; y--) {
                    sb.Append(CreateRow(y));
                    if (y > 0) {
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();

            string CreateRow(int y) {
                var sb = new StringBuilder();
                for (int x = 0; x < columnSize; x++) {
                    string cell = layout[y, x]?.ToString() ?? "";
                    if (align && cell.Length < maxCellLen) {
                        cell += new string(' ', maxCellLen - cell.Length);
                    }
                    if (x < columnSize - 1) {
                        cell += ' ';
                    }
                    sb.Append(cell);
                }
                return sb.ToString();
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
    }
}