namespace Cesium.Ion
{
    public struct IonAuthArgs
    {
        public readonly IonStatus Status;
        public readonly string Token;

        public string Response { get; private set; }

        public IonAuthArgs(IonStatus status, string token)
        {
            Status = status;
            Token = token;

            Response = null;
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
