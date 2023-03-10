using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using WebWindowNetCore.Data;

namespace WebWindowNetCore.Windows;

class WebWindowForm : Form
{
    public WebWindowForm(WebViewSettings? settings, string appDataPath) 
    {
        webView = new WebView2();
        ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
        SuspendLayout();
        // 
        // webView
        // 
        this.webView.AllowExternalDrop = true;
        this.webView.CreationProperties = null;
        this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
        this.webView.Location = new System.Drawing.Point(0, 0);
        this.webView.Margin = new System.Windows.Forms.Padding(0);
        this.webView.Name = "webView";
        this.webView.Dock = DockStyle.Fill;
        this.webView.TabIndex = 0;
        this.webView.ZoomFactor = 1D;
        // 
        // Form1
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.WindowState = FormWindowState.Minimized;
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(80, 60);
        // if (settings.x != -1 && settings.y != -1)
        // {
        //     this.StartPosition = FormStartPosition.Manual;
        //     this.Location = new System.Drawing.Point(settings.x, settings.y);
        // }
        // if (settings.isMaximized)
        //     this.WindowState = FormWindowState.Maximized;

        this.Controls.Add(this.webView);
        this.Name = "WebWindow";

        this.Text = settings?.Title;
        ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
        this.ResumeLayout(false);

//        WebWindowBase.Settings recentSettings = settings;
        // this.FormClosed += (s, e) =>
        // {
        //     if (configuration.SaveWindowSettings == true)
        //     {
        //         var settings = this.WindowState != FormWindowState.Maximized 
        //             ? new WebWindowBase.Settings(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height, this.WindowState == FormWindowState.Maximized)
        //             : recentSettings with { isMaximized = true };
        //         saveSettings.SaveSettings(settings);
        //     }
        // };

        // this.Resize += (s, e) =>
        // {
        //     if (configuration.SaveWindowSettings == true && this.WindowState != FormWindowState.Maximized)
        //         recentSettings = new WebWindowBase.Settings(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height, this.WindowState == FormWindowState.Maximized);
        // };

        //this.webView.Size = new System.Drawing.Size(settings!.Width, settings!.Height);
    
        StartWebviewInit();

        async void StartWebviewInit()
        {
            var enf = await  CoreWebView2Environment.CreateAsync(null, appDataPath);
            await webView.EnsureCoreWebView2Async(enf);
            webView.Source = new System.Uri(settings?.Url ?? "");
            //webView.CoreWebView2.AddHostObjectToScript("WV_File", new WV_File(this));
            ClientSize = new System.Drawing.Size(settings?.Width ?? 800, settings?.Height ?? 600);
            WindowState = FormWindowState.Normal;
            Visible = true;    
        }
    }

    WebView2 webView;
}
