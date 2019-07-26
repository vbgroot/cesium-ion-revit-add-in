using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Cesium.Ion.Revit
{

    public partial class UploadDialog : UserControl
    {
        public string IonToken { get; set; }
        public string OutputPath { get; }
        public Window Window { get; private set; }

        public Document Document { get; set; }

        public UploadDialog(Document Document, string IonToken)
        {
            InitializeComponent();
            this.Document = Document;
            this.IonToken = IonToken;
        }

        public Window Show(string Title)
        {
            Window = Window ?? new Window
            {
                Title = Title,
                Content = this,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            Window.ShowDialog();

            return Window;
        }

        public void Close() {
            Window.Close();
            Window = null;
        }

        public string ExportFBX( View3D view, string FileName = "out")
        {
            var folder = Utils.GetTemporaryDirectory();
            var viewSet = new ViewSet();
            viewSet.Insert(view);
            var options = new FBXExportOptions
            {
                StopOnError = true
            };

            try
            {
                Document.Export(folder, FileName, viewSet, options);
                return Path.Combine(folder, FileName + ".fbx");
            }
            catch (Autodesk.Revit.Exceptions.ExternalApplicationException ex)
            {
                Debug.Print("ExternalApplicationException " + ex.Message);
            }
            return null;
        }


        private void OnUploadClick(object sender, RoutedEventArgs e)
        {
            if (NameInput.Text.Length == 0)
            {
                TaskDialog.Show("Missing Fields!", "Please set \"Name\" field to start upload!");
                Window.Focus();
                return;
            }

            var fbxPath = ExportFBX((View3D) Document.ActiveView);
            Debug.WriteLine(fbxPath);

            if (fbxPath == null)
            {
                TaskDialog.Show("Export Failed!", "Model generation crashed. See log for more information.");
                Close();
                return;
            }

            var ionAPI = new IonAssetAPI(IonToken);
            var ionAsset = ionAPI.Create(
                NameInput.Text,
                DescriptionInput.Text,
                AttributionInput.Text,
                UseWebPCheckbox.IsChecked ?? false
            ).Result;
            var assetURL = ionAPI.Upload(ionAsset, fbxPath).Result;

            Close();
            assetURL.OpenBrowser();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
