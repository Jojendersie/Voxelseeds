using System;

namespace VoxelSeeds
{
    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
#if NETFX_CORE
        [MTAThread]
#else
        [STAThread]
#endif
        static void Main()
        {
            using (var program = new VoxelSeeds())
                program.Run();
        }
    }
}
