using System;

namespace Capstone
{
    public class Vector<T> where T : struct,
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        private T[] array;
        public Vector(T[] nums)
        {
            this.array = new T[2];
            array[0] = nums[0];
            array[1] = nums[1];
        }
        public T this[int i]
        {
            get { return array[i]; }
        }
        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
        {
            dynamic a0 = a[0];
            dynamic a1 = a[1];
            dynamic b0 = b[0];
            dynamic b1 = b[1];
            return new Vector<T>(new T[] { a0 + b0, a1 + b1 });
        }
    }
}
