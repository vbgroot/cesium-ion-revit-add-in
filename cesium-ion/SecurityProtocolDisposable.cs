using System;
using System.Net;

namespace Cesium.Ion
{
    class SecurityProtocolDisposable : IDisposable
    {
        readonly SecurityProtocolType OldProtocol;

        public SecurityProtocolDisposable(SecurityProtocolType Protocol)
        {
            OldProtocol |= ServicePointManager.SecurityProtocol;
            ServicePointManager.SecurityProtocol = Protocol;
        }

        public void Dispose()
        {
            ServicePointManager.SecurityProtocol = OldProtocol;
        }
    }
}
