using System;
using System.Net;

namespace Discovery.SSDP.Agents
{
	public interface IServerAgent : IDisposable
	{
		/// <summary>
		/// Indicates a discovery message has been received
		/// </summary>
		event EventHandler<Events.SearchReceivedEventArgs> SearchReceived;
		/// <summary>
		/// Indicates a discovery response is being sent
		/// </summary>
		event EventHandler<Events.SearchRespondingEventArgs> SearchResponding;

		/// <summary>
		/// Gets the services being advertised
		/// </summary>
		ServiceCollection Services { get; }
		/// <summary>
		/// Gets or sets the multicast address to use to communicate
		/// </summary>
		IPAddress DiscoveryAddress { get; set; }
		/// <summary>
		/// Gets or sets the port to use to communicate
		/// </summary>
		int Port { get; set; }
		/// <summary>
		/// Gets whether the agent is actively listening for messages
		/// </summary>
		bool IsListening { get; }
		/// <summary>
		/// Gets or sets the optional body for messages
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// Broadcast the presence of a service
		/// </summary>
		void Announce(int index);
		/// <summary>
		/// Broadcast the presence of all services
		/// </summary>
		void AnnounceAll();
		/// <summary>
		/// Broadcast the shutdown of a service
		/// </summary>
		void Bye(int index);
		/// <summary>
		/// Broadcast the shutdown of all services
		/// </summary>
		void ByeAll();
		/// <summary>
		/// Starts listening for messages
		/// </summary>
		void StartListener();
		/// <summary>
		/// Stops listening for messages
		/// </summary>
		void StopListener();
	}
}
