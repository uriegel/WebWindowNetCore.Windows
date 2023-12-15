using AspNetExtensions;
using CsTools.Extensions;
using WebWindowNetCore;

//ApplicationConfiguration.Initialize();

var sseEventSource = WebView.CreateEventSource<Event>();
WebWindowForm? webViewForm = null;
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .SetAppId("de.uriegel.webwindownetcode.windows")
    .InitialBounds(800, 600)
    .SaveBounds()
    .DownCast<WebViewBuilder>()
    .FormCreating(FormCreation)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    .DefaultContextMenuEnabled()    
    .OnFilesDrop((id, move, pathes) => 
    {
        var pa = pathes;
    })
    .DebugUrl("http://127.0.0.1:20000")
    //.DebugUrl("https://google.de")
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseSse("sse/test", sseEventSource)
        .UseReverseProxy("127.0.0.1", "", "http://localhost:5173")
        .MapGet("video", context => 
            context
                .SideEffect(c => Console.WriteLine("Range request"))
                .SideEffect(c => c.Response.ContentType = "Hafenrundfahrt.mp4".GetMimeType())
            .StreamRangeFile(@"C:\Users\uwe\Documents\Hafenrundfahrt.mp4"))
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run();    

void StartEvents(Action<Event> onChanged)   
{
    var counter = 0;
    new Thread(_ =>
        {
            while (true)
            {
                Thread.Sleep(5000);
                onChanged(new($"Ein Event {counter++}"));
            }
        })
        {
            IsBackground = true
        }.Start();   
}

void FormCreation(WebWindowForm form)
    => webViewForm = form;

record Event(string Content);
