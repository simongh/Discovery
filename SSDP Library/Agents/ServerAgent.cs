using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Discovery.SSDP.Agents
{
	/// <summary>
	/// Handles server discovery activity 
	/// </summary>
	public class ServerAgent : AgentBase, IServerAgent
	{
		/// <summary>
		/// Indicates a discovery message has been received
		/// </summary>
		public event EventHandler<Events.SearchReceivedEventArgs> SearchReceived;
		/// <summary>
		/// Indicates a discovery response is being sent
		/// </summary>
		public event EventHandler<Events.SearchRespondingEventArgs> SearchResponding;
		/// <summary>
		/// Indicates a discovery response was sent
		/// </summary>
		public event EventHandler SearchResponded;

		/// <summary>
		/// Gets the services being advertised
		/// </summary>
		public ServiceCollection Services
		{
			get;
			private set;
		}

		public ServerAgent()
			: base()
		{
			Services = new ServiceCollection();
		}

		protected override void HandleMessage(object message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return;

			if (msg is Messages.DiscoveryMessage && Services.FirstOrDefault(x => x.ServiceType == msg.Service.ServiceType) != null)
				OnSearchReceived(new Events.SearchReceivedEventArgs((Messages.DiscoveryMessage)msg, sender));
		}

		/// <summary>
		/// Broadcast the presence of a service
		/// </summary>
		public void Announce(int index)
		{
			var m = new Messages.AliveMessage(Services.ElementAt(index));
			Send(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the presence of all services
		/// </summary>
		public void AnnounceAll()
		{
			Send(Services.Select(x => (Messages.MessageBase)new Messages.AliveMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of a service
		/// </summary>
		public void Bye(int index)
		{
			var m = new Messages.ByeMessage(Services.ElementAt(index));
			Send(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of all services
		/// </summary>
		public void ByeAll()
		{
			Send(Services.Select(x => (Messages.MessageBase)new Messages.ByeMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		///// <summary>
		///// Broadcast a message
		///// </summary>
		///// <param name="message">message to be broadcast</param>
		//private void Broadcast(Messages.MessageBase message)
		//{
		//    IPEndPoint e = new IPEndPoint(DiscoveryAddress, Port);
		//    Send(message, e);
		//}

		/// <summary>
		/// Raise the SearchReceived event
		/// </summary>
		/// <param name="e">arguments for the event</param>
		protected void OnSearchReceived(Events.SearchReceivedEventArgs e)
		{
			if (SearchReceived != null)
				SearchReceived(this, e);

			var content = OnSearchResponding(new Events.SearchRespondingEventArgs(Content, e.Sender));

			var response = new Messages.DiscoveryResponseMessage();
			response.Service = Services.FirstOrDefault(x => x.ServiceType == e.ServiceType);

			try
			{
				DelaySearchResponse(e.MaxWaitTime);
				response.Content = content;
				Send(response, e.Sender);

				OnSearchResponded();
			}
			catch (Exception ex)
			{
				throw new DiscoveryException("Exception sending search response", ex);
			}
		}

		/// <summary>
		/// Raise the SearchResponding event
		/// </summary>
		/// <param name="e">arguments for the event</param>
		protected string OnSearchResponding(Events.SearchRespondingEventArgs e)
		{
			if (SearchResponding != null)
				SearchResponding(this, e);

			return e.Content;
		}

		/// <summary>
		/// Raise the SearchResponded event
		/// </summary>
		protected void OnSearchResponded()
		{
			if (SearchResponded == null)
				return;

			SearchResponded(this, EventArgs.Empty);
		}

		private void DelaySearchResponse(int maxWaitTime)
		{
			var rnd = new Random(DateTime.Now.Second);
			System.Threading.Thread.Sleep(rnd.Next(maxWaitTime) * 1000);
		}
	}
}
