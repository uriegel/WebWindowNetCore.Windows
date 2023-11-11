using AspNetExtensions;
using LinqTools;
using WebWindowNetCore;

var sseEventSource = WebView.CreateEventSource<Event>();
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .InitialBounds(800, 600)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    .OnFilesDrop(pathes => 
    {
        var pa = pathes;
    })
    //.DebugUrl("http://localhost:3000")
    //.DebugUrl("https://google.de")
    .Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseSse("sse/test", sseEventSource)
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
    .Run("de.uriegel.Commander");    

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

record Event(string Content);
