using System;
using System.Collections;
using System.Collections.Generic;

// Enumerator which yields all possible tile locations within some bounds, e.g. a cartesian product of all rows and all columns
public struct AllTileLocationEnumerator : IEnumerator<TileLocation> {
    public TileLocation location;
    public TileLocation bound;

    public AllTileLocationEnumerator(TileLocation bound) {
        this.bound = bound;
        this.location = new TileLocation(-1, this.bound.c-1);
    }

    public TileLocation Current {
        get { return this.location; }
    }

    object IEnumerator.Current {
        get { return Current; }
    }

    public bool MoveNext() {
        ++this.location.c;
        if (this.location.c >= this.bound.c) {
            this.location.c = 0;
            ++this.location.r;
        }
        return this.location.r < this.bound.r;
    }

    public void Reset() {
        this.location = new TileLocation(-1, this.bound.c-1);
    }

    void IDisposable.Dispose() { }
}

// Enumerable which yields all tile locations within some bounds
public struct AllTileLocations : IEnumerable<TileLocation> {
    public TileLocation bound;

    public AllTileLocations(TileLocation bound) {
        this.bound = bound;
    }

    public IEnumerator<TileLocation> GetEnumerator() {
        return new AllTileLocationEnumerator(bound);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

// Enumerator which yields all tile locations in a column
public struct RowTileLocationEnumerator : IEnumerator<TileLocation> {
    public int row;
    public int column;
    public TileLocation bound;

    public RowTileLocationEnumerator(int row, TileLocation bound) {
        this.row = row;
        this.column = -1;
        this.bound = bound;
    }

    public TileLocation Current {
        get { return new TileLocation(this.row, this.column); }
    }

    object IEnumerator.Current {
        get { return Current; }
    }

    public bool MoveNext() {
        ++this.column;
        return this.column < this.bound.c;
    }

    public void Reset() {
        this.column = -1;
    }

    void IDisposable.Dispose() { }
}

// Enumerable which yields all tile locations in a row
public struct RowTileLocations : IEnumerable<TileLocation> {
    public int row;
    public TileLocation bound;

    public RowTileLocations(int row, TileLocation bound) {
        this.row = row;
        this.bound = bound;
    }

    public IEnumerator<TileLocation> GetEnumerator() {
        return new RowTileLocationEnumerator(this.row, this.bound);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

// Structure which can be indexed to enumerate all tile locations in a column,
// e.g. TileLocation.rows(bounds)[3] to enumerate (3,0)..(3,bound.c)
public struct TileRowIndex {
    public TileLocation bound;

    public TileRowIndex(TileLocation bound) {
        this.bound = bound;
    }

    public RowTileLocations this[int row] {
        get { return new RowTileLocations(row, this.bound); }
    }
}

// Enumerator which yields all tile locations in a column
public struct ColumnTileLocationEnumerator : IEnumerator<TileLocation> {
    public int row;
    public int column;
    public TileLocation bound;

    public ColumnTileLocationEnumerator(int column, TileLocation bound) {
        this.row = -1;
        this.column = column;
        this.bound = bound;
        this.Reset();
    }

    public TileLocation Current {
        get { return new TileLocation(this.row, this.column); }
    }

    object IEnumerator.Current {
        get { return Current; }
    }

    public bool MoveNext() {
        ++this.row;
        return this.row < this.bound.r;
    }

    public void Reset() {
        this.row = -1;
    }

    void IDisposable.Dispose() { }
}

// Enumerable which yields all tile locations in a column
public struct ColumnTileLocations : IEnumerable<TileLocation> {
    public int column;
    public TileLocation bound;

    public ColumnTileLocations(int column, TileLocation bound) {
        this.column = column;
        this.bound = bound;
    }

    public IEnumerator<TileLocation> GetEnumerator() {
        return new ColumnTileLocationEnumerator(this.column, this.bound);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

// Structure which can be indexed to enumerate all tile locations in a column,
// e.g. TileLocation.columns(bound)[3] to enumerate (0,3)..(bound.r,3)
public struct TileColumnIndex {
    public TileLocation bound;

    public TileColumnIndex(TileLocation bound) {
        this.bound = bound;
    }

    public ColumnTileLocations this[int column] {
        get { return new ColumnTileLocations(column, this.bound); }
    }
}

// Structure with various projections of a tile space bounded from (0,0) to (r,c)
public struct BoundedTiles {
    public TileLocation bound;

    public BoundedTiles(TileLocation bound) {
        this.bound   = bound;
        this.all     = new AllTileLocations(bound);
        this.rows    = new TileRowIndex(bound);
        this.columns = new TileColumnIndex(bound);
        this.extent  = this.bound.south.west;
        this.count   = this.bound.r * this.bound.c;
    }

    public IEnumerable<TileLocation> all;
    public TileRowIndex rows;
    public TileColumnIndex columns;
    public TileLocation extent;
    public int count;

    public TileLocation random() {
        return new TileLocation(RNG.random.Next(this.bound.r), RNG.random.Next(this.bound.c));
    }

    public List<TileLocation> shuffleAll() {
        List<TileLocation> locs = new List<TileLocation>(this.all);
        locs.Sort(sortRandomly);
        return locs;
    }

    private int sortRandomly(TileLocation a, TileLocation b) {
        return RNG.random.Next() - RNG.random.Next();
    }

    public int indexOf(TileLocation l) {
        return l.r * this.bound.c + l.c;
    }
}

// Structure representing a particular tile location (r, c)
public struct TileLocation {
    public static bool operator ==(TileLocation c1, TileLocation c2) {
        return c1.r == c2.r && c1.c == c2.c;
    }

    public static bool operator !=(TileLocation c1, TileLocation c2) {
        return !(c1 == c2);
    }

    public static TileLocation zero = new TileLocation(0, 0);
    public static TileLocation notFound = new TileLocation(-1, -1);

    public int r;
    public int c;

    public TileLocation(int r, int c) {
        this.r = r;
        this.c = c;
    }

    public TileLocation this[CardinalDir d] {
        get {
            switch (d) {
                case CardinalDir.NORTH: return this.north;
                case CardinalDir.EAST:  return this.east;
                case CardinalDir.SOUTH: return this.south;
                default:                return this.west;
            }
        }
    }

    public TileLocation north {
        get { return new TileLocation(this.r+1, this.c); }
    }

    public TileLocation east {
        get { return new TileLocation(this.r, this.c+1); }
    }

    public TileLocation south {
        get { return new TileLocation(this.r-1, this.c); }
    }

    public TileLocation west {
        get { return new TileLocation(this.r, this.c-1); }
    }

    public bool inBound(BoundedTiles tiles) {
        return this.inBound(tiles.bound);
    }

    public bool inBound(TileLocation bound) {
        return this.r >= 0 && this.c >= 0 && this.r < bound.r && this.c < bound.c;
    }

    override public bool Equals(object other) {
        return other is TileLocation && this == (TileLocation)other;
    }

    override public int GetHashCode() {
        return this.r << 7 | this.c;
    }

    override public string ToString() {
        return "(" + r + ", " + c + ")";
    }
}

public enum CardinalDir {
    NORTH, EAST, SOUTH, WEST
};
