using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SoulEngine.Engine.Native
{
    public class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public IntPtr msg;
            public IntPtr wParam;
            public IntPtr lParam;
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(
            out Message msg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax,
            uint wRemoveMsg);

    }
}
