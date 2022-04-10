using System.Collections;

namespace HM.MiniGames {
    /// <summary>
    /// 二维坐标网格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T> : IEnumerable<T>, IEnumerable {
        public T this[Coordinate coord] {
            get {
                return _objects[coord.Y, coord.X];
            }
            set {
                if (_locked) {
                    throw new InvalidOperationException("Grid locked, unable to modify");
                }
                _objects[coord.Y, coord.X] = value;
            }
        }
        public T this[int x, int y] {
            get {
                return _objects[y, x];
            }
            set {
                if (_locked) {
                    throw new InvalidOperationException("Grid locked, unable to modify");
                }
                _objects[y, x] = value;
            }
        }
        public IEnumerable<Coordinate> Coordinates {
            get {
                for (int x = 0; x < ColumnSize; x++) {
                    for (int y = 0; y < RowSize; y++) {
                        yield return new Coordinate(x, y);
                    }
                }
            }
        }
        public int RowSize => _objects.GetLength(0);
        public int ColumnSize => _objects.GetLength(1);

        public static Grid<T> Create(int rowSize, int columnSize) {
            return new Grid<T>(rowSize, columnSize);
        }
        public void Lock() {
            _locked = true;
        }
        public void Unlock() {
            _locked = false;
        }
        public IEnumerable<Coordinate> FindCoordinates(Predicate<T> predicate) {
            for (int x = 0; x < ColumnSize; x++) {
                for (int y = 0; y < RowSize; y++) {
                    if (predicate(_objects[y, x])) {
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
            foreach (var item in _objects) {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private Grid(int rowSize, int columnSize) {
            _objects = new T[rowSize, columnSize];
        }
        private readonly T[,] _objects;
        private bool _locked;
    }
}