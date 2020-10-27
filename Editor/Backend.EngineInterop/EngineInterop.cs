using System;
using System.Runtime.InteropServices;

namespace Backend.EngineInterop
{
    public class EngineInterop
    {
        [DllImport("super_dark_engine")]
        public static extern ulong super_dark_sum(ulong a, ulong b);
    }
}
