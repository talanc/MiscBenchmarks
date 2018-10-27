using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mm_lib_cs
{
    public static partial class CSMM
    {
        public static unsafe void MatrixMulti_Simd(double[,] result, double[,] a, double[,] b)
        {
            var bt = Helper.TransposeMatrix(b);

            var ar = a.GetUpperBound(0) + 1;
            var ac = a.GetUpperBound(1) + 1;
            var br = bt.GetUpperBound(0) + 1;
            var bc = bt.GetUpperBound(1) + 1;

            var simdLength = Vector<double>.Count;

            var r0 = new double[simdLength];

            var a0 = new double[a.Length];
            var b0 = new double[bt.Length];
            fixed (double* asrc = a, adst = a0, bsrc = bt, bdst = b0)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    *(adst + i) = *(asrc + i);
                    *(bdst + i) = *(bsrc + i);
                }
            }

            for (var i = 1; i < ar; i++)
            {
                for (var j = 1; j < bc; j++)
                {
                    for (var k = 1; k < ac; k += simdLength)
                    {
                        var va = new Vector<double>(a0, i * ac + k);
                        var vb = new Vector<double>(b0, j * bc + k);
                        result[i, j] += Vector.Dot(va, vb);
                    }
                }
            }
        }

        public static unsafe void MatrixMulti_SimdFmt(double[,] result, double[] a, double[] bt, int ncols)
        {
            var ar = a.Length / ncols;
            var ac = ncols;
            var br = bt.Length / ncols;
            var bc = ncols;

            var simdLength = Vector<double>.Count;
            var r0 = new double[simdLength];

            for (var i = 1; i < ar; i++)
            {
                for (var j = 1; j < bc; j++)
                {
                    for (var k = 1; k < ac; k += simdLength)
                    {
                        var va = new Vector<double>(a, i * ac + k);
                        var vb = new Vector<double>(bt, j * bc + k);
                        result[i, j] += Vector.Dot(va, vb);
                    }
                }
            }
        }
    }
}
