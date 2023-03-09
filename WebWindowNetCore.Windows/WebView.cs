using WebWindowNetCore.Data;
using static ClrWinApi.Api;

namespace WebWindowNetCore.Windows;

enum Action
{
    DevTools = 1,
    Show,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : WebWindowNetCore.WebView
{
    public static WebViewBuilder Create()
        => new WebViewBuilder();

    public override int Run(string gtkId = "de.uriegel.WebViewNetCore")
    {
        var webForm = new WebWindowForm(settings);
        webForm.Show();
        webForm.FormClosed += (s, e) => PostQuitMessage(0);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
        return 0;
    }


        // => new Application(gtkId)
        //     .Run(app =>
        //     {
        //     app.EnableSynchronizationContext();

        //     GtkDotNet.Timer? timer = null;
        //     saveBounds = settings?.SaveBounds == true;

        //     var window = new Window();
        //     var webView = new GtkDotNet.WebView();
        //     window.Add(webView);
        //     webView.Settings.EnableDeveloperExtras = true;
        //     if (settings?.Url != null)
        //         webView.LoadUri(settings?.Url);
        //     if (settings?.DevTools == true)
        //         webView.Settings.EnableDeveloperExtras = true;
        //     app.AddWindow(window);
        //     window.SetTitle(settings?.Title);
        //     window.SetSizeRequest(200, 200);
        //     window.SetDefaultSize(settings!.Width, settings!.Height);
        //     if (!saveBounds)
        //         window.ShowAll();
        //     else
        //     {
        //         var w = settings?.Width;
        //         var h = settings?.Height;
        //         webView.LoadChanged += (s, e) =>
        //         {
        //             if (e.LoadEvent == WebKitLoadEvent.WEBKIT_LOAD_COMMITTED)
        //                 webView.RunJavascript(
        //                 """ 
        //                     const devTools = document.getElementById('devTools')
        //                     devTools.onclick = () => alert(JSON.stringify({action: 1}))

        //                     const bounds = JSON.parse(localStorage.getItem('window-bounds') || '{}')
        //                     const isMaximized = localStorage.getItem('isMaximized')
        //                     if (bounds.width && bounds.height)
        //                         alert(JSON.stringify({action: 2, width: bounds.width, height: bounds.height, isMaximized: isMaximized == 'true'}))
        //                     else
        //                         alert(JSON.stringify({action: 2}))
        //                 """);
        //         };

        //         window.Configure += (s, e) =>
        //         {
        //             timer?.Dispose();
        //             timer = new(() => 
        //             {
        //                 if (!window.IsMaximized())
        //                     webView.RunJavascript(
        //                         $$"""
        //                             localStorage.setItem('window-bounds', JSON.stringify({width: {{e.Width}}, height: {{e.Height}}}))
        //                             localStorage.setItem('isMaximized', false)
        //                         """);
        //                 else
        //                     webView.RunJavascript($"localStorage.setItem('isMaximized', true)");
        //             }, TimeSpan.FromMilliseconds(400), Timeout.InfiniteTimeSpan);
        //         };
        //     }

        //     webView.ScriptDialog += (s, e) =>
        //     {
        //         Console.WriteLine(e.Message);
        //         var action = JsonSerializer.Deserialize<ScriptAction>(e.Message, JsonDefault.Value);
        //         switch (action?.Action)
        //         {
        //             case Action.DevTools:
        //                 webView.Inspector.Show();
        //                 break;
        //             case Action.Show:
        //                 if (action.Width.HasValue && action.Height.HasValue)
        //                     window.Resize(action.Width.Value, action.Height.Value);                            
        //                 if (action.IsMaximized.GetOrDefault(false))
        //                    window.Maximize();
        //                 window.ShowAll();   
        //                 break;
        //         }
        //     };

        //     settings = null;
        //});

    internal WebView(WebViewBuilder builder)
        => settings = builder.Data;

    WebViewSettings? settings;

    bool saveBounds;
}