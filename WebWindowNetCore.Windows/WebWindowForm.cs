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

        this.ControlBox = false;
this.Text = String.Empty;
        
        
        FormBorderStyle = FormBorderStyle.Sizable;





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

        //Text = settings?.Title;



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

    const uint WM_NCCALCSIZE = 0x83;

    protected override void WndProc(ref Message m)
    {
        //Don't style window in designer...
        if (DesignMode)
            base.WndProc(ref m);

        //Handle Message
        switch ((uint)m.Msg)
        {
            case WM_NCCALCSIZE:
                WmNCCalcSize(ref m); 
                break;

            default: 
                base.WndProc(ref m); 
                break;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct RECT
    {
        public int left, top, right, bottom;
    }

     [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool GetWindowRect(
        IntPtr hwnd,
        out  RECT lpRect
        );

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct NCCALCSIZE_PARAMS
    {
        public RECT rgrc0, rgrc1, rgrc2;
        public WINDOWPOS lppos;
    }        

 [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndinsertafter;
        public int x, y, cx, cy;
        public int flags;
    }
//WM_NCCALCSIZE
    private void WmNCCalcSize(ref Message m)
    {
        //Get Window Rect
        RECT formRect = new RECT();
        GetWindowRect(m.HWnd, out formRect);

        //Check WPARAM
        if (m.WParam != IntPtr.Zero)    //TRUE
        {
            //When TRUE, LPARAM Points to a NCCALCSIZE_PARAMS structure
            var nccsp = (NCCALCSIZE_PARAMS)System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));

            //We're adjusting the size of the client area here. Right now, the client area is the whole form.
            //Adding to the Top, Bottom, Left, and Right will size the client area.
            nccsp.rgrc0.top += 3;      //30-pixel top border
            nccsp.rgrc0.bottom -= 5;    //4-pixel bottom (resize) border
            nccsp.rgrc0.left += 5;      //4-pixel left (resize) border
            nccsp.rgrc0.right -= 5;     //4-pixel right (resize) border

            //Set the structure back into memory
            System.Runtime.InteropServices.Marshal.StructureToPtr(nccsp, m.LParam, true);
        }
        else    //FALSE
        {
            //When FALSE, LPARAM Points to a RECT structure
            var clnRect = (RECT)System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typeof(RECT));

            //Like before, we're adjusting the rectangle...
            //Adding to the Top, Bottom, Left, and Right will size the client area.
            clnRect.top -= 4;      //30-pixel top border
            clnRect.bottom -= 4;    //4-pixel bottom (resize) border
            clnRect.left += 4;      //4-pixel left (resize) border
            clnRect.right -= 4;     //4-pixel right (resize) border

            //Set the structure back into memory
            System.Runtime.InteropServices.Marshal.StructureToPtr(clnRect, m.LParam, true);
        }

        //Return Zero
        m.Result = IntPtr.Zero;
    }

    WebView2 webView;
    bool initialized;
}
