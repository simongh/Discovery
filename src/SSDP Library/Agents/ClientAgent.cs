﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;

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

		public event EventHandler<Events.DiscoveryReceivedEventArgs> DiscoveryReceived;

		private delegate IList<Service> DiscoverDelegate(string serviceType);

		public TimeSpan DiscoveryTimeout { get; set; }

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
			var config = ConfigSection.Instance;

			DiscoveryTimeout = config == null ? new TimeSpan(0, 0, 20) : config.DiscoveryTimeout;
			MaxWaitTime = config == null ? 3 : config.MaxWaitTime;
		}

		protected override void HandleMessage(object message, IPEndPoint sender)
		{
			var msg = message as Messages.MessageBase;
			if (msg == null)
				return;

			msg.Host = sender.Address.ToString();
			msg.Port = sender.Port;

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
					if (response != null && response is Messages.DiscoveryResponseMessage)
					{
						HandleMessage(response, e);

						if (!result.Contains(response.Service))
							result.Add(response.Service);
					}
				}
			}

			return result;
		}

		public IAsyncResult BeginDiscover(string serviceType, AsyncCallback callback, object state)
		{
			DiscoverDelegate discover = Discover;
			return discover.BeginInvoke(serviceType, callback, state);
		}

		public IList<Service> EndDiscover(IAsyncResult asyncResult)
		{
			var discover = (DiscoverDelegate)((AsyncResult)asyncResult).AsyncDelegate;
			return discover.EndInvoke(asyncResult);
		}
	}
}