using System;

namespace Discovery.SSDP
{
	public class DiscoveryException : Exception
	{
		public DiscoveryException()
			: base()
		{ }

		public DiscoveryException(string message)
			: base(message)
		{ }

		public DiscoveryException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
