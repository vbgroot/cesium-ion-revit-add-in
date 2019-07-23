#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Diagnostics;
using Cesium.Ion.Revit.Properties;
#endregion

namespace Cesium.Ion.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public string ExportFBX(Document document, View3D view, string FileName = "out")
        {
            var folder = GetTemporaryDirectory();
            var viewSet = new ViewSet();
            viewSet.Insert(view);
            var options = new FBXExportOptions
            {
                StopOnError = true
            };

            try
            {
                document.Export(folder, FileName, viewSet, options);
                return Path.Combine(folder, FileName + ".fbx");
            }
            catch (Autodesk.Revit.Exceptions.ExternalApplicationException ex)
            {
                Debug.Print("ExternalApplicationException " + ex.Message);
            }
            return null;
        }

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (App.Settings.IonToken.Length <= 0)
            {
                var server = App.Server;
                var auth = new IonAuthenticator();
                server.Listen(auth.RedirectPort);
                auth.OnAuthListener(server, (status, token) =>
                {
                    App.Settings.IonToken = token;
                    App.Settings.Save();
                    server.Dispose();
                    TaskDialog.Show("Token Set!", "Export was a sucess " + token);
                });
                auth.OpenBrowser();
                return Result.Cancelled;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            if (doc.ActiveView as View3D == null)
            {
                TaskDialog.Show("Export Failed!", "You must be in 3D view to export.");
                return Result.Cancelled;
            }

            var fbxPath = ExportFBX(doc, (View3D)doc.ActiveView);
            Debug.WriteLine(fbxPath);

            if (fbxPath == null)
            {
                TaskDialog.Show("Export Failed!", "Model generation crashed. See log for more information.");
                return Result.Cancelled;
            }

            TaskDialog.Show(fbxPath, fbxPath);

            var assetURL = fbxPath
                .Ion("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJiYWMzYWZmZS02ZGFmLTQ0MGMtYTI4YS01OGE2N2ZmNTFkZDgiLCJpZCI6MTEyMzUsInNjb3BlcyI6WyJhc2wiLCJhc3IiLCJhc3ciLCJnYyJdLCJpYXQiOjE1NjE0MTI1NDF9.t1VgegdY50IMt4ffuwnukzjLpxBYbuVj7LL4KTunoLw")
                .Create("Test Revit Upload").Result
                .Upload().Result;

            TaskDialog.Show("YES!", assetURL);

            return Result.Succeeded;
        }
    }
}
