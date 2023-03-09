using System.Runtime.InteropServices;

namespace ClrWinApi;

[StructLayout(LayoutKind.Sequential)]
public struct ApiMessage
{
    /// <summary>
    /// Das Fenster, welches die Nachricht bekommen soll/hat
    /// </summary>
    public IntPtr Window;
    /// <summary>
    /// Die Art der Nachricht
    /// </summary>
    public UInt32 MessageType;
    public IntPtr WParam;
    public IntPtr LParam;
    public UInt32 Time;
    public MousePoint MousePoint;
}

[StructLayout(LayoutKind.Sequential)]
public struct MousePoint
{ 
    public int X;
    public int Y;
}

public static class Api
{
    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int exitCode);
    [DllImport("user32.dll")]
    public static extern sbyte GetMessage(out ApiMessage message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage([In] ref ApiMessage message);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibrary(string filename);
}