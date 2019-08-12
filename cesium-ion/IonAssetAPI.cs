using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.Net.Http;
using System.IO;
using System;
using System.Threading;
using System.Net;

namespace Cesium.Ion
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

            using (new SecurityProtocolDisposable(SecurityProtocolType.Tls12))
            {
                return await endpoint
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(data)
                .ReceiveJson<IonUpload>()
                .ConfigureAwait(false);
            }
        }

        public Task<string> Upload(IonUpload Model, string TargetModel) => Upload(Model, TargetModel, null);

        public async Task<string> Upload(IonUpload Model, string TargetModel, EventHandler<UploadProgressArgs> Handler = null, CancellationToken CancelToken = default(CancellationToken), string ViewerURL = null)
        {
            ViewerURL = ViewerURL ?? Resources.IonURL;

            var config = Model.UploadLocation;
            var s3Config = new AmazonS3Config() { ServiceURL = config.Endpoint };

            using (var s3Client = new AmazonS3Client(config.AccessKey, config.SecretAccessKey, config.SessionToken, s3Config))
            {
                var transfer = new TransferUtility(s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = config.Bucket,
                    FilePath = TargetModel,
                    Key = Path.Combine(config.Prefix, "autodesk.fbx")
                };

                if (Handler != null)
                {
                    uploadRequest.UploadProgressEvent += Handler;
                }

                await transfer.UploadAsync(uploadRequest, CancelToken)
                    .ConfigureAwait(false);
            }

            var completeMeta = Model.OnComplete;
            using (new SecurityProtocolDisposable(SecurityProtocolType.Tls12))
            {
                await completeMeta.Url
                .ToString()
                .SetQueryParams((object)completeMeta.Fields)
                .WithOAuthBearerToken(Token)
                .SendAsync(new HttpMethod(completeMeta.Method))
                .WithSecurityProtocol(SecurityProtocolType.Tls12)
                .ConfigureAwait(false);
            }

            return Url.Combine(ViewerURL, "assets", Model.AssetMetadata.Id.ToString());
        }

    }
}
