using Cloo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm_lib_cs
{
    public static partial class CSMM
    {
        private static ComputeContext context = null;
        private static ComputeKernel kernel = null;
        public static void InitCloo()
        {
            if (kernel != null)
            {
                return;
            }

            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);

            context = new ComputeContext(ComputeDeviceTypes.Gpu, cpl, null, IntPtr.Zero);

            ComputeProgram program = new ComputeProgram(context, MatrixMultiSource);
            program.Build(null, null, null, IntPtr.Zero);

            kernel = program.CreateKernel("MatrixMulti");
        }

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

        public unsafe static void MatrixMulti_OpenCL(double[,] result, double[,] a, double[,] b)
        {
            InitCloo();

            var ncols = result.GetUpperBound(0) + 1;
            var nrows = result.GetUpperBound(1) + 1;

            fixed (double* rp = result, ap = a, bp = b)
            {
                ComputeBuffer<double> aBuffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, a.Length, (IntPtr)ap);
                ComputeBuffer<double> bBuffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, b.Length, (IntPtr)bp);
                ComputeBuffer<double> rBuffer = new ComputeBuffer<double>(context, ComputeMemoryFlags.WriteOnly, result.Length);

                kernel.SetMemoryArgument(0, aBuffer);
                kernel.SetMemoryArgument(1, bBuffer);
                kernel.SetValueArgument(2, ncols);
                kernel.SetMemoryArgument(3, rBuffer);

                ComputeEventList events = new ComputeEventList();

                ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

                commands.Execute(kernel, null, new long[] { result.Length }, null, events);

                commands.ReadFromBuffer(rBuffer, ref result, false, new SysIntX2(), new SysIntX2(), new SysIntX2(ncols, nrows), events);

                commands.Finish();
            }
        }
    }
}
