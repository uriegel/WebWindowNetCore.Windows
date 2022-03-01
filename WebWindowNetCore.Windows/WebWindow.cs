using System.Runtime.InteropServices;
using System.Text.Json;

namespace WebWindowNetCore;

public abstract class WebWindowBase
{
    JsonSerializerOptions serializeOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public record Settings(int X, int Y, int Width, int Height, bool IsMaximized);

    public WebWindowBase(Configuration configuration) 
        => this.configuration = configuration;

    public void Execute()
    {
        var settings = new Settings(configuration.InitialPosition?.X ?? -1, configuration.InitialPosition?.Y ?? -1, 
            configuration.InitialSize?.Width ?? 800, configuration.InitialSize?.Height ?? 600, configuration.IsMaximized == true);

        if (configuration.SaveWindowSettings == true)
        {
            var appData = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                configuration.Organization!, configuration.Application!);
            if (!Directory.Exists(appData))
                Directory.CreateDirectory(appData);

            settingsFile = Path.Combine(appData, "settings.json");
            if (File.Exists(settingsFile))
            {
                using var reader = new StreamReader(File.OpenRead(settingsFile));
                var settingsString = reader.ReadToEnd();
                if (settingsString?.Length > 0)
                {
                    var s = JsonSerializer.Deserialize<Settings>(settingsString, serializeOptions);
                    if (s != null)
                        settings = s;
                }
            }
        }  
        Run(settings);
    }

    protected abstract void Run(Settings settings);

    protected void SaveSettings(WebWindowBase.Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, serializeOptions);
        using var writer = new StreamWriter(File.Create(settingsFile));
        writer.Write(json);
    }

    protected Configuration configuration;
    string settingsFile = "";      
}

public interface ISaveSettings
{
    void SaveSettings(WebWindowBase.Settings settings);
}

public class WebWindow : WebWindowBase, ISaveSettings
{
    public new void SaveSettings(WebWindowBase.Settings settings) => base.SaveSettings(settings);
    public WebWindow(Configuration configuration) : base(configuration) {}
    protected override void Run(Settings settings)
    {
        var webForm = new WebForm(configuration, settings, this as ISaveSettings);
        webForm.Show();
        webForm.FormClosed += (s, e) => PostQuitMessage(0);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
    }

    [DllImport("user32.dll")]
    static extern void PostQuitMessage(int exitCode);
    [DllImport("user32.dll")]
    static extern sbyte GetMessage(out ApiMessage message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref ApiMessage message);
}
