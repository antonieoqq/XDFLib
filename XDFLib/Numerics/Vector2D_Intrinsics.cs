using System;
using System.Runtime.CompilerServices;

namespace XDFLib.Numerics
{
    public partial struct Vector2D
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public double X;
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public double Y;

        #region Constructors
        /// <summary>
        /// Constructs a vector whose elements are all the double specified value.
        /// </summary>
        /// <param name="value">The element to fill the vector with.</param>

        public Vector2D(double value) : this(value, value) { }

        /// <summary>
        /// Constructs a vector with the given individual elements.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        #endregion Constructors

        #region Public Instance Methods
        /// <summary>
        /// Copies the contents of the vector into the given array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(double[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the contents of the vector into the given array, starting from the given index.
        /// </summary>
        /// <exception cref="ArgumentNullException">If array is null.</exception>
        /// <exception cref="RankException">If array is multidimensional.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If index is greater than end of the array or index is less than zero.</exception>
        /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination array
        /// or if there are not enough elements to copy.</exception>
        public void CopyTo(double[] array, int index)
        {
            if (array == null)
            {
                // Match the JIT's exception type here. For perf, a NullReference is thrown instead of an ArgumentNull.
                throw new NullReferenceException("Arg_NullArgumentNullRef");
            }
            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException($"index: {index} Arg_ArgumentOutOfRangeException");
            }
            if ((array.Length - index) < 2)
            {
                throw new ArgumentException($"index: {index} Arg_ElementsInSourceIsGreaterThanDestination");
            }
            array[index] = X;
            array[index + 1] = Y;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Vector2D is equal to this Vector2D instance.
        /// </summary>
        /// <param name="other">The Vector2D to compare this instance to.</param>
        /// <returns>True if the other Vector2D is equal to this instance; False otherwise.</returns>

        public bool Equals(Vector2D other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
        #endregion Public Instance Methods

        #region Public Static Methods
        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The dot product.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vector2D value1, Vector2D value2)
        {
            return value1.X * value2.X +
                   value1.Y * value2.Y;
        }

        /// <summary>
        /// Returns a vector whose elements are the minimum of each of the pairs of elements in the two source vectors.
        /// </summary>
        /// <param name="value1">The first source vector.</param>
        /// <param name="value2">The second source vector.</param>
        /// <returns>The minimized vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D Min(Vector2D value1, Vector2D value2)
        {
            return new Vector2D(
                (value1.X < value2.X) ? value1.X : value2.X,
                (value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Returns a vector whose elements are the maximum of each of the pairs of elements in the two source vectors
        /// </summary>
        /// <param name="value1">The first source vector</param>
        /// <param name="value2">The second source vector</param>
        /// <returns>The maximized vector</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D Max(Vector2D value1, Vector2D value2)
        {
            return new Vector2D(
                (value1.X > value2.X) ? value1.X : value2.X,
                (value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Returns a vector whose elements are the absolute values of each of the source vector's elements.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The absolute value vector.</returns>        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D Abs(Vector2D value)
        {
            return new Vector2D(Math.Abs(value.X), Math.Abs(value.Y));
        }

        /// <summary>
        /// Returns a vector whose elements are the square root of each of the source vector's elements.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The square root vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D SquareRoot(Vector2D value)
        {
            return new Vector2D(Math.Sqrt(value.X), Math.Sqrt(value.Y));
        }
        #endregion Public Static Methods

        #region Public Static Operators
        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The summed vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The difference vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The product vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator *(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The scalar value.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The scaled vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator *(double left, Vector2D right)
        {
            return new Vector2D(left, left) * right;
        }

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator *(Vector2D left, double right)
        {
            return left * new Vector2D(right, right);
        }

        /// <summary>
        /// Divides the first vector by the second.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The vector resulting from the division.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator /(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Divides the vector by the given scalar.
        /// </summary>
        /// <param name="value1">The source vector.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The result of the division.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator /(Vector2D value1, double value2)
        {
            double invDiv = 1.0f / value2;
            return new Vector2D(
                value1.X * invDiv,
                value1.Y * invDiv);
        }

        /// <summary>
        /// Negates a given vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The negated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator -(Vector2D value)
        {
            return Zero - value;
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are equal; False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2D left, Vector2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are not equal; False if they are equal.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2D left, Vector2D right)
        {
            return !(left == right);
        }
        #endregion Public Static Operators
    }
}
