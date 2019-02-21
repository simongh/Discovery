using System.Collections.Generic;

namespace Discovery.Ssdp.Messages
{
	public abstract class MessageBase
	{
		/// <summary>
		/// Gets or sets the service for the message
		/// </summary>
		public ServiceDescription Service { get; set; }

		/// <summary>
		/// Gets or sets the host the message is originating from
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets the port the message originates from
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Gets or sets an optional message body
		/// </summary>
		public string Content { get; set; }

		public IDictionary<string, string> Headers { get; private set; }

		public MessageBase()
		{
			Headers = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public abstract byte[] ToArray();
	}
}