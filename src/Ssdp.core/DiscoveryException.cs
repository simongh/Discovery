using System;

namespace Discovery.Ssdp
{
	public class DiscoveryException : Exception
	{
		public int StatusCode { get; internal set; }

		public DiscoveryException()
			: base()
		{ }

		public DiscoveryException(int statusCode)
			: base()
		{
			StatusCode = statusCode;
		}

		public DiscoveryException(string message)
			: base(message)
		{ }

		public DiscoveryException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}