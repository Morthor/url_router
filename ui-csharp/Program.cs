using System;
using System.Windows.Forms;

namespace UrlRouterUI;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure high DPI settings
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
