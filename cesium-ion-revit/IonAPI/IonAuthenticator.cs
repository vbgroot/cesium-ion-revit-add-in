using System;
using System.Linq;
using Flurl;
using Flurl.Http;
using Cesium.Ion.Revit.Properties;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cesium.Ion.Revit
{
    public delegate void AuthHandler(IonAuthStatus Status, string Token);

    public class IonAuthenticator
    {

        public readonly static string CodeChallengeMethod = "S256";

        public string IonURL { get; set; }
        public string IonAPIURL { get; set; }
        public string ClientID { get; set; }
        public string CodeVerifier { get; }
        public string State { get; }
        public IonScope[] IonScopes { get; set; }
        public int RedirectPort { get; set; }


        public IonAuthenticator(string IonURL = null, string IonAPIURL = null, string ClientID = null, string CodeVerifier = null, string State = null, IonScope[] scope = null, int RedirectPort = -1)
        {
            this.IonURL = IonURL ?? Resources.IonURL;
            this.IonAPIURL = IonAPIURL ?? Resources.IonAPIURL;
            this.ClientID = ClientID ?? Resources.IonClientID;
            this.CodeVerifier = CodeVerifier ?? Utils.RandomString(32);
            this.State = State ?? Utils.RandomString(32);
            this.IonScopes = scope ?? new IonScope[] { IonScope.WRITE };
            this.RedirectPort = RedirectPort <= 0 ? Int32.Parse(Resources.IonRedirectPort) : RedirectPort;
        }

        public string RedirectURI
        {
            get => $"http://localhost:{RedirectPort}/";
        }

        public string CodeChallenge
        {
            get => CodeVerifier
                .SHA256()
                .Base64Encode(URLEncoded: true);
        }

        public string GetOAuthURL()
        {
            var serializedScope = IonScopes
                .Select(scope => scope.ToRest())
                .Aggregate("", (accum, scope) => scope + "," + accum)
                .TrimEnd(',');

            Console.WriteLine(serializedScope);

            return IonURL
                .AppendPathSegment("oauth")
                .SetQueryParams(new
                {
                    response_type = "code",
                    client_id = ClientID,
                    redirect_uri = RedirectURI,
                    scope = serializedScope,
                    state = State,
                    code_challenge = CodeChallenge,
                    code_challenge_method = CodeChallengeMethod
                });
        }

        public bool IsTrusted(string state)
        {
            return State == state;
        }

        public async Task<string> GetToken(String code)
        {
            var body = new
            {
                grant_type = "authorization_code",
                code,
                client_id = ClientID,
                redirect_uri = RedirectURI,
                code_verifier = CodeVerifier
            };

            Console.WriteLine(JsonConvert.SerializeObject(body));

            var response = await IonAPIURL
                .AppendPathSegments("oauth", "token")
                .WithHeader("Accept", "application/json")
                .PostJsonAsync(body)
                .ReceiveJson<TokenResponse>();

            return response.AccessToken;
        }

        public string OpenBrowser()
        {
            var authURL = GetOAuthURL();
            authURL.OpenBrowser();
            return authURL;
        }

        public CodeHandler AsHandler(AuthHandler Handler)
        {
            return (IonAuthStatus Status, string Code, string State) =>
            {
                if (Status != IonAuthStatus.CODE)
                {
                    Handler(Status, null);
                    Console.Write("No code sent!");
                    return;
                }

                if (!IsTrusted(State))
                {
                    Handler(IonAuthStatus.ERROR, null);
                    Console.Write("Cannot trust source! Force exiting");
                    return;
                }

                string token;
                try
                {
                    token = GetToken(Code).Result;
                }
                catch (Exception exception)
                {
                    Handler(IonAuthStatus.DENIED, null);
                    Console.WriteLine(exception);
                    return;
                }

                Handler(IonAuthStatus.CODE, token);
            };
        }

        public void OnAuthListener(IonAuthServer Server, AuthHandler Handler)
        {
            Server.OnCodeListener(AsHandler(Handler));
        }
       
    }

    public partial class TokenResponse {
        [JsonProperty("access_token")]
        public string AccessToken;
    }
}
