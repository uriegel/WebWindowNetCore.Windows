using System.Runtime.InteropServices;
using System.Text.Json;

namespace WebWindowNetCore;

public record Settings(int x, int y, int width, int height, bool isMaximized);
public class WebWindow : IWebWindow
{
    public void Initialize(Configuration configuration) => this.configuration = configuration ?? new Configuration();
    
    public void Execute()
    {
        var settings = new Settings(configuration.InitialPosition?.X ?? -1, configuration.InitialPosition?.Y ?? -1, 
            configuration.InitialSize?.Width ?? 800, configuration.InitialSize?.Height ?? 600, configuration.IsMaximized == true);
        var settingsFile = "";      

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
                    var s = JsonSerializer.Deserialize<Settings>(settingsString);
                    if (s != null)
                        settings = s;
                }
            }
        }  

        var webForm = new WebForm(configuration, settings, settingsFile);
        webForm.Show();
        webForm.FormClosed += (s, e) => PostQuitMessage(0);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
    }

    Configuration configuration = new Configuration();

    [DllImport("user32.dll")]
    static extern void PostQuitMessage(int exitCode);
    [DllImport("user32.dll")]
    static extern sbyte GetMessage(out ApiMessage message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref ApiMessage message);
}
