using System.Text;

namespace Discovery.SSDP.Messages
{
	/// <summary>
	/// Advertises the unavailability of a service
	/// </summary>
	internal class ByeMessage : MessageBase
	{
		public ByeMessage(Service service)
		{
			Service = service;
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public override byte[] ToArray()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("NOTIFY * HTTP/1.1\r\n");
			sb.AppendFormat("{0}: {1}:{2}\r\n", Discovery.SSDP.Headers.Host, Host, Port);
			sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.NotifiedServiceType, Service.ServiceType);
			sb.Append("NTS: ssdp:byebye\r\n");
			sb.AppendFormat("{0}: {1}\r\n",Discovery.SSDP.Headers.ServiceName, Service.UniqueServiceName);
			sb.Append("\r\n");

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
	}
}
