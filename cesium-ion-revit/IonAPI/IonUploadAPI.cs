using Amazon.S3;
using Amazon.S3.Transfer;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using System.Net.Http;
using Cesium.Ion.Revit.Properties;

namespace Cesium.Ion.Revit
{
    public class IonUploadAPI : IonCreateAPI
    {

        public readonly IonUpload Model;
        private readonly IAmazonS3 s3Client;
        private bool IsUploaded = false;

        public IonUploadAPI(IonUpload Model, string TargetModel, string Token, string IonURL) : base(TargetModel, Token, IonURL)
        {
            this.Model = Model;
            var config = Model.UploadLocation;
            this.s3Client = new AmazonS3Client(config.AccessKey, config.SecretAccessKey, config.SessionToken);
        }

        public async Task<string> Upload(string ViewerURL = null)
        {
            ViewerURL = ViewerURL ?? Resources.IonURL;

            if (!IsUploaded) { 
                var transfer = new TransferUtility(s3Client);
                var bucket = Model.UploadLocation.Bucket;

                await transfer.UploadAsync(TargetModel, bucket)
                    .ConfigureAwait(false);

                var completeMeta = Model.OnComplete;
                await completeMeta.Url
                    .ToString()
                    .SetQueryParams((object) completeMeta.Fields)
                    .WithOAuthBearerToken(Token)
                    .SendAsync(new HttpMethod(completeMeta.Method))
                    .ConfigureAwait(false);
            }

            IsUploaded = true;
            return Url.Combine(ViewerURL, "assets", Model.AssetMetadata.Id.ToString());
        }


    }
}
