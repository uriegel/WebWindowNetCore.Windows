using Microsoft.Web.WebView2.WinForms;
using WebWindowNetCore.Data;

namespace WebWindowNetCore.Windows;

class WebWindowForm : Form
{
    public WebWindowForm(WebViewSettings? settings) 
    {
        this.webView = new WebView2();
        ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
        this.SuspendLayout();
        // 
        // webView
        // 
        this.webView.AllowExternalDrop = true;
        this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.webView.CreationProperties = null;
        this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
        this.webView.Location = new System.Drawing.Point(0, 0);
        this.webView.Margin = new System.Windows.Forms.Padding(0);
        this.webView.Name = "webView";
        this.webView.Size = new System.Drawing.Size(798, 449);
        this.webView.TabIndex = 0;
        this.webView.ZoomFactor = 1D;
        this.webView.Source = new System.Uri(settings?.Url ?? "");
        // 
        // Form1
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //this.webView.Size = new System.Drawing.Size(799, 451);
        this.ClientSize = new System.Drawing.Size(800, 450);
        //this.Size = new System.Drawing.Size(600, 800);
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
    }

    WebView2 webView;
}