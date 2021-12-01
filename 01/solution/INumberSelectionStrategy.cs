using System.Collections.Generic;

namespace _01
{
    public interface INumberSelectionStrategy
    {
         IEnumerable<int> SelectNumbers(IEnumerable<int> input);
    }
}