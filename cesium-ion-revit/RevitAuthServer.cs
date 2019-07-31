using Cesium.Ion.Revit.Properties;
using System.IO;
using System.Reflection;

namespace Cesium.Ion.Revit
{
    public class RevitAuthServer : IonAuthServer
    {
        protected override string GetResponse(IonStatus Status, string Code, string State)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"{GetType().Namespace}.Resources.index.html";
            var authMessage = Status == IonStatus.CODE ? Resources.AuthSucceedHTML : Resources.AuthDeniedHTML;
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                return new StreamReader(stream)
                    .ReadToEnd()
                    .Replace("%RES%", authMessage);
            }
        }
    }
}
