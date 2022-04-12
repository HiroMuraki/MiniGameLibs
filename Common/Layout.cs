using System.Text;
using System.Collections;

namespace HM.MiniGames {
    public sealed class Layout<T>
        : IEquatable<Layout<T>>, IEnumerable<T>, IEnumerable, IFormattable {
        #region Properties
        public T this[int x, int y] {
            get {
                return _grid[y, x];
            }
            set {
                _grid[y, x] = value;
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
        public int RowSize => _grid.RowSize;
        public int ColumnSize => _grid.ColumnSize;
        public int Count => RowSize * ColumnSize;
        public IEnumerable<Coordinate> Coordinates => _grid.Coordinates;
        #endregion

        #region Methods
        public T[] ToArray() {
            return LayoutHelper.ToArray(_grid.Origin2DArray);
        }
        public T[,] To2DArray() {
            return LayoutHelper.GetDeepCopy(_grid.Origin2DArray);
        }
        public int CountIf(Predicate<T> predicate) {
            return LayoutHelper.CountIf(_grid.Origin2DArray, predicate);
        }
        public Coordinate[] FindCoordinates(Predicate<T> predicate) {
            return LayoutHelper.FindCoordinates(_grid.Origin2DArray, predicate);
        }
        public bool TryFindCoordinates(Predicate<T> predicate, out Coordinate[] coord) {
            return LayoutHelper.TryFindCoordinates(_grid.Origin2DArray, predicate, out coord);
        }
        public Coordinate[] GetAroundCoordinates(Coordinate center) {
            return LayoutHelper.GetAroundCoordinates(_grid.Origin2DArray, center);
        }
        public bool IsValidCoordinate(Coordinate coord) {
            return LayoutHelper.IsValidCoordinate(_grid.Origin2DArray, coord);
        }
        public Layout<T> GetExpandedLayout(Directions directions, int expandSize) {
            return new(LayoutHelper.Expand(_grid.Origin2DArray, directions, expandSize));
        }
        public Layout<T> GetShrinkedLayout(Directions directions, int expandSize) {
            return new(LayoutHelper.Shrink(_grid.Origin2DArray, directions, expandSize));
        }
        public Layout<T> GetDeepCopy() {
            return new(LayoutHelper.GetDeepCopy(_grid.Origin2DArray));
        }
        public Layout<T> Fill(T[] values) {
            LayoutHelper.Fill(_grid.Origin2DArray, values);
            return this;
        }
        public Layout<T> Fill(T[] values, Coordinate[] ignoredCoords) {
            LayoutHelper.Fill(_grid.Origin2DArray, values, ignoredCoords);
            return this;
        }
        public Layout<T> Fill(T value) {
            LayoutHelper.Fill(_grid.Origin2DArray, value);
            return this;
        }
        public Layout<T> Fill(T values, int count) {
            LayoutHelper.Fill(_grid.Origin2DArray, values, count);
            return this;
        }
        public Layout<T> Fill(T value, int count, Coordinate[] ignoredCoords) {
            LayoutHelper.Fill(_grid.Origin2DArray, value, count, ignoredCoords);
            return this;
        }
        public Layout<T> RandomFill(T[] values) {
            LayoutHelper.RandomFill(_grid.Origin2DArray, values);
            return this;
        }
        public Layout<T> RandomFill(T[] values, Coordinate[] fixedCoords) {
            LayoutHelper.RandomFill(_grid.Origin2DArray, values, fixedCoords);
            return this;
        }
        public Layout<T> RandomFill(T value, int count) {
            LayoutHelper.RandomFill(_grid.Origin2DArray, value, count);
            return this;
        }
        public Layout<T> RandomFill(T value, int count, Coordinate[] fixedCoords) {
            LayoutHelper.RandomFill(_grid.Origin2DArray, value, count, fixedCoords);
            return this;
        }
        public Layout<T> Shuffle() {
            LayoutHelper.Shuffle(_grid.Origin2DArray);
            return this;
        }
        public Layout<T> Shuffle(Coordinate[] fixedCoords) {
            LayoutHelper.Shuffle(_grid.Origin2DArray, fixedCoords);
            return this;
        }
        public string ToString(string? format, IFormatProvider? formatProvider) {
            return _grid.ToString(format, formatProvider);
        }
        public string ToString(string? format) {
            return ToString(format, null);
        }
        public override string ToString() {
            return ToString("");
        }
        public override int GetHashCode() {
            return _grid.GetHashCode();
        }
        public override bool Equals(object? obj) {
            return _grid.Equals(obj);
        }
        public bool Equals(Layout<T>? other) {
            return _grid.Equals(other);
        }
        public IEnumerator<T> GetEnumerator() {
            foreach (var item in _grid) {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
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
        private readonly Grid<T> _grid;
        private Layout(int rowSize, int columnSize, T initValue) {
            _grid = Grid<T>.Create(rowSize, columnSize);
            for (int y = 0; y < rowSize; y++) {
                for (int x = 0; x < columnSize; x++) {
                    _grid.Origin2DArray[y, x] = initValue;
                }
            }
        }
        private Layout(T[,] layout) {
            _grid = Grid<T>.Create(layout.GetLength(0), layout.GetLength(1));
            for (int y = 0; y < RowSize; y++) {
                for (int x = 0; x < ColumnSize; x++) {
                    _grid.Origin2DArray[y, x] = layout[y, x];
                }
            }
        }
        #endregion
    }
}