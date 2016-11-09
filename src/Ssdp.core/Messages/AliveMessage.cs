using System;
using System.Text;

namespace Discovery.Ssdp.Messages
{
	internal class AliveMessage : MessageBase
	{
		public AliveMessage(ServiceDescription service)
		{
			Service = service;
		}

		public override byte[] ToArray()
		{
			Guard.NotNull(Service, nameof(Service));
			Guard.NotNullOrEmpty(Service.ServiceType, nameof(Service.ServiceType));
			Guard.NotNullOrEmpty(Service.UniqueServiceName, nameof(Service.UniqueServiceName));

			var sb = new StringBuilder();

			sb.Append("NOTIFY * HTTP/1.1\r\n");
			sb.AppendFormat("{0}: {1}:{2}\r\n", HeaderNames.Host, Host, Port);
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.NotifiedServiceType, Service.ServiceType);
			sb.Append("NTS: ssdp:alive\r\n");
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.ServiceName, Service.UniqueServiceName);
			if (!string.IsNullOrEmpty(Service.Location))
				sb.AppendFormat("{0}: {1}\r\n", HeaderNames.Location, Service.Location);
			if (Service.Expiry != TimeSpan.Zero)
				sb.AppendFormat("{0}: no-cache=\"Ext\", max-age={1}\r\n", HeaderNames.CacheControl, Service.Expiry.TotalSeconds);

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