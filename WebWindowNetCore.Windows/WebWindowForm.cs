using System.Diagnostics;
using ClrWinApi;
using CsTools.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public class WebWindowForm : Form
{
    public void Init(int width, int height, bool maximize)  
    {
        ClientSize = new Size(width, height);
        if (maximize)
            WindowState = FormWindowState.Maximized;
    }

    public void ShowDevtools() => webView.CoreWebView2.OpenDevToolsWindow();
    
    public WebWindowForm(WebViewSettings? settings, string appDataPath) 
    {
        noTitlebar = settings?.WithoutNativeTitlebar == true;

        if (!noTitlebar)
            Text = settings?.Title;
        else
        {
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        webView = new WebView2();
        ((System.ComponentModel.ISupportInitialize)(webView)).BeginInit();

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
        // 
        // Form1
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        
        if (settings?.ResourceIcon != null)
            Icon = new Icon(Resources.Get(settings.ResourceIcon)!);

        AutoScaleDimensions = new SizeF(8F, 20F);
        WindowState = FormWindowState.Minimized;
        AutoScaleMode = AutoScaleMode.Font;
        
        Controls.Add(webView);
        Name = "WebWindow";

        ((System.ComponentModel.ISupportInitialize)(webView)).EndInit();
        ResumeLayout(false);

        Resize += async (s, e) =>
        {
            if (initialized && settings?.SaveBounds == true) {
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

        // TODO NoTitlebar: maximize
        // TODO NoTitlebar: minimize
        // TODO NoTitlebar: icon from resource
        // TODO WindowChrome on top 2px color1 when focused, otherwise color2

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
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.WindowCloseRequested += (obj, e) => Close();
            
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
                
            var url = Debugger.IsAttached && !string.IsNullOrEmpty(settings?.DebugUrl)
                ? settings?.DebugUrl
                : settings?.Url != null
                ? settings.Url
                : $"http://localhost:{settings?.HttpSettings?.Port ?? 80}{settings?.HttpSettings?.WebrootUrl}/{settings?.HttpSettings?.DefaultHtml}";
            webView.Source = new Uri(url + settings?.Query ?? "");

            WindowState = FormWindowState.Normal;
            initialized = true;
            await webView.ExecuteScriptAsync(
                $$"""
                    const callback = chrome.webview.hostObjects.Callback
                    callback.Ready()
                    const bounds = JSON.parse(localStorage.getItem('window-bounds') || '{}')
                    const isMaximized = localStorage.getItem('isMaximized')
                    callback.Init(bounds.width || {{settings?.Width ?? 800}}, bounds.height || {{settings?.Height ?? 600}}, isMaximized == 'true')
                """);
            if (settings?.DevTools == true)
                await webView.ExecuteScriptAsync(
                    """ 
                        function webViewShowDevTools() {
                            callback.ShowDevtools()
                        }
                    """);
            if ((settings?.HttpSettings?.RequestDelegates?.Length ?? 0) > 0)
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

    const int WM_NCCALCSIZE = 0x83;

    readonly WebView2 webView;
    bool initialized;
    readonly bool noTitlebar;
}
