using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Unosquare.Labs.EmbedIO;

namespace Cesium.Ion
{
    public class IonAuthServer : IDisposable
    {
        private WebServer Server = null;
        public event EventHandler<IonAuthArgs> OnAuthListener;
        public IonAuthenticator Authenticator;

        public void Listen(IonAuthenticator authenticator)
        {
            if (Server != null)
            {
                throw new Exception("Server is already running! Must .Dispose() old server!");
            }
            this.Authenticator = authenticator ?? throw new ArgumentNullException("Authenticator annot be null");
            Server = new WebServer(Authenticator.RedirectPort);
            Server.WithLocalSession();
            Server.OnGet(HandleLoad);
            Server.RunAsync();
        }

        public async Task<bool> HandleLoad(IHttpContext context, CancellationToken ct)
        {
            IonAuthArgs args;

            try
            {
                var param = Url.ParseQueryParams(context.Request.Url.Query);
                var state = param["state"] as string;

                if (!(param["code"] is string authCode))
                {
                    // No Code Sent
                    args = new IonAuthArgs(IonStatus.DENIED, null);
                }
                else if (!Authenticator.IsTrusted(state))
                {
                    // Unkown Source
                    args = new IonAuthArgs(IonStatus.UNTRUSTED, null);
                }
                else
                {
                    string token = await Authenticator
                        .GetToken(authCode)
                        .ConfigureAwait(false);
                    args = new IonAuthArgs(IonStatus.SUCCESS, token);
                }

            }
            catch (Exception exception)
            {
                args = new IonAuthArgs(IonStatus.ERROR, null);
                Console.WriteLine(exception);
            }

            OnAuthListener(this, args);

            await context.HtmlResponseAsync(args.Response ?? "DONE");

            return true;
        }

        public void Dispose()
        {
            Server?.Dispose();
            Server = null;
            OnAuthListener = null;
        }
    }
}
