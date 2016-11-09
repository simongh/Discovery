using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discovery.Ssdp.Agents
{
	public abstract class AgentBase : IDisposable
	{
		private bool disposedValue;
		protected UdpClient _Listener;
		internal MessageParser Parser;

		/// <summary>
		/// Gets or sets the multicast address to use to communicate
		/// </summary>
		public IPAddress DiscoveryAddress { get; set; }

		/// <summary>
		/// Gets or sets the port to use to communicate
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Gets whether the agent is actively listening for messages
		/// </summary>
		public bool IsListening { get; private set; }

		/// <summary>
		/// Gets or sets the optional body for messages
		/// </summary>
		public string Content { get; set; }

		public int MessageCount { get; set; }

		public int Ttl { get; set; }

		public AgentBase()
		{
			Parser = new MessageParser();
			MessageCount = 3;
			DiscoveryAddress = IPAddress.Parse("239.255.255.250");
			Port = 1900;
			Ttl = 4;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_Listener != null)
						_Listener.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public async Task StartListener()
		{
			Guard.NotNull(DiscoveryAddress, nameof(DiscoveryAddress));

			if (IsListening)
				throw new DiscoveryException("Already listening");

			if (_Listener != null)
			{
				_Listener.Dispose();
				_Listener = null;
			}

			_Listener = new UdpClient(Port);
			_Listener.Ttl = (short)Ttl;
			_Listener.JoinMulticastGroup(DiscoveryAddress);

			IsListening = true;
			while (IsListening)
			{
				var result = await _Listener.ReceiveAsync();
				if (!IsListening)
					break;
				await ParseResponse(result);
			}
		}

		/// <summary>
		/// Stops listening for messages
		/// </summary>
		public void StopListener()
		{
			if (!IsListening)
				return;

			IsListening = false;

			var ep = new IPEndPoint(IPAddress.Any, 0);
			_Listener.DropMulticastGroup(DiscoveryAddress);
			_Listener.Dispose();
			_Listener = null;
		}

		protected async Task ParseResponse(UdpReceiveResult result)
		{
			if (result.Buffer == null || result.RemoteEndPoint == null)
				return;

			Messages.MessageBase msg = Parser.Parse(result.Buffer);
			if (msg == null)
				return;

			await HandleMessage(msg, result.RemoteEndPoint);
		}

		protected abstract Task HandleMessage(Messages.MessageBase message, IPEndPoint sender);

		public Task Send(Messages.MessageBase message, IPEndPoint endpoint)
		{
			return Send(new Messages.MessageBase[] { message }, endpoint);
		}

		public async Task Send(IEnumerable<Messages.MessageBase> messages, IPEndPoint endpoint)
		{
			using (var client = new UdpClient())
			{
				foreach (var item in messages)
				{
					item.Host = DiscoveryAddress.ToString();
					item.Port = Port;

					var buffer = item.ToArray();
					for (int i = 0; i < MessageCount; i++)
					{
						await client.SendAsync(buffer, buffer.Length, endpoint);
					}
				}
			}
		}
	}
}