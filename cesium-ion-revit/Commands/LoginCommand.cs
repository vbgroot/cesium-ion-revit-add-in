#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cesium.Ion.Revit.Properties;
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
            var auth = new IonAuthenticator(Resources.IonClientID);
            server.Listen(auth.RedirectPort);

            server.OnCodeListener += auth.AsHandler((Sender, Args) =>
            {
                App.IonToken = Args.Token;
                server.Dispose();
            });

            auth.GetOAuthURL()
                .OpenBrowser();

            return Result.Succeeded;
        }
    }
}
