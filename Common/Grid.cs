using System.Collections;

namespace HM.MiniGames {
    /// <summary>
    /// 二维坐标网格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T> : IEnumerable<T>, IEnumerable {
        #region Properties
        public T this[Coordinate coord] {
            get {
                return _layout[coord.Y, coord.X];
            }
            set {
                if (_locked) {
                    throw new InvalidOperationException("Grid locked, unable to modify");
                }
                _layout[coord.Y, coord.X] = value;
            }
        }
        public T this[int x, int y] {
            get {
                return _layout[y, x];
            }
            set {
                if (_locked) {
                    throw new InvalidOperationException("Grid locked, unable to modify");
                }
                _layout[y, x] = value;
            }
        }
        public IEnumerable<Coordinate> Coordinates {
            get {
                return LayoutHelper.GetCoordinates(_layout);
            }
        }
        public int RowSize => _layout.GetLength(0);
        public int ColumnSize => _layout.GetLength(1);
        #endregion

        #region Methods
        public void Lock() {
            _locked = true;
        }
        public void Unlock() {
            _locked = false;
        }
        public IEnumerable<Coordinate> FindCoordinates(Predicate<T> predicate) {
            for (int x = 0; x < ColumnSize; x++) {
                for (int y = 0; y < RowSize; y++) {
                    if (predicate(_layout[y, x])) {
                        yield return new Coordinate(x, y);
                    }
                }
            }
        }
        public bool TryFindCoordinates(Predicate<T> predicate, out Coordinate[] result) {
            result = FindCoordinates(predicate).ToArray();
            return result.Length != 0;
        }
        public IEnumerator<T> GetEnumerator() {
            foreach (var item in _layout) {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        public static Grid<T> Create(int rowSize, int columnSize) {
            return new Grid<T>(rowSize, columnSize);
        }

        private Grid(int rowSize, int columnSize) {
            _layout = new T[rowSize, columnSize];
        }
        private readonly T[,] _layout;
        private bool _locked;
    }
}