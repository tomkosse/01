using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _01
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("missing first argument (file to process)");
            }

            var numbers = File
                    .ReadAllLines(args[0])
                    .Select(str => int.Parse(str));

            INumberSelectionStrategy strategy = new SlidingWindowNumberSelectionStrategy();
            //INumberSelectionStrategy strategy = new AllNumberSelectionStrategy();

            var selectedNumbers = strategy.SelectNumbers(numbers);

            var outcome = selectedNumbers
                .Skip(1)
                .Zip(selectedNumbers, (current, previous) => current > previous ? 1 : 0)
                .Sum();

            System.Console.WriteLine(outcome);
        }
    }
}
