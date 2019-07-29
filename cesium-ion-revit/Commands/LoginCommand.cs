#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cesium.Ion.Revit.Properties;
using System.Diagnostics;
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
            server.Listen(auth.RedirectPort);

            server.OnCodeListener = auth.AsHandler((status, token) =>
            {
                App.IonToken = token;
                server.Dispose();
            });

            server.OnResponseListener = (status, code, state) =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourcePath = $"{GetType().Namespace}.Resources.index.html";
                var authMessage = status == IonAuthStatus.CODE ? Resources.AuthSucceedHTML : Resources.AuthDeniedHTML;
                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    return new StreamReader(stream)
                        .ReadToEnd()
                        .Replace("%RES%", authMessage);
                }
            };

            auth.GetOAuthURL()
                .OpenBrowser();

            return Result.Succeeded;
        }
    }
}
