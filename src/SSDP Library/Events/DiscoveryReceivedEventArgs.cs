using System;
using System.Net;

namespace Discovery.SSDP.Events
{
	public class DiscoveryReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the senders end point
		/// </summary>
		public System.Net.IPEndPoint Sender { get; private set; }

		public Service Service { get; private set; }

		internal DiscoveryReceivedEventArgs(Messages.DiscoveryResponseMessage message)
		{
			Sender = new IPEndPoint(IPAddress.Parse(message.Host), message.Port);
			Service = message.Service;
		}
	}
}