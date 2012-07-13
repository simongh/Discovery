using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Discovery.SSDP.Agents
{
	/// <summary>
	/// Handles client discovery activity
	/// </summary>
	public class ClientAgent : AgentBase
	{
		private int _MinDiscoveryCount;
		private int _MaxWaitTime;

		public event EventHandler<Events.ByeReceivedEventArgs> ByeReceived;
		public event EventHandler<Events.AnnounceEventArgs> AnnounceReceived;
		public event EventHandler DiscoveryReceived;

		public TimeSpan DiscoveryTimeout
		{
			get;
			set;
		}

		public int MinDiscoveryCount
		{
			get { return _MinDiscoveryCount; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();

				_MinDiscoveryCount = value;
			}
		}

		public int MaxWaitTime
		{
			get { return _MaxWaitTime; }
			set
			{
				if (value < 0 || value > 120)
					throw new ArgumentOutOfRangeException();

				_MaxWaitTime = value;
			}
		}

		public ClientAgent()
			: base()
		{
			DiscoveryTimeout = new TimeSpan(0, 0, 20);
			MaxWaitTime = 3;
		}

		protected override void HandleMessage(object message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return;

			if (msg is Messages.ByeMessage)
				OnByeReceived(new Events.ByeReceivedEventArgs((Messages.ByeMessage)msg));
			else if (msg is Messages.AliveMessage)
				OnAnnounceReceived(new Events.AnnounceEventArgs((Messages.AliveMessage)msg));
			else if (msg is Messages.DiscoveryResponseMessage)
				OnDiscoveryReceived(new Events.DiscoveryReceivedEventArgs((Messages.DiscoveryResponseMessage)msg));
		}

		protected virtual void OnByeReceived(Events.ByeReceivedEventArgs e)
		{
			if (ByeReceived == null)
				return;

			ByeReceived(this, e);
		}

		protected virtual void OnAnnounceReceived(Events.AnnounceEventArgs e)
		{
			if (AnnounceReceived == null)
				return;

			AnnounceReceived(this, e);
		}

		protected virtual void OnDiscoveryReceived(Events.DiscoveryReceivedEventArgs e)
		{
			if (DiscoveryReceived == null)
				return;

			DiscoveryReceived(this, e);
		}

		public IList<Service> Discover(string serviceType)
		{
			var msg = new Messages.DiscoveryMessage();
			msg.Service = new Service();
			msg.Service.ServiceType = serviceType;
			msg.MaxWaitTime = MaxWaitTime;

			var result = new List<Service>();

			using (var client = new UdpClient())
			{
				var buffer = msg.ToArray();
				for (int i = 0; i < MessageCount; i++)
				{
					client.Send(buffer, buffer.Length, new IPEndPoint(DiscoveryAddress, Port));
				}

				while (true)
				{
					client.Client.ReceiveTimeout = (int)(DiscoveryTimeout).TotalMilliseconds;
					var e = new IPEndPoint(IPAddress.Any, 0);
					try
					{
						buffer = client.Receive(ref e);
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

					var response = Parser.Parse(buffer);
					if (response != null && response is Messages.DiscoveryResponseMessage && !result.Contains(response.Service))
						result.Add(response.Service);
				}
			}
			
			return result;
		}
	}
}
