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
		public event Func<object, Events.SearchReceivedEventArgs, Task> SearchReceived;

		/// <summary>
		/// Indicates a discovery response is being sent
		/// </summary>
		public event Func<object, Events.SearchRespondingEventArgs, Task> SearchResponding;

		/// <summary>
		/// Indicates a discovery response was sent
		/// </summary>
		public event Func<object, EventArgs, Task> SearchResponded;

		/// <summary>
		/// Gets the services being advertised
		/// </summary>
		public ServiceDescriptionCollection Services { get; private set; }

		public ServerAgent()
			: base()
		{
			Services = new ServiceDescriptionCollection();
		}

		protected override Task HandleMessageAsync(Messages.MessageBase message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg != null)
			{
				msg.Host = sender.Address.ToString();
				msg.Port = sender.Port;

				if (msg is Messages.DiscoveryMessage && Services.FirstOrDefault(x => x.ServiceType == msg.Service.ServiceType) != null)
					return OnSearchReceivedAsync(new Events.SearchReceivedEventArgs((Messages.DiscoveryMessage)msg, sender));
			}

			return Task.FromResult(0);
		}

		/// <summary>
		/// Broadcast the presence of a service
		/// </summary>
		public Task AnnounceAsync(int index)
		{
			var m = new Messages.AliveMessage(Services.ElementAt(index));
			return SendAsync(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the presence of all services
		/// </summary>
		public Task AnnounceAllAsync()
		{
			return SendAsync(Services.Select(x => (Messages.MessageBase)new Messages.AliveMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of a service
		/// </summary>
		public Task ByeAsync(int index)
		{
			var m = new Messages.ByeMessage(Services.ElementAt(index));
			return SendAsync(m, new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Broadcast the shutdown of all services
		/// </summary>
		public Task ByeAllAsync()
		{
			return SendAsync(Services.Select(x => (Messages.MessageBase)new Messages.ByeMessage(x)), new IPEndPoint(DiscoveryAddress, Port));
		}

		/// <summary>
		/// Raise the SearchReceived event
		/// </summary>
		/// <param name="e">arguments for the event</param>
		protected async Task OnSearchReceivedAsync(Events.SearchReceivedEventArgs e)
		{
			await OnEventHandlerAsync(SearchReceived, e).ConfigureAwait(false);

			var content = await OnSearchRespondingAsync(new Events.SearchRespondingEventArgs(Content, e.Sender)).ConfigureAwait(false);

			var response = new Messages.DiscoveryResponseMessage();
			response.Service = Services.FirstOrDefault(x => x.ServiceType == e.ServiceType);

			try
			{
				await DelaySearchResponseAsync(e.MaxWaitTime).ConfigureAwait(false);
				response.Content = content;
				await SendAsync(response, e.Sender).ConfigureAwait(false);

				await OnSearchRespondedAsync().ConfigureAwait(false);
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
		protected async Task<string> OnSearchRespondingAsync(Events.SearchRespondingEventArgs e)
		{
			await OnEventHandlerAsync(SearchResponding, e).ConfigureAwait(false);
			return e.Content;
		}

		/// <summary>
		/// Raise the SearchResponded event
		/// </summary>
		protected Task OnSearchRespondedAsync()
		{
			return OnEventHandlerAsync(SearchResponded, EventArgs.Empty);
		}

		private Task DelaySearchResponseAsync(int maxWaitTime)
		{
			var rnd = new Random(DateTime.Now.Second);
			return Task.Delay(rnd.Next(maxWaitTime) * 1000);
		}
	}
}