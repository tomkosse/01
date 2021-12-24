using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _24
{
    class Program
    {
        static void Main(string[] args)
        {
            var codeLines = File.ReadAllLines(args[0]);

            var quickMemory = new HashSet<long>();
            var dictionary = new Dictionary<long, long>();

            var remainder = new Queue<string>(codeLines);
            var actions = new List<(char, List<Action<Dictionary<char, long>>>)>();
            
            var ALU = new ALU();
            for (int i = 0; i < 14; i++)
            {
                var inputRegister = remainder.Dequeue()[4];
                List<string> instructionChunk = new List<string>();
                while (remainder.Any() && !remainder.Peek().StartsWith("inp"))
                {
                    instructionChunk.Add(remainder.Dequeue());
                }
                actions.Add((inputRegister, ALU.GenerateActions(instructionChunk)));
            }
            var largest = Calculate(actions, new Dictionary<long, long>()  { { 0, 0 } }, 0, true);
            var smallest = Calculate(actions, new Dictionary<long, long>()  { { 0, 0 } }, 0, false);
            
            System.Console.WriteLine("Part 1: " + largest);
            System.Console.WriteLine("Part 2: " + smallest);
        }

        private static long Calculate(List<(char inputRegister, List<Action<Dictionary<char, long>>> statements)> actions, Dictionary<long, long> outcomes, int numberIndex, bool largest)
        {
            if (numberIndex == 14)
            {
                return outcomes[0];
            }

            var newOutcomes = new Dictionary<long, long>();
            var numbers = outcomes.Keys.ToList();
            Console.WriteLine($"{numberIndex} {numbers.Count}");
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    var registers = new Dictionary<char, long>() { { 'w', 0 }, { 'x', 0 }, { 'y', 0 }, { 'z', numbers[i]} };
                    registers[actions[numberIndex].inputRegister] = j;
                    foreach(var action in actions[numberIndex].statements)
                    {
                        action.Invoke(registers);
                    }
                    var foundZ = registers['z'];
                    long newNumber = outcomes[numbers[i]] * 10 + j;

                    if (newOutcomes.ContainsKey(foundZ))
                    {
                        if(largest)
                        {
                            if (newOutcomes[foundZ] > newNumber)
                            {
                                newOutcomes[foundZ] = newNumber;
                            }
                        }
                        else
                        {
                            if (newOutcomes[foundZ] < newNumber)
                            {
                                newOutcomes[foundZ] = newNumber;
                            }
                        }
                    }
                    else
                    {
                        newOutcomes.Add(foundZ, newNumber);
                    }
                }
            }

            return Calculate(actions, newOutcomes, numberIndex + 1, largest);
        }
    }

    public class ALU
    {
        public static List<Action<Dictionary<char, long>>> GenerateActions(List<string> codeLines)
        {
            var actions = new List<Action<Dictionary<char, long>>>();
            foreach (var line in codeLines)
            {
                char argumentOne = line[4];
                string argumentTwo = line.Substring(Math.Min(line.Length, 6));
                System.Linq.Expressions.Expression<Action<Dictionary<char, long>>> expr;
                switch (line[1])
                {
                    case 'd': if (int.TryParse(argumentTwo, out int vala)) { expr = (r) => Add(r, argumentOne, vala); } else { expr = (r) => Add(r, argumentOne, line[6]); } break;
                    case 'u': if (int.TryParse(argumentTwo, out int valb)) { expr = (r) => Mul(r, argumentOne, valb); } else { expr = (r) => Mul(r, argumentOne, line[6]); } break;
                    case 'i': if (int.TryParse(argumentTwo, out int valc)) { expr = (r) => Div(r, argumentOne, valc); } else { expr = (r) => Div(r, argumentOne, line[6]); } break;
                    case 'o': if (int.TryParse(argumentTwo, out int vald)) { expr = (r) => Mod(r, argumentOne, vald); } else { expr = (r) => Mod(r, argumentOne, line[6]); } break;
                    case 'q': if (int.TryParse(argumentTwo, out int vale)) { expr = (r) => Eql(r, argumentOne, vale); } else { expr = (r) => Eql(r, argumentOne, line[6]); } break;
                    default: throw new ArgumentException(line);
                };
                actions.Add(expr.Compile());
            }
            return actions;
        }

        public static void Add(Dictionary<char, long> registers, char registerA, char registerB) => registers[registerA] += registers[registerB];
        public static void Add(Dictionary<char, long> registers, char registerA, int value) => registers[registerA] += value;
        public static void Mul(Dictionary<char, long> registers, char registerA, char registerB) => registers[registerA] *= registers[registerB];
        public static void Mul(Dictionary<char, long> registers, char registerA, int value) => registers[registerA] *= value;
        public static void Div(Dictionary<char, long> registers, char registerA, char registerB) => registers[registerA] /= registers[registerB];
        public static void Div(Dictionary<char, long> registers, char registerA, int value) => registers[registerA] /= value;
        public static void Mod(Dictionary<char, long> registers, char registerA, char registerB) => registers[registerA] %= registers[registerB];
        public static void Mod(Dictionary<char, long> registers, char registerA, int value) => registers[registerA] %= value;
        public static void Eql(Dictionary<char, long> registers, char registerA, char registerB) => registers[registerA] = registers[registerA] == registers[registerB] ? 1 : 0;
        public static void Eql(Dictionary<char, long> registers, char registerA, int value) => registers[registerA] = registers[registerA] == value ? 1 : 0;
    }
}
