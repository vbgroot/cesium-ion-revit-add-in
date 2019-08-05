#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cesium.Ion.Revit.Properties;
using System.IO;
using System.Reflection;
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
            server.Listen(auth);

            server.OnAuthListener += (Sender, Args) =>
            {
                server.Dispose();
                App.IonToken = Args.Token;

                var assembly = Assembly.GetExecutingAssembly();
                var resourcePath = $"{GetType().Namespace}.Resources.index.html";
                var authMessage = Args.Status == IonStatus.SUCCESS ? Resources.AuthSucceedHTML : Resources.AuthDeniedHTML;
                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    Args.ClearResponse();
                    Args.WriteResponse(new StreamReader(stream)
                        .ReadToEnd()
                        .Replace("%RES%", authMessage));
                }
            };

            auth.GetOAuthURL()
                .OpenBrowser();

            return Result.Succeeded;
        }
    }
}
