using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Discovery.Ssdp.Agents
{
	public class ServerAgent : AgentBase
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
		public ServiceDescriptionCollection Services { get; private set; }

		public ServerAgent()
			: base()
		{
			Services = new ServiceDescriptionCollection();
		}

		protected override async Task HandleMessage(Messages.MessageBase message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return;

			msg.Host = sender.Address.ToString();
			msg.Port = sender.Port;

			if (msg is Messages.DiscoveryMessage && Services.FirstOrDefault(x => x.ServiceType == msg.Service.ServiceType) != null)
				await OnSearchReceived(new Events.SearchReceivedEventArgs((Messages.DiscoveryMessage)msg, sender));
		}

		/// <summary>
		/// Broadcast the presence of a service
		/// </summary>
		public Task Announce(int index)
		{
			var m = new Messages.AliveMessage(Services.ElementAt(index));
			return Send(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the presence of all services
		/// </summary>
		public Task AnnounceAll()
		{
			return Send(Services.Select(x => (Messages.MessageBase)new Messages.AliveMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of a service
		/// </summary>
		public Task Bye(int index)
		{
			var m = new Messages.ByeMessage(Services.ElementAt(index));
			return Send(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of all services
		/// </summary>
		public Task ByeAll()
		{
			return Send(Services.Select(x => (Messages.MessageBase)new Messages.ByeMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Raise the SearchReceived event
		/// </summary>
		/// <param name="e">arguments for the event</param>
		protected async Task OnSearchReceived(Events.SearchReceivedEventArgs e)
		{
			SearchReceived?.Invoke(this, e);

			var content = await OnSearchResponding(new Events.SearchRespondingEventArgs(Content, e.Sender));

			var response = new Messages.DiscoveryResponseMessage();
			response.Service = Services.FirstOrDefault(x => x.ServiceType == e.ServiceType);

			try
			{
				await DelaySearchResponse(e.MaxWaitTime);
				response.Content = content;
				await Send(response, e.Sender);

				await OnSearchResponded();
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
		protected Task<string> OnSearchResponding(Events.SearchRespondingEventArgs e)
		{
			SearchResponding?.Invoke(this, e);

			return Task.FromResult(e.Content);
		}

		/// <summary>
		/// Raise the SearchResponded event
		/// </summary>
		protected Task OnSearchResponded()
		{
			SearchResponded?.Invoke(this, EventArgs.Empty);

			return Task.FromResult(0);
		}

		private async Task DelaySearchResponse(int maxWaitTime)
		{
			var rnd = new Random(DateTime.Now.Second);
			await Task.Delay(rnd.Next(maxWaitTime) * 1000);
		}
	}
}