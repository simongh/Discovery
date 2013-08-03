using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientTest
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());

	//		Console.WriteLine("Client");
	//		ping2();
	//		Console.ReadLine();
		}

		private static void ping2()
		{
			var c = new Discovery.SSDP.Agents.ClientAgent();
			//c.DiscoveryAddress = System.Net.IPAddress.Parse("239.255.255.250");
			c.Port = 19000;

			var s = c.Discover("ge:fridge");

			Console.WriteLine(s.Count());
			if (s.Any())
			{
				Console.WriteLine(s.First().ServiceType);
				Console.WriteLine(s.First().Location);
			}
		}

		private static void ping1()
		{
			using (UdpClient u = new UdpClient())
			{
				//u.JoinMulticastGroup(System.Net.IPAddress.Parse("239.255.255.250"));
				u.ExclusiveAddressUse = false;

				string msg = "M-SEARCH * HTTP/1.1\r\n";
				msg = msg + "Host: 239.255.255.250:19000\r\n";
				msg = msg + "Man: \"ssdp:discover\"\r\n";
				msg = msg + "ST: ge:fridge\r\n";
				msg = msg + "MX: 3\r\n\r\n";

				byte[] buffer = Encoding.UTF8.GetBytes(msg);
				u.Send(buffer, buffer.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Parse("239.255.255.250"), 19000));
				buffer = null;

				System.Net.IPEndPoint e = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
				buffer = u.Receive(ref e);
				Console.WriteLine(Encoding.UTF8.GetString(buffer));
			}
		}
	}
}
