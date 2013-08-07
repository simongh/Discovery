using System;
using System.Text;

namespace Discovery.SSDP.Messages
{
	/// <summary>
	/// Advertises the presence of a service
	/// </summary>
	internal class AliveMessage : MessageBase
	{

		public AliveMessage(Service service)
		{
			Service = service;
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public override byte[] ToArray()
		{
			if (string.IsNullOrEmpty(Service.ServiceType))
				throw new ArgumentNullException("ServiceType");
			if (string.IsNullOrEmpty(Service.UniqueServiceName))
				throw new ArgumentNullException("UniqueServiceName");

			StringBuilder sb = new StringBuilder();

			sb.Append("NOTIFY * HTTP/1.1\r\n");
			sb.AppendFormat("{0}: {1}:{2}\r\n", Discovery.SSDP.Headers.Host, Host, Port);
			sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.NotifiedServiceType, Service.ServiceType);
			sb.Append("NTS: ssdp:alive\r\n");
			sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.ServiceName, Service.UniqueServiceName);
			if (!string.IsNullOrEmpty(Service.Location))
				sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.Location, Service.Location);
			if (Service.Expiry != TimeSpan.Zero)
				sb.AppendFormat("{0}: no-cache=\"Ext\", max-age={1}\r\n", Discovery.SSDP.Headers.CacheControl, Service.Expiry.TotalSeconds);

			sb.Append("\r\n");

			if (!string.IsNullOrEmpty(Content))
			{
				sb.Append(Content);
				sb.Append("\r\n\r\n");
			}

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
	}
}
