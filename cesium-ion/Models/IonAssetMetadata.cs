using Newtonsoft.Json;

namespace Cesium.Ion
{
    public partial class IonAssetMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("attribution")]
        public string Attribution { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

    }
}
