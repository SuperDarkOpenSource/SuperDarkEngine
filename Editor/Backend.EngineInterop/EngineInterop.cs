using System;
using System.Runtime.InteropServices;

namespace Backend.EngineInterop
{
    public unsafe class EngineInterop
    {
        public delegate void ExternalWindowUpdateFn();

        public delegate string ExternalWindowReceiveFn();

        public delegate void ExternalWindowDeliverFn(string msg);

        [DllImport("super_dark_engine")]
        public static extern ulong super_dark_sum(ulong a, ulong b);

        [DllImport("super_dark_engine")]
        public static extern IntPtr externalwindow_create();
        
        [DllImport("super_dark_engine")]
        public static extern void externalwindow_set_update_fn(IntPtr external_window, IntPtr update_fn);
        
        [DllImport("super_dark_engine")]
        public static extern void externalwindow_set_receive_fn(IntPtr external_window, IntPtr receive_fn);
        
        [DllImport("super_dark_engine")]
        public static extern void externalwindow_set_deliver_fn(IntPtr external_window, IntPtr deliver_fn);
        
        [DllImport("super_dark_engine")]
        public static extern IntPtr gameenginecreationinfo_create();
        
        [DllImport("super_dark_engine")]
        public static extern void gameenginecreationinfo_set_externalwindow(IntPtr creation_info, IntPtr external_window);
        
        [DllImport("super_dark_engine")]
        public static extern IntPtr gameengine_create(IntPtr creation_info);
        
        [DllImport("super_dark_engine")]
        public static extern void gameengine_run(IntPtr game_engine);
    }
}
