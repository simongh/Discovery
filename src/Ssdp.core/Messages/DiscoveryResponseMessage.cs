using System.Text;

namespace Discovery.Ssdp.Messages
{
	/// <summary>
	/// The response message to a discovery message
	/// </summary>
	internal class DiscoveryResponseMessage : MessageBase
	{
		public override byte[] ToArray()
		{
			Guard.NotNull(Service, nameof(Service));
			Guard.NotNullOrEmpty(Service.ServiceType, nameof(Service.ServiceType));
			Guard.NotNullOrEmpty(Service.UniqueServiceName, nameof(Service.UniqueServiceName));

			var sb = new StringBuilder();

			sb.Append("HTTP/1.1 200 OK\r\n");
			sb.Append("ext:\r\n");
			sb.AppendFormat("{0}: no-cache=\"ext\", max-age={1}\r\n", HeaderNames.CacheControl, Service.Expiry.TotalSeconds);
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.ServiceType, Service.ServiceType);
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.ServiceName, Service.UniqueServiceName);
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.Location, Service.Location);
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