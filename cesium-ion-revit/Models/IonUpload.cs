namespace Cesium.Ion.Revit
{
    using Newtonsoft.Json;

    public partial class IonUpload
    {
        [JsonProperty("assetMetadata")]
        public IonUploadAssetMetadata AssetMetadata { get; set; }

        [JsonProperty("uploadLocation")]
        public IonUploadLocation UploadLocation { get; set; }

        [JsonProperty("onComplete")]
        public IonOnComplete OnComplete { get; set; }
    }

}
