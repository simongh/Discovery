using System;
using System.Text;

namespace Discovery.SSDP.Messages
{
    /// <summary>
    /// The response message to a discovery message
    /// </summary>
    internal class DiscoveryResponseMessage : MessageBase
    {
        public override byte[] ToArray()
        {
            if (string.IsNullOrEmpty(Service.ServiceType))
                throw new ArgumentNullException("ServiceType");
            if (string.IsNullOrEmpty(Service.UniqueServiceName))
                throw new ArgumentNullException("UniqueServiceName");

            StringBuilder sb = new StringBuilder();

            sb.Append("HTTP/1.1 200 OK\r\n");
            sb.Append("ext:\r\n");
            sb.AppendFormat("{0}: no-cache=\"ext\", max-age={1}\r\n",Discovery.SSDP.Headers.CacheControl, Service.Expiry.TotalSeconds);
            sb.AppendFormat("{0}: {1}\r\n",Discovery.SSDP.Headers.ServiceType, Service.ServiceType);
            sb.AppendFormat("{0}: {1}\r\n",Discovery.SSDP.Headers.ServiceName, Service.UniqueServiceName);
            sb.AppendFormat("{0}: {1}\r\n",Discovery.SSDP.Headers.Location, Service.Location);
            sb.Append("\r\n");

            if (Content != null)
            {
                sb.Append(Content);
                sb.Append("\r\n\r\n");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
