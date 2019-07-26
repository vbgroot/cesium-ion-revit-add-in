namespace Cesium.Ion.Revit
{
    using System;
    using Newtonsoft.Json;

    public partial class IonUploadLocation
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("bucket")]
        public string Bucket { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("accessKey")]
        public string AccessKey { get; set; }

        [JsonProperty("secretAccessKey")]
        public string SecretAccessKey { get; set; }

        [JsonProperty("sessionToken")]
        public string SessionToken { get; set; }
    }

}
