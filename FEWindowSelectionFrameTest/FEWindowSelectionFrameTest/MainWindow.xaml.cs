namespace FEWindowSelectionFrameTest
{
    public partial class MainWindow : System.Windows.Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetWindowTextLength(System.IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(System.IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        FreeEcho.FEWindowSelectionFrame.WindowSelectionFrame WindowSelectionFrame;
        System.Windows.Threading.DispatcherTimer SelectionTimer;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                SelectionTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = new System.TimeSpan(0, 0, 10)
                };
                SelectionTimer.Tick += SelectionTimer_Tick;

                ButtonWindowSelectionTimer.Click += ButtonWindowSelectionTimer_Click;
                ButtonWindowSelectionLeftButtonUp.PreviewMouseDown += ButtonWindowSelectionLeftButtonUp_PreviewMouseDown;
            }
            catch
            {
            }
        }

        private void SelectionTimer_Tick(object sender, System.EventArgs e)
        {
            try
            {
                SelectionTimer.Stop();
                WindowSelectionFrame.StopWindowSelection();

                System.Text.StringBuilder string_data = new System.Text.StringBuilder(GetWindowTextLength(WindowSelectionFrame.SelectedHwnd) + 1);
                GetWindowText(WindowSelectionFrame.SelectedHwnd, string_data, string_data.Capacity);
                System.Diagnostics.Debug.WriteLine(string_data.ToString());

                WindowSelectionFrame.MouseLeftButtonUp -= WindowSelectionFrame_MouseLeftButtonUpEvent;
                WindowSelectionFrame = null;
                System.Windows.MessageBox.Show("選択しました。");
            }
            catch
            {
            }
        }

        private void ButtonWindowSelectionTimer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Windows.MessageBox.Show("10秒後に選択されます。");
                WindowSelectionFrame = new FreeEcho.FEWindowSelectionFrame.WindowSelectionFrame();
                WindowSelectionFrame.StartWindowSelection();
                SelectionTimer.Start();
            }
            catch
            {
            }
        }

        private void ButtonWindowSelectionLeftButtonUp_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                WindowSelectionFrame = new FreeEcho.FEWindowSelectionFrame.WindowSelectionFrame
                {
                    MouseLeftUpStop = true
                };
                WindowSelectionFrame.MouseLeftButtonUp += WindowSelectionFrame_MouseLeftButtonUpEvent;
                WindowSelectionFrame.StartWindowSelection();
            }
            catch
            {
            }
        }

        private void WindowSelectionFrame_MouseLeftButtonUpEvent(object sender, FreeEcho.FEWindowSelectionFrame.MouseLeftButtonUpEventArgs e)
        {
            try
            {
                System.Text.StringBuilder string_data = new System.Text.StringBuilder(GetWindowTextLength(WindowSelectionFrame.SelectedHwnd) + 1);
                GetWindowText(WindowSelectionFrame.SelectedHwnd, string_data, string_data.Capacity);
                System.Diagnostics.Debug.WriteLine(string_data.ToString());

                WindowSelectionFrame.MouseLeftButtonUp -= WindowSelectionFrame_MouseLeftButtonUpEvent;
                WindowSelectionFrame = null;
            }
            catch
            {
            }
        }
    }
}
