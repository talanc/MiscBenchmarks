using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mm_test
{
    [TestClass]
    public class OpenCLTests
    {
        [TestMethod]
        public void TestOpenCLIntegration()
        {
            // Arrange
            var a = new double[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };
            var b = new double[,]
            {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            };
            var result = new double[a.GetUpperBound(0) + 1, a.GetUpperBound(1) + 1];

            // Act
            mm_lib_cs.CSMM.MatrixMulti_OpenCL(result, a, b);

            // Assert
            var expected = new double[,]
            {
                { 30, 36, 42 },
                { 66, 81, 96 },
                { 102, 126, 150 }
            };
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
