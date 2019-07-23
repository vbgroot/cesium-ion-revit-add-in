using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using Cesium.Ion.Revit.Properties;

namespace Cesium.Ion.Revit
{
    public class IonCreateAPI
    {
        private const string V = "";
        public readonly string TargetModel;
        public readonly string Token;
        public readonly string IonURL;

        public IonCreateAPI(string TargetModel, string Token, string IonURL = null)
        {
            IonURL = IonURL ?? Resources.IonAPIURL;
            this.TargetModel = TargetModel;
            this.Token = Token;
            this.IonURL = IonURL;
        }

        public async Task<IonUploadAPI> Create(string name, string description = "", string attribution = "", bool useWebP = false)
        {
            var endpoint = Url.Combine(IonURL, "v1", "assets");
            var data = new
            {
                name,
                description,
                attribution,
                options = new
                {
                    sourceType = "3D_MODEL",
                    textureFormat = useWebP ? "WEBP" : "AUTO"
                }
            };

            var result = await endpoint
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(data)
                .ReceiveJson<IonUpload>()
                .ConfigureAwait(false);

            return new IonUploadAPI(result, TargetModel, Token, IonURL);
        }
    }
}
