using Newtonsoft.Json;
using System;

namespace Cesium.Ion
{
    public partial class IonOnComplete
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("fields")]
        public dynamic Fields { get; set; }
    }

}
