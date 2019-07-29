using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Cesium.Ion.Revit
{
    public partial class UploadDialog : Window
    {
        public readonly IonAssetAPI Ion;
        private readonly Document Document;

        public UploadDialog(Document Document, IonAssetAPI Ion)
        {
            InitializeComponent();
            this.Document = Document;
            this.Ion = Ion;
        }

        public string ExportFBX(string FileName = "out")
        {
            View3D view = (View3D) Document.ActiveView;
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
                Focus();
                return;
            }

            var fbxPath = ExportFBX();

            if (fbxPath == null)
            {
                TaskDialog.Show("Export Failed!", "Model generation crashed. See log for more information.");
                Close();
                return;
            }

            var ionAsset = Ion.Create(
                NameInput.Text,
                DescriptionInput.Text,
                AttributionInput.Text,
                UseWebPCheckbox.IsChecked ?? false
            ).Result;

            Close();

            new ProgressDialog(Ion, ionAsset, fbxPath)
                 .Show();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
