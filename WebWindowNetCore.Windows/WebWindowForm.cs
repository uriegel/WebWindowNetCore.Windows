using System.Diagnostics;
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

    public void ShowDevtools()
        => webView.CoreWebView2.OpenDevToolsWindow();
    
    public WebWindowForm(WebViewSettings? settings, string appDataPath) 
    {
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

        Text = settings?.Title;
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

        StartWebviewInit();

        async void StartWebviewInit()
        {
            var opts = new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--enable-features=msWebView2EnableDraggableRegions"
            };
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


    WebView2 webView;
    bool initialized;
}
