using WebWindowNetCore.Windows;

WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("WebView Test")
    .SaveBounds()
    .Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    

// TODO set initial bounds
// TODO SaveBounds
// TODO webviewhanlder.dll from resource
// TODO env folder
// TODO: the app is a program with resources such as web site, icon, native webViewHandler
/*
using System.Runtime.InteropServices;

namespace WebView2Form;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        StartWebviewInit();

        async void StartWebviewInit()
        {
            var enf =await  Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, @"h:\log\k�sch");
            await webView21.EnsureCoreWebView2Async(enf);
            webView21.CoreWebView2.AddHostObjectToScript("WV_File", new WV_File(this));
        }
    }

    [ComVisible(true)]
    public class WV_File
    {
        public WV_File(Form1 parent)
        {
            this.parent = parent;
        }
        public void DragDropFile()
        {
            var overlay = new Overlay(parent);
            overlay.Top = parent.Top;
            overlay.Left = parent.Left;
            overlay.Width= parent.Width / 2;
            overlay.Height = parent.Height / 2;
            try
            {
                parent.Controls.Add(overlay);
            }
            catch (Exception e)
            {

            }
            overlay.Show(parent);
        }

        Form1 parent;
    }
}

*/
/*
Overlay


namespace WebView2Form;
public partial class Overlay : Form
{
    public Overlay(Form parent) 
    {
        InitializeComponent();
    }
}
using System.Runtime.InteropServices;

namespace WebView2Form;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        MessageBox.Show("Hallo");
        LoadLibrary(@"h:\WebView2Loader.dll");
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibrary(string filename);
}


<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
	<RuntimeIdentifier>win-x64</RuntimeIdentifier>
	<SelfContained>false</SelfContained>
	<PublishSingleFile Condition="'$(Configuration)' == 'Release'">true</PublishSingleFile> 
	  <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.1587.40" />
  </ItemGroup>

</Project>



namespace WebView2Form;

partial class Overlay
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Overlay";
            this.Opacity = 0.3D;
            this.Text = "Overlay";
            this.ResumeLayout(false);

    }

    #endregion
}*/