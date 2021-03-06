﻿using System;

namespace RoboticNavigation.VectorMath
{
    public class Vector2d<T> where T : struct,
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
    {
        private T[] array;
        public T x { get { return array[0]; } }
        public T y { get { return array[1]; } }
        public Vector2d(T[] nums)
        {
            this.array = new T[2];
            array[0] = nums[0];
            array[1] = nums[1];
        }
        public T this[int i]
        {
            get { return array[i]; }
        }
        public static Vector2d<T> operator +(Vector2d<T> a, Vector2d<T> b)
        {
            dynamic a0 = a[0];
            dynamic a1 = a[1];
            dynamic b0 = b[0];
            dynamic b1 = b[1];
            return new Vector2d<T>(new T[] { a0 + b0, a1 + b1 });
        }
        public Vector2d<T> Copy()
        {
            return new Vector2d<T>(new T[] { array[0], array[1] });
        }
        public static Vector2d<T> operator -(Vector2d<T> a, Vector2d<T> b)
        {
            dynamic a0 = a[0];
            dynamic a1 = a[1];
            dynamic b0 = b[0];
            dynamic b1 = b[1];
            return new Vector2d<T>(new T[] { a0 - b0, a1 - b1 });
        }
        public static Vector2d<T> operator /(Vector2d<T> a, T b)
        {
            dynamic a0 = a[0];
            dynamic a1 = a[1];
            dynamic b0 = b;
            return new Vector2d<T>(new T[] { a0 / b0, a1 / b0 });
        }
        public static Vector2d<T> operator *(Vector2d<T> a, T b)
        {
            dynamic a0 = a[0];
            dynamic a1 = a[1];
            dynamic b0 = b;
            return new Vector2d<T>(new T[] { a0 * b0, a1 * b0 });
        }
        public override string ToString()
        {
            return $"({array[0]},{array[1]})";
        }
        public override bool Equals(object obj)
        {
            if (obj is Vector2d<T>)
            {
                var that = obj as Vector2d<T>;
                return this.x.Equals(that.x) && this.y.Equals(that.y);
            }
            return false;
        }
        public override int GetHashCode()
        {
            var hashCode = 352033288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public double Magnitude()
        {
            dynamic x1 = x;
            dynamic y1 = y;
            return Math.Sqrt((x1 * x1) + (y1 * y1));
        }
        public Vector2d<T> Unit()
        {
            dynamic X = this.x;
            dynamic Y = this.y;
            return new Vector2d<T>(new T[] { X / Magnitude(), Y / Magnitude() });
        }
        public double Angle()
        {
            dynamic ax = x;
            dynamic ay = y;
            return Math.Atan2(ay, ax) - (System.Math.PI / 2);
        }
        //public Vector2d<T> Unit()
        //{
        //    return this / this.Magnitude()
        //}
        public double Dot(Vector2d<T> that)
        {
            dynamic ax = x;
            dynamic ay = y;
            dynamic bx = that.x;
            dynamic by = that.y;
            return (ax * bx) + (ay * by);
        }
    }
}
