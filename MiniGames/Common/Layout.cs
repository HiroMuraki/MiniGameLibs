using System.Text;

namespace HM.MiniGames {
    public sealed class Layout<T>
        : IEquatable<Layout<T>>, IFormattable
        where T : notnull {
        #region Properties
        public T this[int x, int y] {
            get {
                return _layout[y, x];
            }
            set {
                _layout[y, x] = value;
            }
        }
        public T this[Coordinate coord] {
            get {
                return _layout[coord.Y, coord.X];
            }
            set {
                _layout[coord.Y, coord.X] = value;
            }
        }
        public int RowSize => _layout.GetLength(0);
        public int ColumnSize => _layout.GetLength(1);
        public IEnumerable<Coordinate> Coordinates => LayoutHelper.GetCoordinates(_layout);
        #endregion

        #region Methods
        public T[] ToArray() {
            return LayoutHelper.ToArray(_layout);
        }
        public Layout<T> GetDeepCopy() {
            return new Layout<T>(LayoutHelper.GetDeepCopy(_layout));
        }
        public int CountIf(Predicate<T> predicate) {
            return LayoutHelper.CountIf(_layout, predicate);
        }
        public Coordinate[] FindCoordinates(Predicate<T> predicate) {
            return LayoutHelper.FindCoordinates(_layout, predicate);
        }
        public bool TryFindCoordinates(Predicate<T> predicate, out Coordinate[] coord) {
            return LayoutHelper.TryFindCoordinates(_layout, predicate, out coord);
        }
        public Coordinate[] GetAroundCoordinates(Coordinate center) {
            return LayoutHelper.GetAroundCoordinates(_layout, center);
        }
        public void RandomAssign(T[] values) {
            LayoutHelper.RandomAssign(_layout, values);
        }
        public void RandomAssign(T[] values, Coordinate[] fixedCoords) {
            LayoutHelper.RandomAssign(_layout, values, fixedCoords);
        }
        public void RandomAssign(T value, int count) {
            LayoutHelper.RandomAssign(_layout, value, count);
        }
        public void RandomAssign(T value, int count, Coordinate[] fixedCoords) {
            LayoutHelper.RandomAssign(_layout, value, count, fixedCoords);
        }
        public void Shuffle() {
            LayoutHelper.Shuffle(_layout);
        }
        public void Shuffle(Coordinate[] fixedCoords) {
            LayoutHelper.Shuffle(_layout, fixedCoords);
        }
        public bool IsValidCoordinate(Coordinate coord) {
            return LayoutHelper.IsValidCoordinate(_layout, coord);
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
        public string ToString(string? format, IFormatProvider? formatProvider) {
            return LayoutHelper.Format2DArrays(_layout, format);
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
            if (obj.GetType() != typeof(Layout<T>)) {
                return false;
            }
            return Equals((Layout<T>)obj);
        }
        public bool Equals(Layout<T>? other) {
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
                    if (!_layout[y, x].Equals(other._layout[y, x])) {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region Static Methods
        public static Layout<T> Create(int rowSize, int columnSize, T initValue) {
            return new Layout<T>(rowSize, columnSize, initValue);
        }
        public static Layout<T> Create(int rowSize, int columnSize) {
            return new Layout<T>(rowSize, columnSize, default!);
        }
        public static Layout<T> Create(T[,] layout) {
            return new Layout<T>(layout);
        }
        public static Layout<T> Create(T[] array, int rowSize, int columnSize) {
            return new(LayoutHelper.CreateLayoutFromArray(array, rowSize, columnSize));
        }
        #endregion

        #region Helpers
        private readonly T[,] _layout;
        private Layout(int rowSize, int columnSize, T initValue) {
            _layout = new T[rowSize, columnSize];
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    _layout[y, x] = initValue;
                }
            }
        }
        private Layout(T[,] layout) {
            _layout = new T[layout.GetLength(0), layout.GetLength(1)];
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    _layout[y, x] = layout[y, x];
                }
            }
        }
        #endregion
    }
}