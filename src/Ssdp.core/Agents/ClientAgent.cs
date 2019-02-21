using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Discovery.Ssdp.Agents
{
	public class ClientAgent : AgentBase
	{
		private int _minDiscoveryCount;
		private int _maxWaitTime;

		public event Func<object, Events.ByeReceivedEventArgs, Task> ByeReceived;

		public event Func<object, Events.AnnounceEventArgs, Task> AnnounceReceived;

		public event Func<object, Events.DiscoveryReceivedEventArgs, Task> DiscoveryReceived;

		public TimeSpan DiscoveryTimeout { get; set; }

		public int MinDiscoveryCount
		{
			get { return _minDiscoveryCount; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();

				_minDiscoveryCount = value;
			}
		}

		public int MaxWaitTime
		{
			get { return _maxWaitTime; }
			set
			{
				if (value < 0 || value > 120)
					throw new ArgumentOutOfRangeException();

				_maxWaitTime = value;
			}
		}

		public ClientAgent()
			: base()
		{
			DiscoveryTimeout = new TimeSpan(0, 0, 20);
			MaxWaitTime = 3;
		}

		protected override Task HandleMessageAsync(Messages.MessageBase message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return Task.FromResult(0);

			msg.Host = sender.Address.ToString();
			msg.Port = sender.Port;

			if (msg is Messages.ByeMessage)
				return OnByeReceivedAsync(new Events.ByeReceivedEventArgs((Messages.ByeMessage)msg));
			else if (msg is Messages.AliveMessage)
				return OnAnnounceReceivedAsync(new Events.AnnounceEventArgs((Messages.AliveMessage)msg));
			else if (msg is Messages.DiscoveryResponseMessage)
				return OnDiscoveryReceivedAsync(new Events.DiscoveryReceivedEventArgs((Messages.DiscoveryResponseMessage)msg));
			else
				return Task.FromResult(0);
		}

		protected virtual Task OnByeReceivedAsync(Events.ByeReceivedEventArgs e)
		{
			return OnEventHandlerAsync(ByeReceived, e);
		}

		protected virtual Task OnAnnounceReceivedAsync(Events.AnnounceEventArgs e)
		{
			return OnEventHandlerAsync(AnnounceReceived, e);
		}

		protected virtual Task OnDiscoveryReceivedAsync(Events.DiscoveryReceivedEventArgs e)
		{
			return OnEventHandlerAsync(DiscoveryReceived, e);
		}

		public async Task<IEnumerable<ServiceDescription>> DiscoverAsync(string serviceType)
		{
			Guard.NotNullOrEmpty(serviceType, nameof(serviceType));

			var msg = new Messages.DiscoveryMessage();
			msg.Service = new ServiceDescription();
			msg.Service.ServiceType = serviceType;
			msg.MaxWaitTime = MaxWaitTime;

			var result = new List<ServiceDescription>();

			using (var client = new UdpClient())
			{
				var buffer = msg.ToArray();
				for (int i = 0; i < MessageCount; i++)
				{
					await client.SendAsync(buffer, buffer.Length, new IPEndPoint(DiscoveryAddress, Port)).ConfigureAwait(false);
				}
			}

			using (var client = new UdpClient())
			{
				while (true)
				{
					client.Client.ReceiveTimeout = (int)(DiscoveryTimeout).TotalMilliseconds;
					var e = new IPEndPoint(IPAddress.Any, 0);
					UdpReceiveResult received;
					try
					{
						received = await client.ReceiveAsync().ConfigureAwait(false);
					}
					catch (SocketException ex)
					{
						if (ex.SocketErrorCode == SocketError.TimedOut)
						{
							if (result.Count < MinDiscoveryCount)
								continue;
							else
								break;
						}
					}

					var response = Parser.Parse(received.Buffer);
					if (response != null && response is Messages.DiscoveryResponseMessage)
					{
						await HandleMessageAsync(response, e).ConfigureAwait(false);

						if (!result.Contains(response.Service))
							result.Add(response.Service);
					}
				}
			}

			return result;
		}
	}
}