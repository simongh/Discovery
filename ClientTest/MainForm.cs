using System;
using System.Linq;
using System.Windows.Forms;

namespace ClientTest
{
	public partial class MainForm : Form
	{
		private Discovery.SSDP.Agents.ClientAgent _Client;

		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();

				_Client.Dispose();
			}
			base.Dispose(disposing);
		}

		private void btnDiscover_Click(object sender, EventArgs e)
		{
			btnDiscover.Enabled = false;
			btnStart.Enabled = false;

			if (!string.IsNullOrEmpty(txtServiceType.Text))
			{
				txtResults.Text = null;

				pbThinking.Style = ProgressBarStyle.Marquee;
				pbThinking.MarqueeAnimationSpeed = 30;

				var s = _Client.Discover(txtServiceType.Text);

				if (s.Any())
					txtResults.Lines = s.Select(x => x.ServiceType + " - " + x.Location).ToArray();
				else
					txtResults.Text = "Not found";

				pbThinking.Style = ProgressBarStyle.Continuous;
				pbThinking.MarqueeAnimationSpeed = 0;
			}

			btnDiscover.Enabled = true;
			btnStart.Enabled = true;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			_Client = new Discovery.SSDP.Agents.ClientAgent();
			_Client.Port = 1900;

			_Client.AnnounceReceived += _Client_AnnounceReceived;
			_Client.ByeReceived += _Client_ByeReceived;
		}

		void _Client_ByeReceived(object sender, Discovery.SSDP.Events.ByeReceivedEventArgs e)
		{
			if (InvokeRequired)
				Invoke((MethodInvoker)delegate
				{
					txtResults.Text = "bye: " + e.Service.ServiceType + "\r\n" + txtResults.Text;
				});
		}

		void _Client_AnnounceReceived(object sender, Discovery.SSDP.Events.AnnounceEventArgs e)
		{
			if (InvokeRequired)
				Invoke((MethodInvoker)delegate {
					txtResults.Text = "announced: " + e.Service.ServiceType + "\r\n" + txtResults.Text;
				});
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			btnDiscover.Enabled = false;
			btnStop.Enabled = true;

			pbThinking.Style = ProgressBarStyle.Marquee;
			pbThinking.MarqueeAnimationSpeed = 30;

			_Client.StartListener();

			btnStart.Enabled = false;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			btnStop.Enabled = false;

			_Client.StopListener();

			pbThinking.Style = ProgressBarStyle.Continuous;
			pbThinking.MarqueeAnimationSpeed = 0;

			btnDiscover.Enabled = true;
			btnStart.Enabled = true;
		}
	}
}
