using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mm_lib_cs;

namespace mm_test
{
    [TestClass]
    public class SimdTests
    {
        [TestMethod]
        public void TestSimd()
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
            var b = new double[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 2, 3, 4 },
                { 0, 5, 6, 7, 8 },
                { 0, 9, 10, 11, 12 },
                { 0, 13, 14, 15, 16 },
            };
            double[,] result = new double[a.GetUpperBound(0) + 1, a.GetUpperBound(1) + 1];

            // Act
            CSMM.MatrixMulti_Simd(result, a, b);

            // Assert
            var expected = new double[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 90, 100, 110, 120 },
                { 0, 202, 228, 254, 280 },
                { 0, 314, 356, 398, 440 },
                { 0, 426, 484, 542, 600 },
            };
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
