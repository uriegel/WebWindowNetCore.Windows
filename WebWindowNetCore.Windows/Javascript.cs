using System.Runtime.InteropServices;
using System.Text.Json;
using LinqTools;
using static AspNetExtensions.Core;

namespace WebWindowNetCore;

[ComVisible(true)]
public class Callback
{
    public Callback(WebWindowForm parent) => this.parent = parent;

    public void ShowDevtools() => parent.ShowDevtools();
    public void MaximizeWindow() => parent.MaximizeWindow();
    public void MinimizeWindow() => parent.MinimizeWindow();
    public void RestoreWindow() => parent.RestoreWindow();
    public int GetWindowState() => parent.GetWindowState();
    public void DragStart(string fileList)
        => JsonSerializer.Deserialize<FileListType>(fileList, JsonWebDefaults)
            ?.SideEffect(flt =>
                parent.DragStart(flt.Path, flt.FileList));
        

    readonly WebWindowForm parent;
}

record FileListType(string[] FileList, string Path);