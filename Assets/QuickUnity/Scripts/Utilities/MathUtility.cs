﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2016 Jerry Lee
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using System;
using System.Security.Cryptography;
using UnityEngine;

namespace QuickUnity.Utilities
{
    /// <summary>
    /// Utility class for common mathematic functions. This class cannot be inherited.
    /// </summary>
    public sealed class MathUtility
    {
        /// <summary>
        /// Gets the angle between objet A and B.
        /// </summary>
        /// <param name="a">The position of A.</param>
        /// <param name="b">The position of B.</param>
        /// <returns>The angle between vector a and b.</returns>
        public static float GetAngle(Vector3 a, Vector3 b)
        {
            return Mathf.Acos(Vector3.Dot(a.normalized, b.normalized)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Gets the random seed.
        /// </summary>
        /// <returns>The random seed.</returns>
        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Gets the reciprocal of a number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The reciprocal of the number.</returns>
        public static float GetReciprocal(float number)
        {
            return 1.0f / number;
        }

        /// <summary>
        /// Generate Gaussian Random Number.
        /// </summary>
        /// <returns>The Gaussian Random Number.</returns>
        public static float GenGaussianRandomNumber()
        {
            float x1 = UnityEngine.Random.value;
            float x2 = UnityEngine.Random.value;

            if (x1 == 0.0f)
                x1 = 0.01f;

            return (float)(Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Mathf.PI * x2));
        }

        /// <summary>
        /// Determines whether the specified n is odd.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns><c>true</c> if the specified n is odd; otherwise, <c>false</c>.</returns>
        public static bool IsOdd(int n)
        {
            return System.Convert.ToBoolean(n & 1);
        }

        /// <summary>
        /// Determines whether the specified n is odd.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns><c>true</c> if the specified n is odd; otherwise, <c>false</c>.</returns>
        public static bool IsOdd(float n)
        {
            return IsOdd(Mathf.FloorToInt(n));
        }

        /// <summary>
        /// Clamps the angle between -180 and 180.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="min">The minimum angle.</param>
        /// <param name="max">The maximum angle.</param>
        /// <returns>The angle between -180 and 180.</returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);

            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            min = NormalizeAngle(min);

            if (min > 180)
            {
                min -= 360;
            }
            else if (min < -180)
            {
                min += 360;
            }

            max = NormalizeAngle(max);

            if (max > 180)
            {
                max -= 360;
            }
            else if (max < -180)
            {
                max += 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// Normalizes the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The normalized angle value.</returns>
        public static float NormalizeAngle(float angle)
        {
            while (angle > 360)
            {
                angle -= 360;
            }

            while (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }
    }
}