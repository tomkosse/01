using System.Collections.Generic;
using System.Linq;

namespace _01
{
    class SlidingWindowNumberSelectionStrategy : INumberSelectionStrategy
    {
        public IEnumerable<int> SelectNumbers(IEnumerable<int> input)
        {
            var numbers = input.ToArray();
            for (int i = 0; i < numbers.Length - 2; i++)
            {
                var windowTotal = numbers[i] + numbers[i + 1] + numbers[i + 2];
                yield return windowTotal;
            }
        }
    }
}