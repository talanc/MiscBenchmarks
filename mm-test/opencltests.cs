using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cloo;

namespace mm_test
{
    [TestClass]
    public class OpenCLTests
    {
        static readonly string MatrixMultiSource = @"
kernel void MatrixMulti(global read_only double* src1, global read_only double* src2, int ncols, global write_only double* dst)
{
    int i = get_global_id(0);
    int r = i / ncols * ncols;
    int c = i % ncols;
    double sum = 0.0;

    for (int j = 0; j < ncols; j++)
    {
        sum += src1[j + r] * src2[j * ncols + c];
    }

    dst[i] = sum;
}";

        [TestMethod]
        public void TestOpenCL()
        {
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);

            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, cpl, null, IntPtr.Zero);

            int ncols = 3;
            double[] src1 = new double[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 9,
            };
            double[] src2 = new double[]
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 9,
            };
            double[] dst = new double[src1.Length];

            ComputeProgram program = new ComputeProgram(context, MatrixMultiSource);
            program.Build(null, null, null, IntPtr.Zero);

            ComputeKernel kernel = program.CreateKernel("MatrixMulti");

            ComputeBuffer<double> src1Buffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, src1);
            ComputeBuffer<double> src2Buffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, src2);
            ComputeBuffer<double> dstBuffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.WriteOnly, dst.Length);

            kernel.SetMemoryArgument(0, src1Buffer);
            kernel.SetMemoryArgument(1, src2Buffer);
            kernel.SetValueArgument(2, ncols);
            kernel.SetMemoryArgument(3, dstBuffer);

            ComputeEventList events = new ComputeEventList();

            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

            commands.Execute(kernel, null, new long[] { src1.Length }, null, events);

            commands.ReadFromBuffer(dstBuffer, ref dst, false, events);

            commands.Finish();

            var expected = new double[]
            {
                30, 36, 42,
                66, 81, 96,
                102, 126, 150
            };
            CollectionAssert.AreEqual(expected, dst);
        }

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
