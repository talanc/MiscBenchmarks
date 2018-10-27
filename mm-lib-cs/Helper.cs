using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_lib_cs
{
    public static class Helper
    {
        public static double[,] GenerateZeroMatrix()
        {
            return new double[13, 13];
        }

        public static double[,] GenerateTestMatrix()
        {
            var rand = new Random();
            var matrix = GenerateZeroMatrix();
            for (var r = 1; r <= 12; r++)
            {
                for (var c = 1; c <= 12; c++)
                {
                    matrix[r, c] = rand.NextDouble() * 1000;
                }
            }
            return matrix;
        }

        public static double[][] GenerateZeroMatrixJagged()
        {
            var matrix = new double[13][];
            for (var i = 0; i <= 12; i++)
            {
                matrix[i] = new double[13];
            }
            return matrix;
        }

        public static double[,] TransposeMatrix(double[,] a)
        {
            var b = new double[a.GetUpperBound(0) + 1, a.GetUpperBound(1) + 1];
            Array.Copy(a, b, a.Length);

            var rlen = a.GetUpperBound(0) + 1;

            for (var r = 1; r < rlen; r++)
            {
                for (var c = 1; c < r; c++)
                {
                    var tmp = b[r, c];
                    b[r, c] = b[c, r];
                    b[c, r] = tmp;
                }
            }

            return b;
        }

        public static double[][] ToJagged(double[,] matrix)
        {
            var jagged = GenerateZeroMatrixJagged();
            for (var r = 1; r <= 12; r++)
            {
                for (var c = 1; c <= 12; c++)
                {
                    jagged[r][c] = matrix[r, c];
                }
            }
            return jagged;
        }

        public static double[,] To2d(double[][] jagged)
        {
            var matrix = GenerateZeroMatrix();
            for (var r = 1; r <= 12; r++)
            {
                for (var c = 1; c <= 12; c++)
                {
                    matrix[r, c] = jagged[r][c];
                }
            }
            return matrix;
        }

        public static double[] To1d(double[,] a)
        {
            var b = new double[a.Length];
            var i = 0;
            for (var r = 0; r <= a.GetUpperBound(0); r++)
            {
                for (var c = 0; c <= a.GetUpperBound(1); c++)
                {
                    b[i] = a[r, c];
                    i++;
                }
            }
            return b;
        }

        public static double[,] To2d(double[] a, int ncols)
        {
            var b = new double[a.Length / ncols, ncols];
            var i = 0;
            for (var r = 0; r <= b.GetUpperBound(0); r++)
            {
                for (var c = 0; c <= b.GetUpperBound(1); c++)
                {
                    b[r, c] = a[i];
                    i++;
                }
            }
            return b;
        }
    }
}
