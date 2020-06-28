namespace FreeEcho
{
    namespace FEWindowSelectionFrame
    {
        /// <summary>
        /// ウィンドウ選択開始の例外
        /// </summary>
        public class WindowSelectionStartException : System.Exception
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public WindowSelectionStartException()
                : base("Failed to start window selection.")
            {
            }
        }

        /// <summary>
        /// ウィンドウ選択停止の例外
        /// </summary>
        public class WindowSelectionStopException : System.Exception
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public WindowSelectionStopException()
                : base("Failed to stop window selection.")
            {
            }
        }

        /// <summary>
        /// マウスの左ボタンが離されたイベントデータ
        /// </summary>
        public class MouseLeftButtonUpEventArgs
        {
        }

        /// <summary>
        /// ウィンドウ選択枠
        /// </summary>
        public class WindowSelectionFrame
        {
            /// <summary>
            /// フレームウィンドウ
            /// </summary>
            private FrameWindow FrameWindow;
            /// <summary>
            /// フレームウィンドウの色
            /// </summary>
            public System.Drawing.Color ColorFrameWindow
            {
                get;
                set;
            } = System.Drawing.Color.FromArgb(255, 0, 0);
            /// <summary>
            /// フレームの幅
            /// </summary>
            public int FrameWidth
            {
                get;
                set;
            } = 5;
            /// <summary>
            /// 選択したウィンドウのハンドル
            /// </summary>
            public System.IntPtr SelectedHwnd
            {
                get;
                protected set;
            }
            /// <summary>
            /// マウスの左ボタンが離されたら停止するか
            /// </summary>
            public bool MouseLeftUpStop
            {
                get;
                set;
            } = false;
            /// <summary>
            /// マウスのフックプロシージャのハンドル
            /// </summary>
            private System.IntPtr Handle;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public WindowSelectionFrame()
            {
            }

            /// <summary>
            /// フレームウィンドウ処理
            /// </summary>
            private void FrameWindowProcessing()
            {
                System.IntPtr selected_hwnd;

                if ((selected_hwnd = NativeMethods.WindowFromPoint(System.Windows.Forms.Control.MousePosition)) != System.IntPtr.Zero)
                {
                    if ((selected_hwnd = NativeMethods.GetAncestor(selected_hwnd, 2)) != System.IntPtr.Zero)       // 2 = GA_ROOT (親ウィンドウのルートウィンドウを取得の値)
                    {
                        if ((selected_hwnd != SelectedHwnd) && (selected_hwnd != FrameWindow.Handle))
                        {
                            SelectedHwnd = selected_hwnd;

                            RECT window_rect;
                            NativeMethods.GetWindowRect(selected_hwnd, out window_rect);
                            window_rect.right -= window_rect.left;
                            window_rect.bottom -= window_rect.top;

                            // 枠の位置とサイズを計算
                            WINDOWPLACEMENT window_placement;
                            NativeMethods.GetWindowPlacement(selected_hwnd, out window_placement);
                            System.Drawing.Rectangle set_frame_window_rectangle = new System.Drawing.Rectangle();
                            if (window_placement.showCmd == 3)      // 3 = SW_SHOWMAXIMIZED
                            {
                                System.Drawing.Rectangle screen_rectangle = System.Windows.Forms.Screen.FromHandle(selected_hwnd).WorkingArea;
                                set_frame_window_rectangle.X = screen_rectangle.Left;
                                set_frame_window_rectangle.Y = screen_rectangle.Top;
                                set_frame_window_rectangle.Width = window_rect.right - ((screen_rectangle.Left - window_rect.left) * 2);
                                set_frame_window_rectangle.Height = window_rect.bottom - ((screen_rectangle.Top - window_rect.top) * 2);
                            }
                            else
                            {
                                set_frame_window_rectangle.X = window_rect.left;
                                set_frame_window_rectangle.Y = window_rect.top;
                                set_frame_window_rectangle.Width = window_rect.right;
                                set_frame_window_rectangle.Height = window_rect.bottom;
                            }

                            // ディスプレイと同じサイズの場合は、全画面モード機能が反応しないようにサイズ調整
                            System.Drawing.Rectangle screen = System.Windows.Forms.Screen.FromHandle(selected_hwnd).Bounds;
                            if ((screen.Left == set_frame_window_rectangle.Left)
                                && (screen.Top == set_frame_window_rectangle.Top)
                                && (screen.Right == set_frame_window_rectangle.Right)
                                && (screen.Bottom == set_frame_window_rectangle.Bottom))
                            {
                                set_frame_window_rectangle.X += 1;
                                set_frame_window_rectangle.Y += 1;
                                set_frame_window_rectangle.Width -= 2;
                                set_frame_window_rectangle.Height -= 2;
                            }

                            // パスを設定
                            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                            path.AddRectangle(new System.Drawing.Rectangle(0, 0, set_frame_window_rectangle.Width, set_frame_window_rectangle.Height));     // 外側の四角形
                            path.AddRectangle(new System.Drawing.Rectangle(FrameWidth, FrameWidth, set_frame_window_rectangle.Width - (FrameWidth + FrameWidth), set_frame_window_rectangle.Height - (FrameWidth + FrameWidth)));       // 内側の四角形

                            FrameWindow.Location = new System.Drawing.Point(set_frame_window_rectangle.X, set_frame_window_rectangle.Y);
                            FrameWindow.Size = new System.Drawing.Size(set_frame_window_rectangle.Width, set_frame_window_rectangle.Height);
                            FrameWindow.Region = new System.Drawing.Region(path);
                            FrameWindow.Opacity = 1.0;
                        }
                    }
                }
            }

            /// <summary>
            /// 破棄
            /// </summary>
            private void Dispose()
            {
                if (FrameWindow != null)
                {
                    FrameWindow.Close();
                    FrameWindow.Dispose();
                    FrameWindow = null;
                }
            }

            /// <summary>
            /// ウィンドウ選択開始
            /// </summary>
            /// <exception cref="WindowSelectionStartException"></exception>
            public void StartWindowSelection()
            {
                try
                {
                    if (Handle == System.IntPtr.Zero)
                    {
                        SelectedHwnd = System.IntPtr.Zero;
                        FrameWindow = new FrameWindow
                        {
                            BackColor = ColorFrameWindow,
                            Visible = false
                        };
                        FrameWindow.Show();

                        HookCallback = MouseHookProcedure;
                        Handle = NativeMethods.SetWindowsHookEx(14, HookCallback, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetEntryAssembly().GetModules()[0]), 0);      // 14 = WH_MOUSE_LL
                        if (Handle == System.IntPtr.Zero)
                        {
                            throw new System.ComponentModel.Win32Exception();
                        }
                    }
                }
                catch
                {
                    Dispose();
                    throw new WindowSelectionStartException();
                }
            }

            /// <summary>
            /// ウィンドウ選択停止
            /// </summary>
            /// <exception cref="WindowSelectionStopException"></exception>
            public void StopWindowSelection()
            {
                try
                {
                    Dispose();

                    if (Handle != System.IntPtr.Zero)
                    {
                        NativeMethods.UnhookWindowsHookEx(Handle);
                        Handle = System.IntPtr.Zero;
                        HookCallback -= MouseHookProcedure;
                    }
                }
                catch
                {
                    throw new WindowSelectionStopException();
                }
            }

            /// <summary>
            /// マウスの左ボタンが離されたイベントのデリゲート
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void MouseLeftButtonUpEventHandler(
                object sender,
                MouseLeftButtonUpEventArgs e
                );
            /// <summary>
            /// マウスの左ボタンが離された
            /// </summary>
            public event MouseLeftButtonUpEventHandler MouseLeftButtonUp;

            /// <summary>
            /// フックチェーンにインストールするフックプロシージャのイベント
            /// </summary>
            private event NativeMethods.MouseHookCallback HookCallback;

            /// <summary>
            /// マウスのフックプロシージャ
            /// </summary>
            /// <param name="nCode">フックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="s">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            private System.IntPtr MouseHookProcedure(
                int nCode,
                uint msg,
                ref MSLLHOOKSTRUCT s
                )
            {
                try
                {
                    if (0 <= nCode)
                    {
                        switch (msg)
                        {
                            case 0x0202:        // WM_LBUTTONUP
                                if (MouseLeftUpStop)
                                {
                                    StopWindowSelection();
                                    MouseLeftButtonUp?.Invoke(this, new MouseLeftButtonUpEventArgs());
                                }
                                break;
                            case 0x0200:      // WM_MOUSEMOVE
                                FrameWindowProcessing();
                                break;
                        }
                    }
                }
                catch
                {
                }

                return (NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s));
            }
        }
    }
}
