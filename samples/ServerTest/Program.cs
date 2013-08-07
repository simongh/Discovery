using System;
using System.Windows.Forms;

namespace ServerTest
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void Runner()
		{
			Console.WriteLine("server");
			var sa = new Discovery.SSDP.Agents.ServerAgent();
			sa.SearchReceived += new EventHandler<Discovery.SSDP.Events.SearchReceivedEventArgs>(sa_SearchReceived);
			
			//sa.DiscoveryAddress = System.Net.IPAddress.Parse("239.255.255.250");
			sa.Port = 19000;
			sa.Services.Add(new Discovery.SSDP.Service());
			sa.Services[0].ServiceType = "ge:fridge";
			sa.Services[0].UniqueServiceName = "uuid:" + Guid.NewGuid().ToString();
			sa.Services[0].Location = "http://foo/bar";

			try
			{
				sa.StartListener();

				Console.ReadLine();
				sa.StopListener();
				Console.ReadLine();
			}
			finally
			{
				sa.Dispose();
			}
		}

		static void sa_SearchReceived(object sender, Discovery.SSDP.Events.SearchReceivedEventArgs e)
		{
			Console.WriteLine("received: " + e.ServiceType + " from " + e.Sender);
			//Console.WriteLine("received" + Encoding.UTF8.GetString(t.Data) + " from " + t.EndPoint.Address.ToString());
		}
	}
}
