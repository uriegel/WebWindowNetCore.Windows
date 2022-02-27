using System.Runtime.InteropServices;

namespace WebWindowNetCore;
public class WebWindow : IWebWindow
{
    public void Initialize(Configuration configuration) => this.configuration = configuration;
    
    public void Execute()
    {
        var webForm = new WebForm(configuration!);
        webForm.Show();
        webForm.FormClosed += (s, e) => PostQuitMessage(0);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
    }

    Configuration? configuration;


    [DllImport("user32.dll")]
    static extern void PostQuitMessage(int exitCode);
    [DllImport("user32.dll")]
    static extern sbyte GetMessage(out ApiMessage message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref ApiMessage message);
}
