using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Unosquare.Labs.EmbedIO;

namespace Cesium.Ion
{
    public delegate string ResponseHandler(IonAuthStatus Status, string Code, string State);

    public delegate void CodeHandler(IonAuthStatus Status, string Code, string State);

    public class IonAuthServer : IDisposable
    {
        private WebServer Server = null;
        public CodeHandler OnCodeListener { private get; set; } = null;
        public ResponseHandler OnResponseListener { private get; set; } = null;

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

        public async Task<bool> HandleLoad(IHttpContext context, CancellationToken ct)
        {
            var url = context.Request.Url;
            var param = Url.ParseQueryParams(url.Query);
            var authStatus = IonAuthStatus.ERROR;

            var state = param["state"] as string;
            var authCode = param["code"] as string;
            authStatus = authCode != null ? IonAuthStatus.CODE : IonAuthStatus.DENIED;

            var html = OnResponseListener?.Invoke(authStatus, authCode, state) ?? "DONE";
            await context.HtmlResponseAsync(html);
            try
            {
                OnCodeListener?.Invoke(authStatus, authCode, state);
            }
            catch (Exception exception)
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
