using System;
using System.Threading.Tasks;

namespace server.core
{
	internal class Program : IDisposable
	{
		private readonly Discovery.Ssdp.Agents.ServerAgent _serverAgent;
		private bool disposedValue = false; // To detect redundant calls

		private static async Task Main(string[] args)
		{
			var p = new Program();

			await p.RunAsync(args);
			Console.ReadKey();
		}

		public Program()
		{
			_serverAgent = new Discovery.Ssdp.Agents.ServerAgent();

			_serverAgent.SearchReceived += _serverAgent_SearchReceived;
			_serverAgent.SearchResponding += _serverAgent_SearchResponding;
			_serverAgent.SearchResponded += _serverAgent_SearchResponded;
		}

		private Task _serverAgent_SearchResponded(object sender, EventArgs e)
		{
			Console.WriteLine("done responding:");
			return Task.FromResult(0);
		}

		private Task _serverAgent_SearchResponding(object sender, Discovery.Ssdp.Events.SearchRespondingEventArgs e)
		{
			Console.WriteLine($"responding to: {e.Recipient}");
			return Task.FromResult(0);
		}

		private Task _serverAgent_SearchReceived(object sender, Discovery.Ssdp.Events.SearchReceivedEventArgs e)
		{
			Console.WriteLine($"received: {e.ServiceType} from: {e.Sender}");
			return Task.FromResult(0);
		}

		public async Task RunAsync(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Starting listener. Press any key to quit");
				await _serverAgent.StartListenerAsync();
			}
			else if (args.Length == 1 && args[0] == "active")
			{
				_serverAgent.Services.Add(new Discovery.Ssdp.ServiceDescription
				{
					UniqueServiceName = "ssdp-test",
					Location = "localhost",
					ServiceType = "ssdp-test",
				});

				Console.WriteLine("announcing service alive");
				await _serverAgent.AnnounceAllAsync();

				Console.WriteLine("waiting 1 min");
				await Task.Delay(TimeSpan.FromMinutes(1));

				Console.WriteLine("announcing service ending");
				await _serverAgent.ByeAllAsync();

				Console.WriteLine("Press any key to quit");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_serverAgent.Dispose();
				}

				disposedValue = true;
			}
		}

		~Program()
		{
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}