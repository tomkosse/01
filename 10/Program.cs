using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _10
{
    class Program
    {
        private static List<char> _openingCharacters = new List<char>() { '(', '[', '{', '<' };
        private static List<char> _closingCharacters = new List<char>()  { ')', ']', '}', '>' };

        private static char GetClosingCharacterByOpeningCharacter(char openingCharacter)
        {
            return _closingCharacters[_openingCharacters.IndexOf(openingCharacter)];
        }

        private static char GetOpeningCharacterByClosingCharacter(char closingCharacter)
        {
            return _openingCharacters[_closingCharacters.IndexOf(closingCharacter)];
        }

        public static int DetermineScoreForCorruption(char closingCharacter)
        {
            switch (closingCharacter)
            {
                case ')': return 3; case ']': return 57; case '}': return 1197; case '>': return 25137;
                default:
                    throw new ArgumentOutOfRangeException(closingCharacter.ToString());
            }
        }
        public static int DetermineScoreForCompletion(char closingCharacter)
        {
            switch (closingCharacter)
            {
                case ')': return 1; case ']': return 2; case '}': return 3; case '>': return 4;
                default:
                    throw new ArgumentOutOfRangeException(closingCharacter.ToString());
            }
        }

        public static int CheckForCorruption(string line)
        {
            var stack = new Stack<char>();
            foreach(var character in line.ToCharArray())
            {
                if(_openingCharacters.Any(c => c == character))
                {
                    stack.Push(character);
                }
                else
                {
                    var correspondingOpeningCharacter = stack.Pop();
                    if(GetOpeningCharacterByClosingCharacter(character) != correspondingOpeningCharacter)
                    {
                        return DetermineScoreForCorruption(character);
                    }
                }
            }
            return 0;
        }
        

        private static long Complete(string line)
        {
            var stack = new Stack<char>();
            foreach(var character in line.ToCharArray())
            {
                if(_openingCharacters.Any(c => c == character))
                {
                    stack.Push(character);
                }
                else
                {
                    stack.Pop();
                }
            }
            long score = 0;

            foreach(var c in stack.Select(GetClosingCharacterByOpeningCharacter))
            {
                score = (score * 5) + DetermineScoreForCompletion(c);
            }
            return score;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);
            int corruptionScore = input.Sum(CheckForCorruption);
            System.Console.WriteLine("Corruption score: " + corruptionScore);
            
            var correctLines = input.Where(line => CheckForCorruption(line) == 0);
            var completionScores = correctLines.Select(Complete);

            long completionScore = completionScores
                                    .OrderBy(i => i)
                                    .Skip(completionScores.Count() / 2)
                                    .First();
            System.Console.WriteLine("Completion score: " + completionScore);
        }
    }
}
