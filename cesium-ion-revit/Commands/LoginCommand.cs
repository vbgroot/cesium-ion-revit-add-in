#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace Cesium.Ion.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class LoginCommand : IExternalCommand
    {   
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            var server = App.Server;
            var auth = new IonAuthenticator();
            server.Listen(auth.RedirectPort);
            auth.OnAuthListener(server, (status, token) =>
            {
                App.IonToken = token;
                server.Dispose();
            });
            auth.OpenBrowser();
            return Result.Succeeded;
        }
    }
}
