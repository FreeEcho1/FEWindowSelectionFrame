namespace FreeEcho
{
    namespace FEWindowSelectionFrame
    {
        /// <summary>
        /// RECT
        /// </summary>
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /// <summary>
        /// POINT
        /// </summary>
        struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// WINDOWPLACEMENT
        /// </summary>
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        /// <summary>
        /// 低レベルのマウス入力イベント情報
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        /// <summary>
        /// ネイティブメソッド
        /// </summary>
        class NativeMethods
        {
            public delegate System.IntPtr MouseHookCallback(int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr WindowFromPoint(System.Drawing.Point Point);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr GetAncestor(System.IntPtr hWnd, uint gaFlags);
            [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern System.IntPtr GetWindowRect(System.IntPtr hWnd, out RECT lpRect);
            [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern bool GetWindowPlacement(System.IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, MouseHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);
        }
    }
}
