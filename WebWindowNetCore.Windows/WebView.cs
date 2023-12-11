using WebWindowNetCore.Data;

namespace WebWindowNetCore;

enum Action
{
    DevTools = 1,
    Show,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : Base.WebView
{
    public static WebViewBuilder Create() => new();

    public override int Run()
    {
        var webForm = new WebWindowForm(settings, OnFormCreation, appDataPath);
        Application.Run(webForm);
        return 0;
    }

    internal WebView(WebViewBuilder builder)
    {
        appDataPath = builder.AppDataPath;
        OnFormCreation = (builder as WebWindowNetCore.WebViewBuilder).OnFormCreation;
        settings = builder.Data;
    }

    readonly Action<WebWindowForm>? OnFormCreation;
    readonly string appDataPath;
    readonly WebViewSettings settings;
}
