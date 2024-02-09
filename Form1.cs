using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;




namespace Facebook_desktop
{
    public partial class Form1 : Form
    {
        public WebView2 browser;

        // Store original window state to restore it later
        private FormBorderStyle _previousBorderStyle;
        private FormWindowState _previousWindowState;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Facebook";
            this.Icon = Properties.Resources.facebook_Icon; // Ensure this exists or comment out
            InitBrowser();
        }

        public async void InitBrowser()
        {
            browser = new WebView2();
            browser.Dock = DockStyle.Fill;
            this.Controls.Add(browser);

            string userDataFolder = Path.Combine(Path.GetTempPath(), "Facebook");

            var options = new CoreWebView2EnvironmentOptions();
            options.AreBrowserExtensionsEnabled = true;

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
            await browser.EnsureCoreWebView2Async(env);

            // --- FULLSCREEN LOGIC START ---
            browser.CoreWebView2.ContainsFullScreenElementChanged += (sender, args) =>
            {
                if (browser.CoreWebView2.ContainsFullScreenElement)
                {
                    // 1. Save current state
                    _previousBorderStyle = this.FormBorderStyle;
                    _previousWindowState = this.WindowState;

                    // 2. The "Jolt": Switch to Normal first if already Maximized
                    // This forces Windows to recognize the change in FormBorderStyle
                    this.WindowState = FormWindowState.Normal;

                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.TopMost = true;
                }
                else
                {
                    // Exiting Fullscreen
                    this.TopMost = false;
                    this.FormBorderStyle = _previousBorderStyle;
                    this.WindowState = _previousWindowState;
                }
            };
            // --- FULLSCREEN LOGIC END ---

            // Get the directory where your .exe is located
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine it with your extension folder name
            // Assuming the "uBlock0.chromium" folder is sitting right next to your .exe
            string extensionPath = Path.Combine(baseDirectory, "Bloker");

            try
            {
                await browser.CoreWebView2.Profile.AddBrowserExtensionAsync(extensionPath);
            }
            catch (Exception ex)
            {

            }

            browser.CoreWebView2.Navigate("https://facebook.com/");
        }
    }
}



    
    
    