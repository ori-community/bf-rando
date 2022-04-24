using System;
using System.Linq;

namespace Randomiser
{
    public class BitCollection : ISerializable
    {
        private readonly int[] array;

        public int Count { get; }

        private int ArrayCount => Count / 32 + 1;

        public BitCollection(int length)
        {
            Count = length;
            array = new int[ArrayCount];
        }

        public void Clear()
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = 0;
        }

        public void Serialize(Archive ar)
        {
            for (int i = 0; i < array.Length; i++)
                ar.Serialize(ref array[i]);
        }

        public bool this[int index]
        {
            get
            {
                if (index > Count - 1)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot exceed {Count - 1}");

                return (array[index / 32] & (1 << (index % 32))) != 0;
            }
            set
            {
                if (index > Count - 1)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot exceed {Count - 1}");

                if (value)
                    array[index / 32] |= 1 << (index % 32);
                else
                    array[index / 32] &= ~(1 << (index % 32));
            }
        }

        public override string ToString() => string.Join(" ", array.Select(a => Convert.ToString(a)).ToArray());

        public int Sum
        {
            get
            {
                int c = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < sizeof(int) * 8; j++)
                        c += (array[i] & (1 << j)) > 0 ? 1 : 0;
                }
                return c;
            }
        }
    }
}
