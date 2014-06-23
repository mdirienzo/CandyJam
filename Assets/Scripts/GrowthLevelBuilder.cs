using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthLevelBuilder : LevelBuilder {
    private LevelParameters parameters;
    private BoundedTiles tiles;
    private List<TileLocation> spawnRegion;
    private List<TileLocation> _trapLocations;
    public override List<TileLocation> trapLocations { get { return _trapLocations; } }
    private TileWall[] _walls;
    public override TileWall[] walls { get { return _walls; } }
    private List<TileLocation> randomLocations;
    private IEnumerator<TileLocation> randomLocationEnumerator;

    public GrowthLevelBuilder(LevelParameters parameters, BoundedTiles tiles, List<TileLocation> spawnRegion) {
        this.parameters = parameters;
        this._trapLocations = new List<TileLocation>();
        this.tiles = tiles;
        this.spawnRegion = spawnRegion;
        TileWall[] walls = this._walls = new TileWall[this.tiles.count];
        this.randomLocations = this.tiles.shuffleAll();
        this.randomLocationEnumerator = this.randomLocations.GetEnumerator();

        foreach (TileLocation l in tiles.all) {
            TileWall w = new TileWall();
            walls[this.tiles.indexOf(l)] = w;
            w.north = true;
            w.east  = true;
        }
    }

    private TileLocation nextRandomLocation() {
        if (!this.randomLocationEnumerator.MoveNext()) {
            this.randomLocations = this.tiles.shuffleAll();
            this.randomLocationEnumerator = this.randomLocations.GetEnumerator();
            this.randomLocationEnumerator.MoveNext();
        }

        return this.randomLocationEnumerator.Current;
    }

    private void debugWalls() {
        string s = "";
        for (int r = this.tiles.bound.r-1; r >= 0; --r) {
            for (int c = 0; c < this.tiles.bound.c; ++c) {
                TileWall w = this.walls[r*this.tiles.bound.c+c];
                s += w.north ? "^" : ".";
                s += w.east ? ">" : ".";
            }
            s += "\n";
        }
        Debug.Log(s);
    }


    public override void build() {
        for (int i = 0; i < (int)(this.tiles.count * this.parameters.trapFraction); ++i) {
            TileLocation l = TileLocation.notFound;
            int searched = 0;
            do { l = this.tiles.random(); }
            while (searched++ < this.tiles.count && (this.spawnRegion.Contains(l) || this.trapLocations.Exists(m => TileLocation.distance(l, m) < 1.5f)));

            if (searched >= this.tiles.count) break;

            this.trapLocations.Add(l);
        }

        TileLocation spawnRegionNE = TileLocation.zero;
        foreach (TileLocation l in this.spawnRegion) {
            spawnRegionNE = new TileLocation(Mathf.Max(spawnRegionNE.r, l.r), Mathf.Max(spawnRegionNE.c, l.c));
        }

        TileWall[] walls = this.walls;
        bool[] reachableTiles = new bool[this.tiles.count];

        foreach (TileLocation l in this.spawnRegion) {
            if (l.r != spawnRegionNE.r) walls[this.tiles.indexOf(l)].north = false;
            if (l.c != spawnRegionNE.c) walls[this.tiles.indexOf(l)].east  = false;
            reachableTiles[this.tiles.indexOf(l)] = true;
        }

        // because we consider the trapped tiles permanently unreachable, we'll never break down one of their walls naturally
        // so, for each trap break down some walls at random

        foreach (TileLocation l in this.trapLocations) {
            int wallsToBreak = RNG.random.Next(3)+1;
            for (int i = 0; i < wallsToBreak; ++i) {
                TileLocation[] neighbors = this.tiles.neighbors(l);
                TileLocation m = neighbors[RNG.random.Next(neighbors.Length)];
                if (m == l.north) walls[this.tiles.indexOf(l)].north = false;
                if (m == l.east)  walls[this.tiles.indexOf(l)].east  = false;
                if (m.north == l) walls[this.tiles.indexOf(m)].north = false;
                if (m.east == l)  walls[this.tiles.indexOf(m)].east  = false;
            }
        }

        while (true) {
            int searched = 0;
            TileLocation unreachable = TileLocation.notFound, reachable = TileLocation.notFound;
            while (searched++ < this.tiles.count) {
                TileLocation l = this.nextRandomLocation();
                if (reachableTiles[this.tiles.indexOf(l)]) continue;
                if (this.trapLocations.Contains(l)) continue;

                TileLocation m = TileLocation.notFound;
                TileLocation[] reachableNeighbors = Array.FindAll(this.tiles.neighbors(l), n => reachableTiles[this.tiles.indexOf(n)]);
                if (reachableNeighbors.Length > 0) {
                    m = reachableNeighbors[RNG.random.Next(reachableNeighbors.Length)];
                }

                if (m != TileLocation.notFound) {
                    unreachable = l;
                    reachable = m;
                    break;
                }
            }

            if (unreachable == TileLocation.notFound) {
                break;
            }

            if (reachable == unreachable.north) walls[this.tiles.indexOf(unreachable)].north = false;
            if (reachable == unreachable.east)  walls[this.tiles.indexOf(unreachable)].east  = false;
            if (reachable.north == unreachable) walls[this.tiles.indexOf(  reachable)].north = false;
            if (reachable.east == unreachable)  walls[this.tiles.indexOf(  reachable)].east  = false;

            reachableTiles[this.tiles.indexOf(unreachable)] = true;
        }

        for (int i = 0; i < (int)Mathf.Ceil(this.tiles.count * this.parameters.extraPathFraction); ++i) {
            TileLocation l;
            int index;
            int searched = 0;
            do { l = this.nextRandomLocation(); index = this.tiles.indexOf(l); }
            while (!(walls[index].north || walls[index].east) && searched++ < this.tiles.count);

            switch (RNG.random.Next(2)) {
                case 0: walls[index].north = false; break;
                case 1: walls[index].east  = false; break;
            }
        }
    }
}

