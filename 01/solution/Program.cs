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
            var numbers = File
                    .ReadAllLines(args[0])
                    .Select(str => int.Parse(str));

            INumberSelectionStrategy strategy = new SlidingWindowNumberSelectionStrategy();
            //INumberSelectionStrategy strategy = new AllNumberSelectionStrategy();
                    
            var selectedNumbers = strategy.SelectNumbers(numbers);

            int? previousNumber = null;
            int larger = 0;
            foreach (int number in selectedNumbers)
            {
                if (previousNumber.HasValue)
                {
                    if (number > previousNumber)
                    {
                        larger++;
                    }
                }
                previousNumber = number;
            }
            System.Console.WriteLine(larger);
        }
    }
}
