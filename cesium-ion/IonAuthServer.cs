using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Unosquare.Labs.EmbedIO;

namespace Cesium.Ion
{
    public delegate string ResponseHandler();
    public struct IonCodeArgs
    {
        public readonly IonStatus Status;
        public readonly string Code;
        public readonly string State;

        public IonCodeArgs(IonStatus status, string code, string state)
        {
            Status = status;
            Code = code;
            State = state;
        }
    }

    public class IonAuthServer : IDisposable
    {
        private WebServer Server = null;
        public event EventHandler<IonCodeArgs> OnCodeListener;
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

        protected virtual string GetResponse(IonStatus Status, string Code, string State)
        {
            return "DONE";
        }

        public async Task<bool> HandleLoad(IHttpContext context, CancellationToken ct)
        {
            var url = context.Request.Url;
            var param = Url.ParseQueryParams(url.Query);
            var authStatus = IonStatus.ERROR;

            var state = param["state"] as string;
            var authCode = param["code"] as string;
            authStatus = authCode != null ? IonStatus.CODE : IonStatus.DENIED;

            await context.HtmlResponseAsync(GetResponse(authStatus, authCode, state));

            try
            {
                OnCodeListener(this, new IonCodeArgs(authStatus, authCode, state));
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
