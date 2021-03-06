﻿using System;
using System.Numerics;

namespace ComplexFilterQA
{
    public static class KernelHelper
    {
        public static int GetKernelSizeForGaussianSigma(double sigma)
        {
            return 2 * (int)Math.Ceiling(sigma * 3.0f) + 1;
        }

        public static Complex[,] MakeComplexKernel(Func<int, int, double> realFunction,
                                                   Func<int, int, double> imaginaryFunction, int size)
        {
            var realPart = MakeKernel(realFunction, size);
            var imPart = MakeKernel(imaginaryFunction, size);
            return MakeComplexFromDouble(realPart, imPart);
        }

        public static double Max2d(double[,] arr)
        {
            double max = double.NegativeInfinity;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] > max) max = arr[x, y];
                }
            }
            return max;
        }

        /// <summary>
        /// Make squared kernel.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double[,] MakeKernel(Func<int, int, double> function, int size)
        {
            double[,] kernel = new double[size, size];
            int center = size / 2;
            double sum = 0;
            for (int x = -center; x <= center; x++)
            {
                for (int y = -center; y <= center; y++)
                {
                    sum += kernel[center + x, center + y] = function(x, y);
                }
            }
            // normalization
            if (Math.Abs(sum) > 0.0000001)
                for (int x = -center; x <= center; x++)
                {
                    for (int y = -center; y <= center; y++)
                    {
                        kernel[center + x, center + y] /= sum;
                    }
                }
            return kernel;
        }

        /// <summary>
        /// Make rectangular kernel without normalization.
        /// even and odd
        /// </summary>
        /// <param name="function"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <returns></returns>
        public static double[,] MakeKernel(Func<float, float, double> function, int sizeX, int sizeY)
        {
            double[,] kernel = new double[sizeX, sizeY];
            int centerX = sizeX / 2;
            int centerY = sizeY / 2;
            float shiftX = (sizeX - 2 * centerX - 1) / 2f;
            float shiftY = (sizeY - 2 * centerY - 1)/2f;

            for (int x = -centerX; x <= centerX+2*shiftX; x++)
            {
                for (int y = -centerY; y <= centerY+2*shiftY; y++)
                {
                    kernel[centerX + x, centerY + y] = function(x-shiftX, y-shiftY);
                }
            }
            return kernel;
        }

        public static Complex[,] MakeComplexFromDouble(double[,] real, double[,] imaginary)
        {
            int maxX = real.GetLength(0);
            int maxY = real.GetLength(1);
            Complex[,] result = new Complex[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    result[x, y] = new Complex(real[x, y], imaginary[x, y]);
                }
            }
            return result;
        }

        public static double[,] Subtract(double[,] source, double[,] value)
        {
            var maxX = source.GetLength(0);
            var maxY = source.GetLength(1);
            var result = new double[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    result[x, y] = source[x, y] - value[x, y];
                }
            }
            return result;
        }

        public static double[,] Zip2D(double[,] arr1, double[,] arr2, Func<double, double, double> f)
        {
            var result = new double[arr1.GetLength(0), arr1.GetLength(1)];
            for (int x = 0; x < arr1.GetLength(0); x++)
            {
                for (int y = 0; y < arr1.GetLength(1); y++)
                {
                    result[x, y] = f(arr1[x, y], arr2[x, y]);
                }
            }
            return result;
        }

        public static V[,] Zip2D<T, U, V>(T[,] arr1, U[,] arr2, Func<T, U, V> f)
        {
            var result = new V[arr1.GetLength(0), arr1.GetLength(1)];
            for (int x = 0; x < arr1.GetLength(0); x++)
            {
                for (int y = 0; y < arr1.GetLength(1); y++)
                {
                    result[x, y] = f(arr1[x, y], arr2[x, y]);
                }
            }
            return result;
        }

        public static double[,] Add(double[,] source, double[,] value)
        {
            var maxX = source.GetLength(0);
            var maxY = source.GetLength(1);
            var result = new double[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    result[x, y] = source[x, y] + value[x, y];
                }
            }
            return result;
        }

        public static U[,] Select2D<T, U>(this T[,] array, Func<T, U> f)
        {
            var result = new U[array.GetLength(0), array.GetLength(1)];

            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    result[x, y] = f(array[x, y]);
                }
            }

            return result;
        }

        public static U[,] Select2D<T, U>(this T[,] array, Func<T, int, int, U> f)
        {
            var result = new U[array.GetLength(0), array.GetLength(1)];

            for (int row = 0; row < array.GetLength(0); row++)
            {
                for (int column = 0; column < array.GetLength(1); column++)
                {
                    result[row, column] = f(array[row, column], row, column);
                }
            }

            return result;
        }
    }
}
