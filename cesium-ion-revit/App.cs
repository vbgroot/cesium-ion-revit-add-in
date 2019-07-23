#region Namespaces
using System;
using System.Windows;
using System.Reflection;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Cesium.Ion.Revit.Properties;
using System.Windows.Media;
#endregion

namespace Cesium.Ion.Revit
{

    class App : IExternalApplication
    {
        public readonly static IonAuthServer Server = new IonAuthServer();

        public readonly static Settings Settings = Properties.Settings.Default;

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
            Assembly assembly = Assembly.GetExecutingAssembly();
            string path = assembly.Location;
            string namespacePrefix = typeof(App).Namespace + ".";

            // Add a new ribbon panel
            RibbonPanel panel = application.CreateRibbonPanel(Resources.RibbonLabel);

            // Add cesium button
            PushButtonData buttonData = new PushButtonData(
               Resources.UploadButtonLabel,
               Resources.UploadButtonLabel,
               path,
               namespacePrefix + nameof(Command)
            );
            PushButton button = panel.AddItem(buttonData) as PushButton;
            button.ToolTip = Resources.UploadButtonDescription;
            button.LargeImage = NewBitmapSource(Resources.Cesium32);
            button.Image = NewBitmapSource(Resources.Cesium16);

            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication a)
        {
            Settings.Save();

            return Result.Succeeded;
        }
    }
}
