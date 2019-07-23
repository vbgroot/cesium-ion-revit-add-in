using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cesium.Ion.Revit.Properties;
using Flurl;
using Unosquare.Labs.EmbedIO;

namespace Cesium.Ion.Revit
{
    public delegate void CodeHandler(IonAuthStatus Status, string Code, string State);

    public class IonAuthServer : IDisposable
    {

        private WebServer Server = null;
        private CodeHandler CodeHandler = null;

        public void OnCodeListener(CodeHandler CodeHandler)
        {
            this.CodeHandler = CodeHandler;
        }

        public void Listen(int port)
        {
            if (Server != null)
            {
                return;
            }
            Server = new WebServer(port);
            Server.WithLocalSession();
            Server.OnGet(HandleLoad);
            Server.RunAsync();
        }

        public async Task<bool> HandleLoad(IHttpContext context, CancellationToken ct) {
            var url = context.Request.Url;
            var param = Url.ParseQueryParams(url.Query);
            var authStatus = IonAuthStatus.ERROR;

            string html;
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"{GetType().Namespace}.Resources.index.html";
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                html = new StreamReader(stream).ReadToEnd();
            }

            var authCode = param["code"] as string;
            authStatus = authCode != null ? IonAuthStatus.CODE : IonAuthStatus.DENIED;
            var authMessage = authStatus == IonAuthStatus.CODE ? Resources.AuthSucceedHTML : Resources.AuthDeniedHTML;
            await context.HtmlResponseAsync(html.Replace("%RES%", authMessage));
            try {
                CodeHandler?.Invoke(authStatus, authCode, param["state"] as string);
            } catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return true;
        }

        public void Dispose()
        {
            if (Server == null)
            {
                return;
            }

            Server.Dispose();
            Server = null;
        }
    }
}
