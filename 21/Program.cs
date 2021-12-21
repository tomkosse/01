using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _21
{
    class Program
    {
        private static long[] tally = new long[2];
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();
            var players = lines.Select(l => (int)char.GetNumericValue(l[28])).Select(i => i - 1).ToArray();
            
            var (scores, turn) = SimulateGameWithDeterministicDice(players.ToArray());
            int part1 = Math.Min(scores[0], scores[1]) * turn;

            GenerateDiceOutcomesWithDiracDice(players[0], players[1], 0, 0, 1, true);
            var part2 = Math.Max(tally[0], tally[1]);

            sw.Stop();
            System.Console.WriteLine("Part 1: " + part1);
            System.Console.WriteLine("Part 2: " + part2);
            System.Console.WriteLine("Done in " + sw.ElapsedMilliseconds + "ms");
        }

        private static void GenerateDiceOutcomesWithDiracDice(int positionP1, int positionP2, int scoreP1, int scoreP2, long universeCounter, bool isPlayerOnesTurn)
        {
            if(isPlayerOnesTurn == false && scoreP1 >= 21)
            {
                tally[0] += universeCounter;
                return;
            }
            else if(isPlayerOnesTurn == true && scoreP2 >= 21)
            {
                tally[1] += universeCounter;
                return;
            }

            for(int i=3; i <= 9; i++)
            {
                var universes = Universes[i - 3];
                if(isPlayerOnesTurn)
                {
                    var loc = (positionP1 + i) % 10;
                    GenerateDiceOutcomesWithDiracDice(loc, positionP2, scoreP1 + loc + 1, scoreP2, universeCounter * universes, false);
                }
                else
                {
                    var loc = (positionP2 + i) % 10;
                    GenerateDiceOutcomesWithDiracDice(positionP1, loc, scoreP1, scoreP2 + loc + 1, universeCounter * universes, true);
                }
            }
        }

        static int[] Universes = new int[7] {1, 3, 6, 7, 6, 3, 1 }; // Normaal verdeelde uitkomst van de dobbelstenen

        private static (int[] scores, int turn) SimulateGameWithDeterministicDice(int[] players)
        {
            int maxScore = 1000;
            int[] diceOutcomes = Enumerable.Range(1, 100).ToArray();
            int[] scores = new int[players.Length];
            int whosTurn = 0;
            int turn = 0;
            while (!scores.Any(s => s >= maxScore))
            {
                var die1 = diceOutcomes[turn % 100];
                var die2 = diceOutcomes[(turn + 1) % 100];
                var die3 = diceOutcomes[(turn + 2) % 100];
                var loc = (players[whosTurn] + die1 + die2 + die3) % 10;
                scores[whosTurn] += loc + 1;
                players[whosTurn] = loc;
                whosTurn++;
                if (whosTurn == players.Length)
                {
                    whosTurn = 0;
                }
                turn += 3;
            }
            return (scores, turn);
        }        
    }
}