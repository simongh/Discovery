using System;

namespace Discovery.SSDP.Events
{
	/// <summary>
	/// Contains details of the message when a search request is recieved
	/// </summary>
	public class SearchReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the body of the search request
		/// </summary>
		public string ReceivedContent { get; private set; }

		/// <summary>
		/// Gets the service type being searched for
		/// </summary>
		public string ServiceType { get; private set; }

		/// <summary>
		/// Gets the senders end point
		/// </summary>
		public System.Net.IPEndPoint Sender { get; private set; }

		public int MaxWaitTime { get; private set; }

		internal SearchReceivedEventArgs(Messages.DiscoveryMessage message, System.Net.IPEndPoint sender)
		{
			ServiceType = message.Service.ServiceType;
			MaxWaitTime = message.MaxWaitTime;
			ReceivedContent = message.Content;
			Sender = sender;
		}
	}
}