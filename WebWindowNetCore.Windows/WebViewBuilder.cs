using WebWindowNetCore.Data;

namespace WebWindowNetCore.Windows;

public class WebViewBuilder : WebWindowNetCore.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    internal new WebViewSettings Data { get => base.Data; }

    internal WebViewBuilder()
    {
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
    }
}