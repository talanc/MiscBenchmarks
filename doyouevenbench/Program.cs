using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dim = System.Single;

namespace doyouevenbench
{
    [LegacyJitX86Job(), RyuJitX64Job()]
    public class VectorAddBenchmark
    {
        private Dim[] a = GenerateA();
        private Dim[] b = GenerateA();

        private Dim[] result = GenerateEmpty();

        //[Benchmark(Baseline = true)]
        //public Dim[] Naive()
        //{
        //    for (var i = 0; i < NumNum; i++)
        //    {
        //        result[i] = a[i] + b[i];
        //    }

        //    return result;
        //}

        //[Benchmark()]
        //public Dim[] Unrolled8()
        //{
        //    for (var i = 0; i < NumNum; i += UnrolledNum)
        //    {
        //        result[i] = a[i] + b[i];
        //        result[i + 1] = a[i + 1] + b[i + 1];
        //        result[i + 2] = a[i + 2] + b[i + 2];
        //        result[i + 3] = a[i + 3] + b[i + 3];
        //        result[i + 4] = a[i + 4] + b[i + 4];
        //        result[i + 5] = a[i + 5] + b[i + 5];
        //        result[i + 6] = a[i + 6] + b[i + 6];
        //        result[i + 7] = a[i + 7] + b[i + 7];
        //    }

        //    return result;
        //}

        //[Benchmark()]
        //public unsafe Dim[] Unsafe()
        //{
        //    fixed (Dim* r0 = result, a0 = a, b0 = b)
        //    {
        //        Dim* rp = r0, ap = a0, bp = b0;

        //        for (var i = 0; i < NumNum; i++)
        //        {
        //            *(rp + i) = *(ap + i) + *(bp + i);
        //        }
        //    }

        //    return result;
        //}

        //[Benchmark()]
        //public unsafe Dim[] UnsafeUnrolled8()
        //{
        //    fixed (Dim* r0 = result, a0 = a, b0 = b)
        //    {
        //        Dim* rp = r0, ap = a0, bp = b0;

        //        for (var i = 0; i < NumNum; i += UnrolledNum)
        //        {
        //            *(rp + i) = *(ap + i) + *(bp + i);
        //            *(rp + i + 1) = *(ap + i + 1) + *(bp + i + 1);
        //            *(rp + i + 2) = *(ap + i + 2) + *(bp + i + 2);
        //            *(rp + i + 3) = *(ap + i + 3) + *(bp + i + 3);
        //            *(rp + i + 4) = *(ap + i + 4) + *(bp + i + 4);
        //            *(rp + i + 5) = *(ap + i + 5) + *(bp + i + 5);
        //            *(rp + i + 6) = *(ap + i + 6) + *(bp + i + 6);
        //            *(rp + i + 7) = *(ap + i + 7) + *(bp + i + 7);
        //        }
        //    }

        //    return result;
        //}

        [Benchmark()]
        public Dim[] Simd()
        {
            if (!Vector.IsHardwareAccelerated)
                throw new InvalidOperationException("SIMD is not hardware accelerated");

            var simdLength = Vector<Dim>.Count;

            for (var i = 0; i < NumNum; i += simdLength)
            {
                var va = new Vector<Dim>(a, i);
                var vb = new Vector<Dim>(b, i);
                (va + vb).CopyTo(result, i);
            }

            return result;
        }

        const int UnrolledNum = 8;
        const int NumNum = 128;

        static Dim[] GenerateEmpty()
        {
            return new Dim[NumNum];
        }

        static Dim[] GenerateA()
        {
            var rnd = new Random();
            var a = new Dim[NumNum];
            for (var i = 0; i < NumNum; i++)
            {
                a[i] = (Dim)rnd.NextDouble() * 1000;
            }
            return a;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<VectorAddBenchmark>();
        }
    }
}
