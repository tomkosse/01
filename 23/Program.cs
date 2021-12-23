using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _23
{
    public record Tile(int X, int Y, char? AlcoveCharacter)
    {
        public IEnumerable<Tile> Adjecent { get; set; }
        public bool Occupied
        {
            get
            {
                return Amphipod != null;
            }
        }

        public bool OccupiedByRightAmphipod
        {
            get
            {
                return Occupied && Amphipod.Character == AlcoveCharacter.Value;
            }
        }

        public Amphipod Amphipod { get; set; }

        public bool IsAlcove
        {
            get
            {
                return AlcoveCharacter.HasValue;
            }
        }

        public bool IsHallway
        {
            get
            {
                return !IsAlcove;
            }
        }
    }

    public class Amphipod
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public char Character { get; }

        public Tile Tile { get; private set; }

        public int Cost
        {
            get
            {
                return Character switch
                {
                    'A' => 1,
                    'B' => 10,
                    'C' => 100,
                    'D' => 1000,
                    _ => throw new Exception("IMPOSSIBLE")
                };
            }
        }

        public Amphipod(int x, int y, char character, Tile tile)
        {
            this.X = x;
            this.Y = y;
            this.Character = character;
            this.Tile = tile;
        }

        public void MoveTo(Game game, Tile tile)
        {
            this.X = tile.X;
            this.Y = tile.Y;

            this.Tile.Amphipod = null;
            tile.Amphipod = this;
            this.Tile = tile;
        }
    }
    public class Game
    {
        public List<Tile> Tiles { get; }
        private List<Amphipod> amphipods;

        public static Dictionary<Game, int> ZobristTable = new Dictionary<Game, int>();

        public Game(List<Tile> tiles, List<Amphipod> amphipods)
        {
            this.Tiles = tiles;
            this.amphipods = amphipods;
        }

        public bool IsDone
        {
            get
            {
                return amphipods.All(a => a.Tile.AlcoveCharacter == a.Character);
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 1430287;
            for (int i = 0; i < Tiles.Count; i++)
            {
                var tile = Tiles[i];
                hashCode = hashCode * 7302013 ^ tile.GetHashCode();
            }
            for(int i=0; i < amphipods.Count; i++)
            {
                var amphi = amphipods[i];
                hashCode = hashCode * 3102037 ^ amphi.GetHashCode();
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (typeof(Game) != obj.GetType())
            {
                return false;
            }
            var otherGame = (Game)obj;
            for (int i = 0; i < Tiles.Count; i++)
            {
                var tile = Tiles[i];
                var otherTile = otherGame.Tiles[i];
                if (otherTile != tile)
                {
                    return false;
                }
            }
            
            for (int i = 0; i < amphipods.Count; i++)
            {
                var amphi = amphipods[i];
                var otherTile = otherGame.amphipods[i];
                if (otherTile != amphi)
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<Move> PossibleMoves
        {
            get
            {
                foreach (Amphipod amphipod in amphipods)
                {
                    var startTile = amphipod.Tile;
                    if (amphipod.Character == startTile.AlcoveCharacter)
                    {
                        var alcoves = Tiles.Where(t => t.AlcoveCharacter == startTile.AlcoveCharacter).OrderByDescending(t => t.Y).TakeWhile(t => t.OccupiedByRightAmphipod);
                        if(alcoves.Contains(startTile))
                        {
                            continue;
                        }
                    }
                    var routes = new List<List<Tile>>();
                    GenerateRoutes(startTile, new List<Tile>() { startTile }, routes);
                    foreach (var route in routes.Where(r => IsValidMove(amphipod, r.First(), r.Last())))
                    {
                        yield return new Move(route, amphipod, this);
                    }
                }
            }
        }

        private void GenerateRoutes(Tile startTile, List<Tile> currentRoute, List<List<Tile>> allRoutes)
        {
            var possibleTiles = startTile.Adjecent.Where(t => !t.Occupied && !currentRoute.Contains(t));
            foreach (var tile in possibleTiles)
            {
                var copy = currentRoute.ToList();
                copy.Add(tile);
                allRoutes.Add(copy);
                GenerateRoutes(tile, copy, allRoutes);
            }
        }

        private bool IsValidMove(Amphipod amphipod, Tile startTile, Tile endTile)
        {
            var isNotAlcoveBlockingTile = !endTile.IsHallway || !endTile.Adjecent.Any(t => t.Y == endTile.Y + 1 && t.X == endTile.X && t.IsAlcove);
            var isInOrOutOfHallway = startTile.IsAlcove && endTile.IsHallway || endTile.IsAlcove;
            var allAmphipodsInTargetAlcove = amphipods.Where(a => a.Tile.IsAlcove && a.Tile.AlcoveCharacter == endTile.AlcoveCharacter);
            var isIntoCorrectAlcoveWithoutStrangers = endTile.IsHallway || (amphipod.Character == endTile.AlcoveCharacter.Value && !allAmphipodsInTargetAlcove.Any(a => a.Character != endTile.AlcoveCharacter.Value));
            var isNonZeroMove = startTile != endTile;
            var isNotLeavingBottomOfAlcoveEmpty = endTile.IsHallway || !Tiles.Any(t => t.Y == endTile.Y + 1 && t.X == endTile.X && !t.Occupied);

            return isNotAlcoveBlockingTile && isInOrOutOfHallway && isIntoCorrectAlcoveWithoutStrangers && isNonZeroMove && isNotLeavingBottomOfAlcoveEmpty;
        }

        internal void DoMove(Move move)
        {
            move.Amphipod.MoveTo(this, move.EndTile);
        }

        internal void UndoMove(Move move)
        {
            move.Amphipod.MoveTo(this, move.StartTile);
        }

        internal void Print()
        {
            Console.WriteLine();
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    var tile = Tiles.FirstOrDefault(t => t.X == x && t.Y == y);
                    if (tile != null)
                    {
                        var printChar = tile.Occupied ? tile.Amphipod.Character : '.';
                        Console.Write(printChar);
                    }
                    else
                    {
                        Console.Write('#');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public class Move
    {
        public Move(List<Tile> route, Amphipod amphipod, Game game)
        {
            Route = route;
            StartTile = route[0];
            EndTile = route[^1];
            Amphipod = amphipod;
            _game = game;
        }

        public List<Tile> Route { get; }
        private readonly Game _game;

        public Tile StartTile { get; }
        public Tile EndTile { get; }
        public Amphipod Amphipod { get; }

        public int Cost
        {
            get
            {
                return (Route.Count - 1) * Amphipod.Cost;
            }
        }

        public bool PutsAmphipodInRightPlace
        {
            get
            {
                return EndTile.IsAlcove;
            }
        }

        public bool RemovesAmphipodFromWrongPlace
        {
            get
            {
                return Amphipod.Tile.IsAlcove && Amphipod.Tile.AlcoveCharacter.Value != Amphipod.Character;
            }
        }

        public bool EmptiesAlcove
        {
            get
            {
                return RemovesAmphipodFromWrongPlace && _game.Tiles.Where(t => t.IsAlcove && t.AlcoveCharacter == StartTile.AlcoveCharacter && t != StartTile).All(t => !t.Occupied || t.Amphipod.Character == t.AlcoveCharacter.Value);
            }
        }

        public int DistanceToRightBeforeAlcove
        {
            get
            {
                if (EndTile.IsHallway)
                {
                    return Math.Abs(_game.Tiles.First(t => t.IsAlcove && t.AlcoveCharacter == Amphipod.Character).X - 1 - EndTile.X);
                }
                else
                {
                    return -1;
                }
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var game = CreateGame(lines);
            var lowestCost = FindLowestCost(null, game);
            System.Console.WriteLine(lowestCost);
        }

        private static Game CreateGame(string[] lines)
        {
            var tiles = new List<Tile>();
            var amphipods = new List<Amphipod>();
            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                char alcoveChar = 'A';
                var chars = line.ToCharArray();
                for (int x = 0; x < chars.Length; x++)
                {
                    var character = chars[x];
                    if (character == '.')
                    {
                        tiles.Add(new Tile(x, y, null));
                    }
                    else if (Char.IsLetter(character))
                    {
                        var tile = new Tile(x, y, alcoveChar);
                        var amphipod = new Amphipod(x, y, character, tile);
                        tiles.Add(tile);
                        amphipods.Add(amphipod);
                        tile.Amphipod = amphipod;
                        alcoveChar++;
                    }
                }
            }

            foreach (var tile in tiles)
            {
                tile.Adjecent = GetAdjecentTiles(tiles, tile);
            }
            return new Game(tiles, amphipods);
        }

        private static IEnumerable<Tile> GetAdjecentTiles(List<Tile> tiles, Tile tile)
        {
            return new Tile[]
            {
                tiles.FirstOrDefault(t => t.X == tile.X - 1 && t.Y == tile.Y),
                tiles.FirstOrDefault(t => t.X == tile.X + 1 && t.Y == tile.Y),
                tiles.FirstOrDefault(t => t.X == tile.X && t.Y == tile.Y + 1),
                tiles.FirstOrDefault(t => t.X == tile.X && t.Y == tile.Y - 1),
            }
            .Where(t => t != null);
        }

        private static int FindLowestCost(Move currentMove, Game game, int currentCost = 0, int lowestCostFound = int.MaxValue)
        {
            int currentTotalCost = currentCost + (currentMove?.Cost).GetValueOrDefault();
            if (game.IsDone)
            {
                System.Console.WriteLine("Solved for " + currentTotalCost);
                return currentTotalCost;
            }

            var moves = OrderMoves(game.PossibleMoves.ToArray());

            int localLowestCostFound = lowestCostFound;
            foreach (var move in moves)
            {
                game.DoMove(move);
                int newCost = currentTotalCost + move.Cost;
                if (Game.ZobristTable.ContainsKey(game))
                {
                    var cost = Game.ZobristTable[game];
                    localLowestCostFound = Math.Min(cost, newCost);
                }
                else
                {
                    if (newCost < localLowestCostFound)
                    {
                        var outcome = FindLowestCost(move, game, currentTotalCost, localLowestCostFound);
                        Game.ZobristTable[game] = outcome;
                        localLowestCostFound = outcome;
                    }
                }
                game.UndoMove(move);
            }
            return localLowestCostFound;
        }

        private static Move[] OrderMoves(Move[] moves)
        {
            return moves.OrderByDescending(m => m.PutsAmphipodInRightPlace).ThenBy(m => m.Cost).ThenBy(m => m.EmptiesAlcove).ThenBy(m => m.DistanceToRightBeforeAlcove).ToArray();
        }
    }
}