using WebWindowNetCore;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var window = new WebWindow();
        window.Initialize(new() { Title = "Das ist das Webview-Fenster", Url="http://www.microsoft.com"});
        window.Execute();
    }
}