using System.Runtime.InteropServices;

namespace WebWindowNetCore;

[ComVisible(true)]
public class Callback
{
    public Callback(WebWindowForm parent) => this.parent = parent;

    public void Init(int width, int height, bool maximize) 
        => parent.Init(width, height, maximize);

    public void ShowDevtools() => parent.ShowDevtools();
    public void MaximizeWindow() => parent.MaximizeWindow();
    public void MinimizeWindow() => parent.MinimizeWindow();
    public void RestoreWindow() => parent.RestoreWindow();
    public int GetWindowState() => parent.GetWindowState();
    public void DragFiles() => parent.DragFiles();

    WebWindowForm parent;
}
