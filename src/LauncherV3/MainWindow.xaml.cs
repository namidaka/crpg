namespace LauncherV3;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.SharpZipLib.Tar;
using LauncherV3.LauncherHelper;
using static System.Net.Mime.MediaTypeNames;
using static LauncherV3.LauncherHelper.GameInstallationFolderResolver;
using static LauncherV3.MainViewModel;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    private GameInstallationInfo? gameLocation;
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private DispatcherTimer _flushTimer;
    private readonly static MainViewModel ViewModel = new();
    public MainWindow()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) => Log(args?.ExceptionObject.ToString() ?? string.Empty);
        InitializeComponent();
        DataContext = ViewModel;
        Instance = this;
        _flushTimer = new DispatcherTimer();
        _flushTimer.Interval = TimeSpan.FromMilliseconds(2000);
        _flushTimer.Tick += FlushTimer_Tick;
        _flushTimer.Start();
        ViewModel.RequestClose += (sender, e) => Close();
    }


    private void Log(string exception)
    {

            File.AppendAllText("appErrorLog.txt", exception);

    }

    private void LogException(Exception ex)
    {
        if (ex != null)
        {
            File.AppendAllText("appErrorLog.txt", ex.ToString());
        }
    }
    private void Window_Loaded(object sender, EventArgs e)
    {
        if (!ViewModel.HasWritePermissionOnConfigDir())
        {
            MessageBox.Show(
                "Please extract the launcher first and put it in a folder where you have write permission.",
                "Write Permission Required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            Close();
        }

        ViewModel.StartRoutine();
    }

    private async void FlushTimer_Tick(object? sender, EventArgs e)
    {
        // Stop the timer to prevent re-entrancy issues
        _flushTimer?.Stop();
        // Thread-safe asynchronous flush
        await FlushTextToTextBoxAsync();

        // Restart the timer
        _flushTimer?.Start();
    }

    private async Task FlushTextToTextBoxAsync()
    {
        StringBuilder sb = new StringBuilder();
        string viewModelText = ViewModel.FlushText();
        if (viewModelText != string.Empty)
        {
            sb.AppendLine(viewModelText);
        }

        while (_messageQueue.TryDequeue(out string? text))
        {
            sb.AppendLine(text);
        }

        consoleTextBox.AppendText(sb.ToString());
        consoleTextBox.ScrollToEnd();

    }

    public void WriteToConsole(string text)
    {
        _messageQueue.Enqueue(text);
    }

    private void MinimizeButton_Click(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            e.Handled = true;
        }
        else
        {
            WindowState = WindowState.Minimized;
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        // Use the Process class to start the default browser with the URL
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true // necessary for .NET Core and above
        });
        e.Handled = true; // Indicates that the request is handled
    }

    private void CloseButton_Click(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            e.Handled = true;
        }
        else
        {
            Close();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // This allows the window to be dragged around by the mouse.
        this.DragMove();
    }

    private void locationButton_Click(object sender, RoutedEventArgs e)
    {

    }

}
