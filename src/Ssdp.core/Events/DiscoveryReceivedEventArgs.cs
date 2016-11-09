using System;
using System.Net;

namespace Discovery.Ssdp.Events
{
	public class DiscoveryReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the senders end point
		/// </summary>
		public IPEndPoint Sender { get; private set; }

		public ServiceDescription Service { get; private set; }

		internal DiscoveryReceivedEventArgs(Messages.DiscoveryResponseMessage message)
		{
			Sender = new IPEndPoint(IPAddress.Parse(message.Host), message.Port);
			Service = message.Service;
		}
	}
}