using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MathNet.Numerics.LinearAlgebra.Double;
using mm_lib_cs;
using mm_lib_vb;

namespace mmbench1
{
    [ShortRunJob]
    public class MMBenchmarks
    {
        private double[,] result = Helper.GenerateZeroMatrix();
        private double[,] a = Helper.GenerateTestMatrix();
        private double[,] b = Helper.GenerateTestMatrix();

        private double[][] resultjag = Helper.ToJagged(Helper.GenerateZeroMatrix());
        private double[][] ajag = Helper.ToJagged(Helper.GenerateTestMatrix());
        private double[][] bjag = Helper.ToJagged(Helper.GenerateTestMatrix());

        private double[] a1d = Helper.To1d(Helper.GenerateTestMatrix());
        private double[] bt1d = Helper.To1d(Helper.TransposeMatrix(Helper.GenerateTestMatrix()));

        private DenseMatrix amat = DenseMatrix.OfArray(Helper.GenerateTestMatrix());
        private DenseMatrix bmat = DenseMatrix.OfArray(Helper.GenerateTestMatrix());

        [Benchmark]
        public double[,] Original()
        {
            var newResult = VBMM.MatrixMulti_Original(a, b);
            return newResult;
        }

        [Benchmark(Baseline = true)]
        public double[,] Baseline()
        {
            VBMM.MatrixMulti_Baseline(result, a, b);
            return result;
        }

        [Benchmark]
        public double[][] Jagged()
        {
            VBMM.MatrixMulti_Jagged(resultjag, ajag, bjag);
            return resultjag;
        }

        [Benchmark]
        public double[,] Unrolled()
        {
            VBMM.MatrixMulti_Unrolled(result, a, b);
            return result;
        }

        [Benchmark]
        public double[][] Jagged_Unrolled()
        {
            VBMM.MatrixMulti_JaggedUnrolled(resultjag, ajag, bjag);
            return resultjag;
        }

        [Benchmark]
        public double[,] Unsafe()
        {
            CSMM.MatrixMulti_Unsafe(result, a, b);
            return result;
        }

        [Benchmark]
        public double[,] Unsafe_Unrolled()
        {
            CSMM.MatrixMulti_Unsafe_Unrolled(result, a, b);
            return result;
        }

        [Benchmark]
        public double[,] Simd()
        {
            CSMM.MatrixMulti_Simd(result, a, b);
            return result;
        }

        [Benchmark]
        public double[,] SimdFmt()
        {
            CSMM.MatrixMulti_SimdFmt(result, a1d, bt1d, result.GetUpperBound(1) + 1);
            return result;
        }

        [GlobalSetup(Target = nameof(Numerics))]
        public void Numerics_Setup()
        {
            CSMM.UseManagedProvider();
        }

        [Benchmark]
        public double[,] Numerics()
        {
            CSMM.MatrixMulti_Numerics(result, a, b);
            return result;
        }

        [GlobalSetup(Target = nameof(NumericsIntel))]
        public void NumericsIntel_Setup()
        {
            CSMM.UseIntelProvider();
        }

        [Benchmark]
        public double[,] NumericsIntel()
        {
            CSMM.MatrixMulti_Numerics(result, a, b);
            return result;
        }

        [GlobalSetup(Target = nameof(NumericsIntelFmt))]
        public void NumericsIntelFmt_Setup()
        {
            CSMM.UseIntelProvider();
        }

        [Benchmark]
        public DenseMatrix NumericsIntelFmt()
        {
            return amat * bmat;
        }

        [Benchmark]
        public double[,] OpenCL()
        {
            CSMM.MatrixMulti_OpenCL(result, a, b);
            return result;
        }

        [Benchmark]
        public double[,] Managed()
        {
            MmLibCpp.CppMm.Managed_Mat12_Mat12(result, a, b);
            return result;
        }

        [Benchmark]
        public double[,] Native()
        {
            MmLibCpp.CppMm.NativeWrapper_Mat12_Mat12(result, a, b);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MMBenchmarks>();
        }
    }
}
