using System;
using System.Diagnostics;

namespace Cesium.Ion.Max3D
{
    public class IonMax3D : IDisposable
    {
        public static string IonToken
        {
            get => Properties.Settings.Default.IonToken;
            set => Properties.Settings.Default.IonToken = value;
        }

        public static bool HasToken
        {
            get => IonToken.Length > 0;
        }

        public IonAuthenticator Authenticator { get; private set; }
        public IonAuthServer Server { get; private set; }
        public string ClientID { get; private set; }

        public IonMax3D(string ClientID)
        {
            this.ClientID = ClientID;
        }

        public void Auth()
        {
            Authenticator = Authenticator ?? new IonAuthenticator(ClientID);
            Server = Server ?? new IonAuthServer();

            Server.OnCodeListener += Authenticator.AsHandler((Sender, Args) =>
            {
                IonToken = Args.Token;
                Dispose();
            });

            Process.Start(Authenticator.GetOAuthURL());
        }

        public void Dispose()
        {
            Authenticator = null;
            Server.Dispose();
            Server = null;
        }
    }
}
