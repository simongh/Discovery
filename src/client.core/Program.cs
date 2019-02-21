using System;
using System.Threading.Tasks;

namespace client.core
{
	internal class Program : IDisposable
	{
		private readonly Discovery.Ssdp.Agents.ClientAgent _client;
		private bool disposedValue = false;

		private static async Task Main(string[] args)
		{
			using (var p = new Program())
			{
				await p.RunAsync(args);

				Console.ReadKey();
			}
		}

		public Program()
		{
			_client = new Discovery.Ssdp.Agents.ClientAgent();

			_client.AnnounceReceived += _client_AnnounceReceived;
			_client.ByeReceived += _client_ByeReceived;
			_client.DiscoveryReceived += _client_DiscoveryReceived;
		}

		private Task _client_DiscoveryReceived(object sender, Discovery.Ssdp.Events.DiscoveryReceivedEventArgs e)
		{
			Console.WriteLine($"discovered: {e.Service.ServiceType} - {e.Service.Location}");

			return Task.FromResult(0);
		}

		private Task _client_ByeReceived(object sender, Discovery.Ssdp.Events.ByeReceivedEventArgs e)
		{
			Console.WriteLine($"bye: {e.Service.ServiceType}");
			return Task.FromResult(0);
		}

		private Task _client_AnnounceReceived(object sender, Discovery.Ssdp.Events.AnnounceEventArgs e)
		{
			Console.WriteLine($"announced: {e.Service.ServiceType}");
			return Task.FromResult(0);
		}

		private async Task RunAsync(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Starting listener. Press any key to exit");
				await _client.StartListenerAsync();
			}
			else if (args.Length == 1 && args[0] == "discover")
			{
				Console.WriteLine("discovering ssdp test service");
				await _client.DiscoverAsync("ssdp-test");

				Console.WriteLine("Press any key to exit");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_client.Dispose();
				}

				disposedValue = true;
			}
		}

		~Program()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}