using System.Reflection;
using System.Text.Json;

namespace WebWindowNetCore;

public partial class WebForm : Form
{
    public WebForm(Configuration configuration, WebWindowBase.Settings settings, ISaveSettings saveSettings) 
    {
        this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
        ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
        this.SuspendLayout();
        // 
        // webView
        // 
        this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.webView.CreationProperties = null;
        this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
        this.webView.Location = new System.Drawing.Point(0, 0);
        this.webView.Margin = new System.Windows.Forms.Padding(0);
        this.webView.Name = "webView";
        this.webView.Size = new System.Drawing.Size(799, 451);
        this.webView.Source = new System.Uri(configuration.Url!, System.UriKind.Absolute);
        this.webView.TabIndex = 0;
        this.webView.ZoomFactor = 1D;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.webView.Size = new System.Drawing.Size(799, 451);
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Size = new System.Drawing.Size(settings.Width, settings.Height);
        if (settings.X != -1 && settings.Y != -1)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(settings.X, settings.Y);
        }
        if (settings.IsMaximized)
            this.WindowState = FormWindowState.Maximized;

        this.Controls.Add(this.webView);
        this.Name = "WebWindow";
        this.Text = configuration.Title;
        ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
        this.ResumeLayout(false);

        WebWindowBase.Settings recentSettings = settings;
        this.FormClosed += (s, e) =>
        {
            if (configuration.SaveWindowSettings == true)
            {
                var settings = this.WindowState != FormWindowState.Maximized 
                    ? new WebWindowBase.Settings(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height, this.WindowState == FormWindowState.Maximized)
                    : recentSettings with { IsMaximized = true };
                saveSettings.SaveSettings(settings);
            }
        };

        var assembly = Assembly.GetEntryAssembly();
        var stream = assembly!.GetManifestResourceStream("appicon");
        if (stream != null)
            this.Icon = new Icon(stream);

        this.Resize += (s, e) =>
        {
            if (configuration.SaveWindowSettings == true && this.WindowState != FormWindowState.Maximized)
                recentSettings = new WebWindowBase.Settings(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height, this.WindowState == FormWindowState.Maximized);
        };

    }
    Microsoft.Web.WebView2.WinForms.WebView2 webView;
}
