#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cesium.Ion.Revit.Properties;
#endregion

namespace Cesium.Ion.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class UploadCommand : IExternalCommand
    {   
 
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            if (doc.ActiveView as View3D == null)
            {
                TaskDialog.Show(Resources.ErrorViewportTitle, Resources.ErrorViewportDescription);
                return Result.Cancelled;
            }

            new UploadDialog(doc, new IonAssetAPI(App.IonToken))
                .ShowDialog();

            return Result.Succeeded;
        }
    }
}
