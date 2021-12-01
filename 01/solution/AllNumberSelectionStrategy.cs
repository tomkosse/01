using System.Collections.Generic;

namespace _01
{
    public class AllNumberSelectionStrategy : INumberSelectionStrategy
    {
        public IEnumerable<int> SelectNumbers(IEnumerable<int> input)
        {
            return input;
        }
    }
}