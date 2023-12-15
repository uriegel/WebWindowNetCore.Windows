using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using ClrWinApi;
using CsTools.Extensions;
using WebWindowNetCore.Data;

using static AspNetExtensions.Core;
using LinqTools;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace WebWindowNetCore;

public class WebWindowForm : Form
{
    public void ShowDevtools() => webView.CoreWebView2.OpenDevToolsWindow();
    public void MaximizeWindow() => WindowState = FormWindowState.Maximized;
    public void MinimizeWindow() => WindowState = FormWindowState.Minimized;
    public void RestoreWindow() => WindowState = FormWindowState.Normal;
    public Task DragStart(string path, string[] fileList) 
        => new TaskCompletionSource<int>()
            .SideEffect(t => dropFinishedSubject
                                .Take(1)
                                .Subscribe(_ => t.SetResult(0)))
            .SideEffect(_ => DoDragDrop(new DataObject(DataFormats.FileDrop, fileList
                                                            .Select(f => path.AppendPath(f))
                                                            .ToArray()), DragDropEffects.All))
            .Task;
    
    public int GetWindowState() => (int)WindowState;

    public void ScriptAction(int id) => OnScriptAction?.Invoke(id, null); 
        
    public WebWindowForm(WebViewSettings settings, Action<WebWindowForm>? OnFormCreation, string appDataPath) 
    {
        noTitlebar = settings.WithoutNativeTitlebar == true;
        OnScriptAction = settings.OnScriptAction;

        FormClosing += (s, e) => 
            s.SideEffectIf(WindowState == FormWindowState.Normal,
                _ => (Data.Bounds.Retrieve(settings.AppId, new Bounds(null, null, settings.Width, settings.Height, null))
                        with { Width = Width, Height = Height })
                        .Save(settings.AppId));
        if (!noTitlebar) 
            Text = settings.Title;
        else if (Environment.OSVersion.Version.Build < 22000)
        {
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        webView = new WebView2();
        ((System.ComponentModel.ISupportInitialize)(webView)).BeginInit();

        QueryContinueDrag += (s, e) =>
            e.SideEffectIf(e.Action == DragAction.Drop || e.Action == DragAction.Cancel,
                _ => dropFinishedSubject.OnNext(0));

        SuspendLayout();
        // 
        // webView
        // 
        webView.AllowExternalDrop = true;
        webView.CreationProperties = null;
        webView.DefaultBackgroundColor = Color.White;
        webView.Location = new Point(0, 0);
        webView.Margin = new Padding(0);
        webView.Name = "webView";
        webView.Dock = DockStyle.Fill;
        webView.TabIndex = 0;
        webView.ZoomFactor = 1D;

        OnFormCreation?.Invoke(this);

        // 
        // Form1
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        
        if (settings.ResourceIcon != null)
            Icon = new Icon(Resources.Get(settings.ResourceIcon)!);

        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;

        var bounds = Data.Bounds.Retrieve(settings.AppId!, new Bounds(null, null, settings.Width, settings.Height, null));
        Width = bounds.Width ?? 800;
        Height = bounds.Height ?? 600;
        
        Controls.Add(webView);
        Name = "WebWindow";

        ((System.ComponentModel.ISupportInitialize)(webView)).EndInit();
        ResumeLayout(false);

        Resize += async (s, e) =>
        {
            if (initialized && settings.SaveBounds == true) {
                if (WindowState != FormWindowState.Maximized)
                    await webView.ExecuteScriptAsync(
                        $$"""
                            localStorage.setItem('window-bounds', JSON.stringify({width: {{Width}}, height: {{Height}}}))
                            localStorage.setItem('isMaximized', false)
                        """);
                else
                    await webView.ExecuteScriptAsync($"localStorage.setItem('isMaximized', true)");
            }
        };

        StartWebviewInit();

        async void StartWebviewInit()
        {
            var opts = noTitlebar
                ? new CoreWebView2EnvironmentOptions
                    {
                        AdditionalBrowserArguments = "--enable-features=msWebView2EnableDraggableRegions"
                    }
                : null;
            var enf = await  CoreWebView2Environment.CreateAsync(null, appDataPath, opts);
            await webView.EnsureCoreWebView2Async(enf);
            webView.CoreWebView2.AddHostObjectToScript("Callback", new Callback(this));
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = settings.DefaultContextMenuEnabled;
            webView.CoreWebView2.WindowCloseRequested += (obj, e) => Close();

            if (settings.OnFilesDrop != null)
                webView.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    var msg = JsonSerializer.Deserialize<WebMsg>(e.WebMessageAsJson, JsonWebDefaults);
                    if (msg?.Msg == 1)
                    {
                        var filesDropPathes = e.AdditionalObjects
                                                .Select(n => (n as CoreWebView2File)!.Path)
                                                .ToArray();
                        settings.OnFilesDrop(msg.Text ?? "", msg.Move, filesDropPathes);
                    }
                };
            
            webView.CoreWebView2.ContainsFullScreenElementChanged += (objs, args) =>
            {
                if (webView.CoreWebView2.ContainsFullScreenElement)
                {
                    TopMost = true;
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    Taskbar.Hide();
                }
                else
                {
                    TopMost = false;
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    Taskbar.Show();
                }
            };
                
            webView.Source = new Uri(WebViewSettings.GetUri(settings));

            WindowState = FormWindowState.Normal;
            initialized = true;
            await webView.ExecuteScriptAsync(
                $$"""
                    const callback = chrome.webview.hostObjects.Callback
                    callback.Ready()

                    function webViewMaximize() {
                        callback.MaximizeWindow()
                    }
                    function webViewMinimize() {
                        callback.MinimizeWindow()
                    }
                    function webViewRestore() {
                        callback.RestoreWindow()
                    }
                    async function webViewGetWindowState() {
                        return await callback.GetWindowState()
                    }
                    async function webViewDragStart(path, fileList) {
                        await callback.DragStart(JSON.stringify({path, fileList}))
                    }
                """);
            if (settings.DevTools == true)
                await webView.ExecuteScriptAsync(
                    """ 
                        function webViewShowDevTools() {
                            callback.ShowDevtools()
                        }
                    """);
            if (settings.OnScriptAction != null)
                await webView.ExecuteScriptAsync(
                    """ 
                        function webViewScriptAction(id) {
                            callback.ScriptAction(id)
                        }
                    """);
            if ((settings.HttpSettings?.RequestDelegates?.Length ?? 0) > 0)
                await webView.ExecuteScriptAsync(
                    """ 
                        async function webViewRequest(method, input) {

                            const msg = {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify(input)
                            }

                            const response = await fetch(`/request/${method}`, msg) 
                            return await response.json() 
                        }
                    """);
            if (settings.OnFilesDrop != null)
                await webView.ExecuteScriptAsync(
                    """ 
                        function webViewDropFiles(id, move, dropFiles) {
                            chrome.webview.postMessageWithAdditionalObjects({
                                msg: 1,
                                text: id,
                                move
                            }, dropFiles);
                        }
                    """);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (DesignMode || !noTitlebar)
            //Don't style window in designer...
            base.WndProc(ref m);
        else
            switch (m.Msg)
            {
                case WM_NCCALCSIZE:
                    CalcSizeNoTitlebar(ref m); 
                    break;

                default: 
                    base.WndProc(ref m); 
                    break;
            }
    }

    static void CalcSizeNoTitlebar(ref Message m)
    {
        if (m.WParam != IntPtr.Zero)    
        {
            var nccsp = NcCalcSizeParams.FromIntPtr(m.LParam);
            nccsp.Rgrc0.Top += 1;
            nccsp.Rgrc0.Bottom -= 5;
            nccsp.Rgrc0.Left += 5;  
            nccsp.Rgrc0.Right -= 5; 
            System.Runtime.InteropServices.Marshal.StructureToPtr(nccsp, m.LParam, true);
        }
        else
        {
            var clnRect = (Rect)System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typeof(Rect))!;
            clnRect.Top += 1;
            clnRect.Bottom -= 5;
            clnRect.Left += 5;  
            clnRect.Right -= 5; 
            System.Runtime.InteropServices.Marshal.StructureToPtr(clnRect, m.LParam, true);
        }
        m.Result = IntPtr.Zero;
    }

    readonly Action<int, string?>? OnScriptAction;

    readonly Subject<int> dropFinishedSubject = new();

    const int WM_NCCALCSIZE = 0x83;

    readonly WebView2 webView;
    bool initialized;
    readonly bool noTitlebar;
}

record WebMsg(int Msg, bool Move, string? Text);
