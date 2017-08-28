using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HATE
{

    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list, Action<T, T> swapAction, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            int n = list.Count;
            while (n-- > 1)
            {
                int k = random.Next(n + 1);
                swapAction(list[n], list[k]);
            }
        }

        public static byte[] Garble(this byte[] array, float chnc, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return array.Select(x => (char.IsLetterOrDigit((char)x) && random.NextDouble() < chnc)  ? (byte)(random.Next(75) + 47) : x).ToArray();         
        }
    }

}
