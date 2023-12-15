using System.Diagnostics;
using System.Reflection;
using CsTools;
using CsTools.Extensions;
using WebWindowNetCore.Data;

using static ClrWinApi.Api;

namespace WebWindowNetCore;

public class WebViewBuilder : WebWindowNetCore.Base.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    public WebViewBuilder FormCreating(Action<WebWindowForm> onFormCreation)
        => this.SideEffect(n => OnFormCreation = onFormCreation);

    public string AppDataPath { get; }

    internal new WebViewSettings Data { get => base.Data; }

    internal WebViewBuilder()
    {
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        var loader = GetWebViewLoader();
        AppDataPath = new FileInfo(loader).DirectoryName!;
        LoadLibrary(loader);

        static string GetWebViewLoader()
        {
            var targetFileName = 
                Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AppendPath(@$"de.uriegel.WebWindowNetCore\{Process.GetCurrentProcess().ProcessName}")
                .EnsureDirectoryExists()
                .AppendPath("WebView2Loader.dll");

            try 
            {
                using var targetFile = targetFileName.CreateFile();
                Assembly
                    .GetExecutingAssembly()
                    ?.GetManifestResourceStream("binaries/webviewloader")
                    ?.CopyTo(targetFile);
            }
            catch {}
            return targetFileName;
        }
    }

    internal Action<WebWindowForm>? OnFormCreation;
}