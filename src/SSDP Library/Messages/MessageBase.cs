namespace Discovery.SSDP.Messages
{
	/// <summary>
	/// Base class for messages
	/// </summary>
	public abstract class MessageBase
	{
		/// <summary>
		/// Gets or sets the service forthe message
		/// </summary>
		public Service Service { get; set; }

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

		public System.Net.WebHeaderCollection Headers { get; private set; }

		public MessageBase()
		{
			Headers = new System.Net.WebHeaderCollection();
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public abstract byte[] ToArray();
	}
}