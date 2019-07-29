using Newtonsoft.Json;
using System;

namespace Cesium.Ion
{
    public partial class IonUploadAssetMetadata : IonAssetMetadata
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("dateAdded")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("percentComplete")]
        public long PercentComplete { get; set; }

    }
}
