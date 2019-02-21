using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Discovery.Ssdp.Agents
{
	public abstract class AgentBase : IDisposable
	{
		private bool _disposedValue;
		protected UdpClient _Listener;
		internal MessageParser Parser;
		private CancellationTokenSource _listenerToken;

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

		~AgentBase()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					if (_Listener != null)
						_Listener.Dispose();
				}

				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		public Task StartListenerAsync()
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
			_listenerToken = new CancellationTokenSource();
			return Task.Factory.StartNew(async () =>
			{
				while (IsListening)
				{
					var result = await _Listener.ReceiveAsync().ConfigureAwait(false);

					if (!IsListening)
						break;
					await ParseResponseAsync(result);
				}
			}, _listenerToken.Token);
		}

		/// <summary>
		/// Stops listening for messages
		/// </summary>
		public void StopListener()
		{
			if (!IsListening)
				return;

			IsListening = false;
			_listenerToken.Cancel();

			var ep = new IPEndPoint(IPAddress.Any, 0);
			_Listener.DropMulticastGroup(DiscoveryAddress);
			_Listener.Dispose();
			_Listener = null;
		}

		protected Task ParseResponseAsync(UdpReceiveResult result)
		{
			if (result.Buffer == null || result.RemoteEndPoint == null)
				return Task.FromResult(0);

			Messages.MessageBase msg = Parser.Parse(result.Buffer);
			if (msg == null)
				return Task.FromResult(0);

			return HandleMessageAsync(msg, result.RemoteEndPoint);
		}

		protected abstract Task HandleMessageAsync(Messages.MessageBase message, IPEndPoint sender);

		public Task SendAsync(Messages.MessageBase message, IPEndPoint endpoint)
		{
			return SendAsync(new Messages.MessageBase[] { message }, endpoint);
		}

		public async Task SendAsync(IEnumerable<Messages.MessageBase> messages, IPEndPoint endpoint)
		{
			Guard.NotNull(messages, nameof(messages));
			Guard.NotNull(endpoint, nameof(endpoint));

			using (var client = new UdpClient())
			{
				foreach (var item in messages)
				{
					item.Host = DiscoveryAddress.ToString();
					item.Port = Port;

					var buffer = item.ToArray();
					for (int i = 0; i < MessageCount; i++)
					{
						await client.SendAsync(buffer, buffer.Length, endpoint).ConfigureAwait(false);
					}
				}
			}
		}

		protected async Task OnEventHandlerAsync<T>(Func<object, T, Task> handler, T e)
		{
			if (handler == null)
				return;

			var invocationList = handler.GetInvocationList();
			var tasks = new List<Task>(invocationList.Length);

			foreach (var item in invocationList)
			{
				tasks.Add(((Func<object, T, Task>)item)(this, e));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}
	}
}