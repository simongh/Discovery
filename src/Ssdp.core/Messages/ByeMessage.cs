using System.Text;

namespace Discovery.Ssdp.Messages
{
	/// <summary>
	/// Advertises the unavailability of a service
	/// </summary>
	internal class ByeMessage : MessageBase
	{
		public ByeMessage(ServiceDescription service)
		{
			Service = service;
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public override byte[] ToArray()
		{
			Guard.NotNull(Service, nameof(Service));
			Guard.NotNullOrEmpty(Service.ServiceType, nameof(Service.ServiceType));
			Guard.NotNullOrEmpty(Service.UniqueServiceName, nameof(Service.UniqueServiceName));

			var sb = new StringBuilder();

			sb.Append("NOTIFY * HTTP/1.1\r\n");
			sb.AppendFormat("{0}: {1}:{2}\r\n", HeaderNames.Host, Host, Port);
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.NotifiedServiceType, Service.ServiceType);
			sb.Append("NTS: ssdp:byebye\r\n");
			sb.AppendFormat("{0}: {1}\r\n", HeaderNames.ServiceName, Service.UniqueServiceName);
			sb.Append("\r\n");

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
	}
}