using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mm_lib_vb;
using mm_lib_cs;

namespace mm_test
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void TestTranspose()
        {
            // Arrange
            var a = new double[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 2, 3, 4 },
                { 0, 5, 6, 7, 8 },
                { 0, 9, 10, 11, 12 },
                { 0, 13, 14, 15, 16 },
            };
            var expected = new double[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 5, 9, 13 },
                { 0, 2, 6, 10, 14 },
                { 0, 3, 7, 11, 15 },
                { 0, 4, 8, 12, 16 },
            };

            // Act
            var actual = Helper.TransposeMatrix(a);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNoAlloc()
        {
            AssertMatrix((result, a, b) =>
            {
                VBMM.MatrixMulti_Baseline(result, a, b);
            });
        }

        [TestMethod]
        public void TestJagged()
        {
            AssertMatrixJagged((result, a, b) =>
            {
                VBMM.MatrixMulti_Jagged(result, a, b);
            });
        }

        [TestMethod]
        public void TestJaggedUnrolled()
        {
            AssertMatrixJagged((result, a, b) =>
            {
                VBMM.MatrixMulti_JaggedUnrolled(result, a, b);
            });
        }

        [TestMethod]
        public void TestUnrolled()
        {
            AssertMatrix((result, a, b) =>
            {
                VBMM.MatrixMulti_Unrolled(result, a, b);
            });
        }

        [TestMethod]
        public void TestUnsafe()
        {
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_Unsafe(result, a, b));
        }

        [TestMethod]
        public void TestUnsafeUnrolled()
        {
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_Unsafe_Unrolled(result, a, b));
        }

        [TestMethod]
        public void TestOpenCL()
        {
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_OpenCL(result, a, b));
        }

        [TestMethod]
        public void TestNumerics()
        {
            CSMM.UseManagedProvider();
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_Numerics(result, a, b));
        }

        [TestMethod]
        public void TestNumericsIntel()
        {
            CSMM.UseIntelProvider();
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_Numerics(result, a, b));
        }

        [TestMethod]
        public void TestSimd()
        {
            AssertMatrix((result, a, b) => CSMM.MatrixMulti_Simd(result, a, b));
        }

        [TestMethod]
        public void TestSimdFmt()
        {
            AssertMatrix((result, a, b) =>
            {
                var a0 = Helper.To1d(a);
                var b0 = Helper.To1d(Helper.TransposeMatrix(b));
                CSMM.MatrixMulti_SimdFmt(result, a0, b0, a.GetUpperBound(1) + 1);
            });
        }

        [TestMethod]
        public void TestCppManaged()
        {
            Assert.AreEqual(1, MmLibCpp.CppMm.Something());

            AssertMatrix((result, a, b) =>
            {
                MmLibCpp.CppMm.Managed_Mat12_Mat12(result, a, b);
            });
        }

        [TestMethod]
        public void TestCppNative()
        {
            Assert.AreEqual(1, MmLibCpp.CppMm.Something());

            AssertMatrix((result, a, b) =>
            {
                MmLibCpp.CppMm.NativeWrapper_Mat12_Mat12(result, a, b);
            });
        }

        public void AssertMatrix(Action<double[,], double[,], double[,]> action)
        {
            // Arrange
            var result = Helper.GenerateZeroMatrix();
            var a = Helper.GenerateTestMatrix();
            var b = Helper.GenerateTestMatrix();

            // Act
            action(result, a, b);

            var expected = VBMM.MatrixMulti_Original(a, b);

            // Assert
            for (var i = 0; i <= result.GetUpperBound(0); i++)
            {
                for (var j = 0; j <= result.GetUpperBound(1); j++)
                {
                    Assert.AreEqual(expected[i, j], result[i, j], 0.00001, $"I={i}, J={j}, Expected={expected[i,j]}, Actual={result[i, j]}");
                }
            }
            //CollectionAssert.AreEqual(expected, result);
        }

        public void AssertMatrixJagged(Action<double[][], double[][], double[][]> action)
        {
            // Arrange
            var result = Helper.GenerateZeroMatrixJagged();
            var a = Helper.ToJagged(Helper.GenerateTestMatrix());
            var b = Helper.ToJagged(Helper.GenerateTestMatrix());

            // Act
            action(result, a, b);

            var newResult = Helper.To2d(result);

            // Assert
            CollectionAssert.AreEqual(newResult, VBMM.MatrixMulti_Original(Helper.To2d(a), Helper.To2d(b)));
        }
    }
}
