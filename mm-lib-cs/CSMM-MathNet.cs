using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace mm_lib_cs
{
    public static partial class CSMM
    {
        public static void UseManagedProvider() => Control.UseManaged();
        public static void UseIntelProvider() => Control.UseNativeMKL();

        public static void MatrixMulti_Numerics(double[,] result, double[,] a, double[,] b)
        {
            var am = DenseMatrix.OfArray(a);
            var bm = DenseMatrix.OfArray(b);
            var rm = am * bm;
            var arr = rm.AsArray();
            if (arr == null)
            {
                arr = rm.ToArray();
            }
            Array.Copy(arr, result, arr.Length);
        }
    }
}
