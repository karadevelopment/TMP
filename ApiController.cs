using SVN.Core.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SVN.Controllers
{
    public static class Extensions
    {
        private static IEnumerable<T> RandomizeAsync<T>(this IEnumerable<T> param, int index1, int index2)
        {
            for (var index = default(int); index <= index1 - 1; index++)
            {
                yield return param.ElementAt(index);
            }
            foreach (var value in param.ToList().GetRange(index1, index2 - index1 + 1).OrderBy(x => Guid.NewGuid()))
            {
                yield return value;
            }
            for (var index = index2 + 1; index <= param.Count() - 1; index++)
            {
                yield return param.ElementAt(index);
            }
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> param)
        {
            return param.RandomizeAsync(-1, -1).ToList();
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> param, int index1, int index2)
        {
            index2 = Math.Max(index1, index2);
            return param.RandomizeAsync(index1, index2).ToList();
        }
    }
    public class ApiController : BaseController
    {
        private IEnumerable<int> GetNumbersAsync(int length, int min, int max)
        {
            var target = Math2.Random.Range(1, Math.Abs(max));

            var result = new List<int>
            {
                Math2.Random.Range(1, Math.Abs(max)),
            };
            for (var i = 2; i <= length; i++)
            {
                var targetLeft = target - result.Sum();
                var lengthLeft = length - i + 1;
                var value = targetLeft / lengthLeft;
                result.Add(value);
            }
            for (var i = 1; i <= max - min; i++)
            {
                var item1Index = Math2.Random.Range(1, result.Count - 1);
                var item2Index = Math2.Random.Range(1, result.Count - 1);
                var item1 = result[item1Index];
                var item2 = result[item2Index];

                if (item1Index == item2Index)
                {
                    continue;
                }

                var item1Max = max - item1;
                var item2Max = item2 - min;
                var valueMax = Math.Min(item1Max, item2Max);
                var value = Math2.Random.Range(default(int), valueMax);

                result[item1Index] = item1 + value;
                result[item2Index] = item2 - value;
            }
            foreach (var value in result.Randomize(1, result.Count - 1))
            {
                yield return value;
            }
        }

        private IEnumerable<string> GetEquationsAsync(int amount, int length, int min, int max)
        {
            yield return "<style>";
            yield return "td { min-width: 3rem; text-align: right; }";
            yield return "</style>";
            yield return "<table>";

            for (var y = 1; y <= amount; y++)
            {
                var sum = default(int);
                var result = default(string);

                foreach (var number in this.GetNumbersAsync(length, min, max))
                {
                    sum += number;

                    if (result is null)
                    {
                        result = $"<td>{number}</td>";
                    }
                    else if (number == default(int))
                    {
                        result += $"<td></td>";
                    }
                    else
                    {
                        result += $"<td>{(default(int) <= number ? $"+{number}" : $"{number}")}</td>";
                    }
                }
                result += $"<td>=</td>";
                result += $"<td>{sum}</td>";

                yield return $"<tr>{result}</tr>";
            }

            yield return "</table>";
        }

        public string RandomEquation(int amount, int length, int min, int max)
        {
            length = Math.Max(length, 2);
            max = Math.Max(min, max);
            return this.GetEquationsAsync(amount, length, min, max).Join(string.Empty);
        }
    }
}