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

		public event EventHandler<Events.ByeReceivedEventArgs> ByeReceived;

		public event EventHandler<Events.AnnounceEventArgs> AnnounceReceived;

		public event EventHandler<Events.DiscoveryReceivedEventArgs> DiscoveryReceived;

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

		protected override async Task HandleMessage(Messages.MessageBase message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return;

			msg.Host = sender.Address.ToString();
			msg.Port = sender.Port;

			if (msg is Messages.ByeMessage)
				await OnByeReceived(new Events.ByeReceivedEventArgs((Messages.ByeMessage)msg));
			else if (msg is Messages.AliveMessage)
				await OnAnnounceReceived(new Events.AnnounceEventArgs((Messages.AliveMessage)msg));
			else if (msg is Messages.DiscoveryResponseMessage)
				await OnDiscoveryReceived(new Events.DiscoveryReceivedEventArgs((Messages.DiscoveryResponseMessage)msg));
		}

		protected virtual Task OnByeReceived(Events.ByeReceivedEventArgs e)
		{
			ByeReceived?.Invoke(this, e);

			return Task.FromResult(0);
		}

		protected virtual Task OnAnnounceReceived(Events.AnnounceEventArgs e)
		{
			AnnounceReceived?.Invoke(this, e);

			return Task.FromResult(0);
		}

		protected virtual Task OnDiscoveryReceived(Events.DiscoveryReceivedEventArgs e)
		{
			DiscoveryReceived?.Invoke(this, e);

			return Task.FromResult(0);
		}

		public async Task<IEnumerable<ServiceDescription>> Discover(string serviceType)
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
					await client.SendAsync(buffer, buffer.Length, new IPEndPoint(DiscoveryAddress, Port));
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
						received = await client.ReceiveAsync();
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
						await HandleMessage(response, e);

						if (!result.Contains(response.Service))
							result.Add(response.Service);
					}
				}
			}

			return result;
		}
	}
}