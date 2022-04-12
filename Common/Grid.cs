using System.Collections;

namespace HM.MiniGames {
    /// <summary>
    /// 二维坐标网格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Grid<T> : IEnumerable<T>, IEnumerable, IEquatable<Grid<T>>, IFormattable {
        public static readonly Grid<T> Empty = new(0, 0);

        #region Properties
        internal T[,] Origin2DArray { get; }
        public T this[int x, int y] {
            get {
                return Origin2DArray[y, x];
            }
            set {
                if (_locked) {
                    throw new InvalidOperationException("Grid locked, unable to modify");
                }
                Origin2DArray[y, x] = value;
            }
        }
        public T this[Coordinate coord] {
            get {
                return this[coord.X, coord.Y];
            }
            set {
                this[coord.X, coord.Y] = value;
            }
        }
        public IEnumerable<Coordinate> Coordinates => LayoutHelper.GetCoordinates(Origin2DArray);
        public int RowSize => Origin2DArray.GetLength(0);
        public int ColumnSize => Origin2DArray.GetLength(1);
        #endregion

        #region Methods
        public Grid<T> Lock() {
            _locked = true;
            return this;
        }
        public Grid<T> Unlock() {
            _locked = false;
            return this;
        }
        public Coordinate[] FindCoordinates(Predicate<T> predicate) {
            var result = new List<Coordinate>();
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    if (predicate(Origin2DArray[y, x])) {
                        result.Add(new Coordinate(x, y));
                    }
                }
            }
            return result.ToArray();
        }
        public bool TryFindCoordinates(Predicate<T> predicate, out Coordinate[] result) {
            result = FindCoordinates(predicate);
            return result.Length != 0;
        }
        public IEnumerator<T> GetEnumerator() {
            foreach (var item in Origin2DArray) {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public string ToString(string? format, IFormatProvider? formatProvider) {
            return LayoutHelper.Format2DArrays(Origin2DArray, format);
        }
        public string ToString(string? format) {
            return ToString(format, null);
        }
        public override string ToString() {
            return ToString("", null);
        }
        public override bool Equals(object? obj) {
            if (obj is null) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof(Grid<T>)) {
                return false;
            }
            return Equals((Grid<T>)obj);
        }
        public override int GetHashCode() {
            return (RowSize << 2) ^ ColumnSize;
        }
        public bool Equals(Grid<T>? other) {
            if (other is null) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            if (RowSize != other.RowSize || ColumnSize != other.ColumnSize) {
                return false;
            }
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    if (Equals(Origin2DArray[y, x], other.Origin2DArray[x, y])) {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        public static Grid<T> Create(int rowSize, int columnSize) {
            return new(rowSize, columnSize);
        }

        private Grid(int rowSize, int columnSize) {
            Origin2DArray = new T[rowSize, columnSize];
        }
        private bool _locked;
    }
}