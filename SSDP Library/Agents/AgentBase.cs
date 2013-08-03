using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Discovery.SSDP.Agents
{
	/// <summary>
	/// Base agent class
	/// </summary>
	public abstract class AgentBase : IDisposable
	{
		/// <summary>
		/// State object for async calls
		/// </summary>
		protected class UdpState
		{
			public UdpClient Client;

			public int Counter;

			public UdpState(UdpClient client)
				: this(client, 0)
			{ }

			public UdpState(UdpClient client, int counter)
			{
				Client = client;
				Counter = counter;
			}
		}

		protected UdpClient _Listener;
		internal MessageParser Parser;

		/// <summary>
		/// Gets or sets the multicast address to use to communicate
		/// </summary>
		public IPAddress DiscoveryAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the port to use to communicate
		/// </summary>
		public int Port
		{
			get;
			set;
		}

		/// <summary>
		/// Gets whether the agent is actively listening for messages
		/// </summary>
		public bool IsListening
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the optional body for messages
		/// </summary>
		public string Content
		{
			get;
			set;
		}

		public int MessageCount
		{
			get;
			set;
		}

		public int Ttl
		{
			get;
			set;
		}

		public AgentBase()
		{
			Parser = new MessageParser();
			var config = ConfigSection.Instance;
			MessageCount = config == null ? 3 : config.MessageCount;
			DiscoveryAddress = IPAddress.Parse(config==null ? "239.255.255.250" : config.Address);
			Port = config == null ? 1900 : config.Port;
			Ttl = config == null ? 4 : config.Ttl;
		}

		#region IDispose

		/// <summary>
		/// Disposes the agent cleanly
		/// </summary>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_Listener == null)
				return;

			if (disposing)
			{ }

			IsListening = false;
			_Listener.Close();
			_Listener = null;
		}

		~AgentBase()
		{
			Dispose(false);
		}

		#endregion

		/// <summary>
		/// Starts listening for messages
		/// </summary>
		public void StartListener()
		{
			if (DiscoveryAddress == null)
				throw new ArgumentNullException("Address");

			if (IsListening)
				throw new ApplicationException("Already listening");

			if (_Listener != null)
			{
				_Listener.Close();
				_Listener = null;
			}

			_Listener = new UdpClient(Port);
			_Listener.Ttl = (short)Ttl;
			_Listener.JoinMulticastGroup(DiscoveryAddress);

			_Listener.BeginReceive(new AsyncCallback(ReceiveCallback), new UdpState(_Listener));
			IsListening = true;
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
		}

		/// <summary>
		/// Async callback method
		/// </summary>
		/// <param name="ar">async state</param>
		protected void ReceiveCallback(IAsyncResult ar)
		{
			UdpClient client = ((UdpState)ar.AsyncState).Client;

			if (client == null || client.Client == null || client.Client.IsBound == false)
				return;
			if (!IsListening)
				return;

			IPEndPoint e = new IPEndPoint(IPAddress.Any, Port);
			byte[] buffer = client.EndReceive(ar, ref e);

			ThreadPool.QueueUserWorkItem(new WaitCallback(ParseResponse), new TaskInfo(e, buffer));
			client.BeginReceive(new AsyncCallback(ReceiveCallback), new UdpState(client));
		}

		/// <summary>
		/// Process received messages
		/// </summary>
		/// <param name="stateInfo">A <c>TaskInfo</c> object containing the received message data</param>
		protected void ParseResponse(object stateInfo)
		{
			if (!IsListening)
				return;

			TaskInfo t = (TaskInfo)stateInfo;
			Messages.MessageBase msg = Parser.Parse(t.Data);
			if (msg == null)
				return;

			HandleMessage(msg, t.EndPoint);
		}

		protected abstract void HandleMessage(object message, IPEndPoint sender);

		//internal IAsyncResult Send(Messages.MessageBase message, IPEndPoint endpoint, AsyncCallback receiveCallback)
		//{
		//    message.Host = DiscoveryAddress.ToString();
		//    message.Port = Port;
		//    message.Content = Content;

		//    using (UdpClient u = new UdpClient())
		//    {
		//        byte[] buffer = message.ToArray();
		//        u.ExclusiveAddressUse = false;
		//        u.Send(buffer, buffer.Length, endpoint);

		//        if (receiveCallback != null)
		//            return u.BeginReceive(receiveCallback, new UdpState(u));
		//        else
		//            return null;
		//    }

		//}

		//internal void Send(Messages.MessageBase message, IPEndPoint endpoint)
		//{
		//    message.Host = DiscoveryAddress.ToString();
		//    message.Port = Port;
		//    message.Content = Content;

			

		//    var buffer = message.ToArray();
		//    _Listener.Send(buffer, buffer.Length, endpoint);
		//}

		//protected void CheckClient()
		//{
		//    if (Port == 0 || Port < 0)
		//        throw new ArgumentOutOfRangeException("Port");

		//    if (_Listener == null)
		//        _Listener = new UdpClient(Port);
		//}

		public void Send(Messages.MessageBase message, IPEndPoint endpoint)
		{
			Send(new Messages.MessageBase[] { message }, endpoint);
		}

		public void Send(IEnumerable<Messages.MessageBase> messages, IPEndPoint endpoint)
		{
			UdpClient client = new UdpClient();

			byte[] buffer = null;

			foreach (var item in messages)
			{
				item.Host = DiscoveryAddress.ToString();
				item.Port = Port;

				buffer = item.ToArray();
				for (int i = 0; i < MessageCount; i++)
				{
					client.BeginSend(buffer, buffer.Length, endpoint, delegate(IAsyncResult ar)
					{
						var u = (UdpState)ar.AsyncState;
						u.Client.EndSend(ar);
						if (u.Counter == MessageCount)
							u.Client.Close();
					}, new UdpState(client, i));
				}
			}
		}


	}
}
