using System.Collections.Generic;
using UnityEngine;

public class RandomWalkLevelBuilder : LevelBuilder {
    private int extraWalks;
    private int numTraps;
    private BoundedTiles tiles;
    private List<TileLocation> spawnRegion;
    private List<TileLocation> _trapLocations;
    public override List<TileLocation> trapLocations { get { return _trapLocations; } }
    private TileWall[] _walls;
    public override TileWall[] walls { get { return _walls; } }
    private bool[] reachable;
    private bool[] flood;
    private List<TileLocation> randomLocations;
    private IEnumerator<TileLocation> randomLocationEnumerator;

    public RandomWalkLevelBuilder(int extraWalks, int numTraps, BoundedTiles tiles, List<TileLocation> spawnRegion) {
        this.extraWalks = extraWalks;
        this.numTraps = numTraps;
        this._trapLocations = new List<TileLocation>();
        this.tiles = tiles;
        this.spawnRegion = spawnRegion;
        this._walls = new TileWall[this.tiles.count];
        this.reachable = new bool[this.tiles.count];
        this.flood = new bool[this.tiles.count];
        this.randomLocations = this.tiles.shuffleAll();
        this.randomLocationEnumerator = this.randomLocations.GetEnumerator();

        foreach (TileLocation l in tiles.all) {
            TileWall w = new TileWall();
            this.walls[this.tiles.indexOf(l)] = w;
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
        for (int i = 0; i < this.numTraps; ++i) {
            TileLocation l;
            do { l = this.tiles.random(); } while (this.spawnRegion.Contains(l));

            this.trapLocations.Add(l);
        }

        TileLocation spawnRegionNE = TileLocation.zero;
        foreach (TileLocation l in this.spawnRegion) {
            spawnRegionNE = new TileLocation(Mathf.Max(spawnRegionNE.r, l.r), Mathf.Max(spawnRegionNE.c, l.c));
        }

        foreach (TileLocation l in this.spawnRegion) {
            if (l.r != spawnRegionNE.r) this.walls[this.tiles.indexOf(l)].north = false;
            if (l.c != spawnRegionNE.c) this.walls[this.tiles.indexOf(l)].east  = false;
        }

        // create walls. start by making everything a wall, then pick random points and
        // randomly walk from them bashing down walls for a finite number of steps. in
        // order to remove isolated clusters keep on picking points that cannot be reached
        // from the player origin

        int walks = 0;
        while (walks < 400) {
            this.markReachable(this.spawnRegion, this.reachable);

            TileLocation isolatedLocation = this.findIsolatedLocation(); // to off 'em

            if (isolatedLocation == TileLocation.notFound) {
                // no more isolated locations, so be done
                break;
            }

            TileLocation nearestReachable = this.findNearestReachablePointTo(isolatedLocation);

            if (walks > 200) {
                Debug.Log("level build not converging: iteration " + walks + ": walking from " + isolatedLocation + " to " + nearestReachable);
            }

            this.randomWalk(isolatedLocation, nearestReachable);
            ++walks;
        }

        Debug.Log("finished building necessary walls after " + walks + " walks");

        for (int additionalWalks = 0; additionalWalks < this.extraWalks; ++additionalWalks) {
            TileLocation a;
            TileLocation b;
            float dist;
            do {
                a = this.nextRandomLocation();
                b = this.nextRandomLocation();
                dist = TileLocation.distance(a, b);
            } while (dist <= 1.0f || dist > this.tiles.bound.c/3*2);

            this.randomWalk(a, b);
        }
    }

    private bool isReachable(TileLocation loc) {
        return this.reachable[this.tiles.indexOf(loc)];
    }

    private TileLocation findIsolatedLocation() {
        TileLocation bestLoc = TileLocation.notFound;
        int bestReach = this.tiles.count;
        int count = 0;
        while (bestReach > 1 && count++ < this.tiles.count) {
            TileLocation loc = this.nextRandomLocation();
            int locIndex = this.tiles.indexOf(loc);
            if (this.trapLocations.Contains(loc) || this.reachable[locIndex]) {
                continue;
            }
            this.markReachable(new TileLocation[] { loc }, this.flood);
            int reach = 0;
            for (int i = 0; i < this.flood.Length; ++i) {
                if (this.flood[i]) { reach += 1; }
            }
            if (reach < bestReach) {
                bestReach = reach;
                bestLoc = loc;
            }
        }

        return bestLoc;
    }

    private bool walled(TileLocation l, CardinalDir d) {
        switch (d) {
            case CardinalDir.NORTH: return this.walls[this.tiles.indexOf(l)].north;
            case CardinalDir.EAST:  return this.walls[this.tiles.indexOf(l)].east;
            case CardinalDir.SOUTH: return !l.south.inBound(this.tiles) || this.walls[this.tiles.indexOf(l.south)].north;
            default:                return !l.west .inBound(this.tiles) || this.walls[this.tiles.indexOf(l.west )].east;
        }
    }

    private void markReachable(IEnumerable<TileLocation> initial, bool[] marks) {
        for (int i = 0; i < marks.Length; ++i) {
            marks[i] = false;
        }

        foreach (TileLocation loc in initial) {
            marks[this.tiles.indexOf(loc)] = true;
        }

        bool updated;
        do {
            updated = false;

            foreach (TileLocation l in this.tiles.all) {
                bool wasMarked = marks[this.tiles.indexOf(l)];
                if (wasMarked) continue;

                TileLocation n = l.north;
                TileLocation e = l.east;
                TileLocation s = l.south;
                TileLocation w = l.west;

                bool nt = (this.passable(n) && !this.walled(l, CardinalDir.NORTH)) ? marks[this.tiles.indexOf(n)] : false;
                bool et = (this.passable(e) && !this.walled(l, CardinalDir.EAST )) ? marks[this.tiles.indexOf(e)] : false;
                bool st = (this.passable(s) && !this.walled(l, CardinalDir.SOUTH)) ? marks[this.tiles.indexOf(s)] : false;
                bool wt = (this.passable(w) && !this.walled(l, CardinalDir.WEST )) ? marks[this.tiles.indexOf(w)] : false;

                bool nowMarked = wasMarked || nt || et || st || wt;

                updated = updated || (wasMarked != nowMarked);
                marks[this.tiles.indexOf(l)] = nowMarked;

            }
        } while (updated);
    }

    private bool passable(TileLocation l) {
        return l.inBound(this.tiles) && !this.trapLocations.Contains(l);
    }

    private TileLocation findNearestReachablePointTo(TileLocation l) {
        float bestDist = -1.0f;
        TileLocation bestLoc = TileLocation.notFound;

        for (int i = 0; i < this.reachable.Length; ++i) {
            TileLocation m = this.tiles.fromIndex(i);
            if (m == l) continue;
            if (!this.reachable[i] || this.trapLocations.Contains(m)) continue;

            float dist = TileLocation.distance(l, m);
            if (bestDist < 0.0f || dist < bestDist) {
                bestDist = dist;
                bestLoc = m;
            }
        }

        return bestLoc;
    }

    private void randomWalk(TileLocation location, TileLocation target) {

        CardinalDir[] onePreferredDirection = new CardinalDir[1];
        CardinalDir[] twoPreferredDirections = new CardinalDir[2];

        int steps = 0;
        while (steps++ < 6) {
            if (target == location) return;
            CardinalDir[] preferredDirections;
            if (target.r == location.r || target.c == location.c) {
                preferredDirections = onePreferredDirection;
                if (target.r < location.r) {
                    preferredDirections[0] = CardinalDir.SOUTH;
                } else if (target.r > location.r) {
                    preferredDirections[0] = CardinalDir.NORTH;
                } else if (target.c < location.c) {
                    preferredDirections[0] = CardinalDir.WEST;
                } else if (target.c > location.c) {
                    preferredDirections[0] = CardinalDir.EAST;
                }
            } else {
                preferredDirections = twoPreferredDirections;
                if (target.r < location.r) {
                    preferredDirections[0] = CardinalDir.SOUTH;
                } else {
                    preferredDirections[0] = CardinalDir.NORTH;
                }

                if (target.c < location.c) {
                    preferredDirections[1] = CardinalDir.WEST;
                } else {
                    preferredDirections[1] = CardinalDir.EAST;
                }
            }

            TileLocation next;
            CardinalDir dir;
            do {
                int rand = RNG.random.Next(4 + preferredDirections.Length);
                switch (rand) {
                    case 0: dir = CardinalDir.NORTH; break;
                    case 1: dir = CardinalDir.EAST;  break;
                    case 2: dir = CardinalDir.SOUTH; break;
                    case 3: dir = CardinalDir.WEST;  break;
                    default: dir = preferredDirections[(rand - 4)]; break;
                }
                dir = (CardinalDir)RNG.random.Next(4);
                next = location[dir];
            } while (!next.inBound(this.tiles));

            switch (dir) {
                case CardinalDir.NORTH:
                    this.walls[this.tiles.indexOf(location)].north = false;
                    break;
                case CardinalDir.EAST:
                    this.walls[this.tiles.indexOf(location)].east = false;
                    break;
                case CardinalDir.SOUTH:
                    this.walls[this.tiles.indexOf(next)].north = false;
                    break;
                default:
                    this.walls[this.tiles.indexOf(next)].east = false;
                    break;
            }

            location = next;
        }
    }
}

