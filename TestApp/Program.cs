using WebWindowNetCore.Windows;

static class Program
{
    [STAThread]
    static void Main()
    {
        WebViewApp.Run();
        // var setting = new Configuration(FullscreenEnabled: true);
        // // TODO: App.run();
    }
}

// TODO: Structure:
// TODO: WebWindoNetCore is a Dll with a webView
// TODO: the app is a program with resources such as web site, icon, native webViewHandler
// TODO: Builder concept
// TODO: Nuget with platform dependant references
// TODO: Windows Make Sln WebWindowNetCore.Windows with Tester
// TODO: Windows Make Nuget package 
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

*/