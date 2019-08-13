namespace Cesium.Ion
{
    public class IonAuthArgs
    {
        public readonly IonStatus Status;
        public readonly string Token;

        public string Response { get; private set; }
        public bool DisposeAfter { get; set; }

        public IonAuthArgs(IonStatus status, string token)
        {
            Status = status;
            Token = token;

            Response = null;
            DisposeAfter = false;
        }

        public void WriteResponse(string Content)
        {
            Response = (Response ?? "") + Content;
        }

        public void ClearResponse()
        {
            Response = "";
        }
    }
}
