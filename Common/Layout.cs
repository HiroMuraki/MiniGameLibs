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
        public int Count => RowSize * ColumnSize;
        public IEnumerable<Coordinate> Coordinates => LayoutHelper.GetCoordinates(_layout);
        #endregion

        #region Methods
        public T[] ToArray() {
            return LayoutHelper.ToArray(_layout);
        }
        public T[,] To2DArray() {
            return LayoutHelper.GetDeepCopy(_layout);
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
        public bool IsValidCoordinate(Coordinate coord) {
            return LayoutHelper.IsValidCoordinate(_layout, coord);
        }
        public Layout<T> GetExpandedLayout(Directions directions, int expandSize) {
            return new(LayoutHelper.Expand(_layout, directions, expandSize));
        }
        public Layout<T> GetShrinkedLayout(Directions directions, int expandSize) {
            return new(LayoutHelper.Shrink(_layout, directions, expandSize));
        }
        public Layout<T> GetDeepCopy() {
            return new(LayoutHelper.GetDeepCopy(_layout));
        }
        public Layout<T> Fill(T[] values) {
            LayoutHelper.Fill(_layout, values);
            return this;
        }
        public Layout<T> Fill(T[] values, Coordinate[] ignoredCoords) {
            LayoutHelper.Fill(_layout, values, ignoredCoords);
            return this;
        }
        public Layout<T> Fill(T values, int count) {
            LayoutHelper.Fill(_layout, values, count);
            return this;
        }
        public Layout<T> Fill(T value, int count, Coordinate[] ignoredCoords) {
            LayoutHelper.Fill(_layout, value, count, ignoredCoords);
            return this;
        }
        public Layout<T> RandomFill(T[] values) {
            LayoutHelper.RandomFill(_layout, values);
            return this;
        }
        public Layout<T> RandomFill(T[] values, Coordinate[] fixedCoords) {
            LayoutHelper.RandomFill(_layout, values, fixedCoords);
            return this;
        }
        public Layout<T> RandomFill(T value, int count) {
            LayoutHelper.RandomFill(_layout, value, count);
            return this;
        }
        public Layout<T> RandomFill(T value, int count, Coordinate[] fixedCoords) {
            LayoutHelper.RandomFill(_layout, value, count, fixedCoords);
            return this;
        }
        public Layout<T> Shuffle() {
            LayoutHelper.Shuffle(_layout);
            return this;
        }
        public Layout<T> Shuffle(Coordinate[] fixedCoords) {
            LayoutHelper.Shuffle(_layout, fixedCoords);
            return this;
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
            return new(rowSize, columnSize, initValue);
        }
        public static Layout<T> Create(int rowSize, int columnSize) {
            return new(rowSize, columnSize, default!);
        }
        public static Layout<T> Create(T[,] layout) {
            return new(layout);
        }
        public static Layout<T> Create(T[] array, int rowSize, int columnSize) {
            return new(LayoutHelper.Create2DArraysFromArray(array, rowSize, columnSize));
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