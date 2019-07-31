#region Namespaces
using System;
using System.Windows;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Cesium.Ion;
using Cesium.Ion.Revit.Properties;
using System.Windows.Media;
using System.Reflection;
#endregion

namespace Cesium.Ion.Revit
{

    public class App : IExternalApplication
    {
        public static RevitAuthServer Server { get; private set; } = null;

        private static PushButton UploadButton = null;
        private static PushButton LogoutButton = null;
        private static PushButton LoginButton = null;

        public static string IonToken
        {
            get => Settings.Default.IonToken ?? "";
            set
            {
                value = value ?? "";
                Settings.Default.IonToken = value;
                RefreshButtons();
                Settings.Default.Save();
            }
        }

        public static bool HasToken => IonToken.Length > 0;

        private static void RefreshButtons()
        {
            UploadButton.Enabled = HasToken;
            LogoutButton.Visible = HasToken;
            LoginButton.Visible = !HasToken;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource NewBitmapSource(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }


        public Result OnStartup(UIControlledApplication application)
        {
            Server = new RevitAuthServer();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string path = assembly.Location;
            string namespacePrefix = typeof(App).Namespace + ".";

            // Add a new ribbon panel
            RibbonPanel panel = application.CreateRibbonPanel(Resources.RibbonLabel);

            // Add cesium button
            PushButtonData cesiumButton = new PushButtonData(
                Resources.UploadButtonLabel,
                Resources.UploadButtonLabel,
                path,
                namespacePrefix + nameof(UploadCommand)
            )
            {
                ToolTip = Resources.UploadButtonDescription,
                LargeImage = NewBitmapSource(Resources.Cesium32),
                Image = NewBitmapSource(Resources.Cesium16)
            };

            // Add Sign-Out button
            PushButtonData logoutButton = new PushButtonData(
                Resources.LogoutButtonLabel,
                Resources.LogoutButtonLabel, // this will be changed later
                path,
                namespacePrefix + nameof(LogoutCommand)
            )
            {
                ToolTip = Resources.LogoutButtonDescription
            };

            PushButtonData loginButton = new PushButtonData(
                Resources.LoginButtonLabel,
                Resources.LoginButtonLabel,
                path,
                namespacePrefix + nameof(LoginCommand)
            )
            {
                ToolTip = Resources.LoginButtonDescription
            };

            // Add More Info button
            PushButtonData helpButton = new PushButtonData(
                Resources.HelpButtonLabel,
                Resources.HelpButtonLabel,
                path,
                namespacePrefix + nameof(HelpCommand)
            )
            {
                ToolTip = Resources.HelpButtonDescription
            };

            UploadButton = panel.AddItem(cesiumButton) as PushButton;
            panel.AddSeparator();
            var buttons = panel.AddStackedItems(logoutButton, loginButton, helpButton);
            LogoutButton = buttons[0] as PushButton;
            LoginButton = buttons[1] as PushButton;
            RefreshButtons();

            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication a)
        {
            Settings.Default.Save();
            Server.Dispose();
            Server = null;
            UploadButton = null;
            LogoutButton = null;
            LoginButton = null;

            return Result.Succeeded;
        }
    }
}
