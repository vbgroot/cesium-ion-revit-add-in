using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using Cesium.Ion.Revit.Properties;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.Net.Http;
using System.IO;

namespace Cesium.Ion.Revit
{
    public class IonAssetAPI
    {
        public readonly string Token;
        public readonly string IonURL;

        public IonAssetAPI(string Token, string IonURL = null)
        {
            IonURL = IonURL ?? Resources.IonAPIURL;
            this.Token = Token;
            this.IonURL = IonURL;
        }

        public async Task<IonUpload> Create(string name, string description = "", string attribution = "", bool useWebP = false)
        {
            var endpoint = Url.Combine(IonURL, "v1", "assets");
            var data = new
            {
                name,
                description,
                attribution,
                type = "3DTILES",
                options = new
                {
                    sourceType = "3D_MODEL",
                    textureFormat = useWebP ? "WEBP" : "AUTO"
                }
            };

            return await endpoint
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(data)
                .ReceiveJson<IonUpload>()
                .ConfigureAwait(false);
        }

        public async Task<string> Upload(IonUpload Model, string TargetModel, string ViewerURL = null)
        {
            ViewerURL = ViewerURL ?? Resources.IonURL;

            var config = Model.UploadLocation;
            var s3Config = new AmazonS3Config() { ServiceURL = config.Endpoint };

            using (var s3Client = new AmazonS3Client(config.AccessKey, config.SecretAccessKey, config.SessionToken, s3Config)) { 
                var transfer = new TransferUtility(s3Client);

                await transfer.UploadAsync(TargetModel, config.Bucket, Path.Combine(config.Prefix, "revit.fbx"))
                    .ConfigureAwait(false);
            }

            var completeMeta = Model.OnComplete;
            await completeMeta.Url
                .ToString()
                .SetQueryParams((object)completeMeta.Fields)
                .WithOAuthBearerToken(Token)
                .SendAsync(new HttpMethod(completeMeta.Method))
                .ConfigureAwait(false);

            return Url.Combine(ViewerURL, "assets", Model.AssetMetadata.Id.ToString());
        }

    }
}
