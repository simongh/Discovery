using System;
using System.Windows.Forms;

namespace ServerTest
{
	public partial class MainForm : Form
	{
		private Discovery.SSDP.Agents.ServerAgent _agent;

		public MainForm()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			btnStart.Enabled = false;

			_agent.StartListener();

			pbThinking.Style = ProgressBarStyle.Marquee;
			pbThinking.MarqueeAnimationSpeed = 30;

			btnStop.Enabled = true;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			_agent = new Discovery.SSDP.Agents.ServerAgent();

			_agent.SearchReceived += _agent_SearchReceived;
			_agent.SearchResponding += _agent_SearchResponding;
			_agent.SearchResponded += _agent_SearchResponded;
		}

		void _agent_SearchResponded(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
				Invoke((MethodInvoker)delegate { _agent_SearchResponded(sender, e); });
			else
				txtResults.Text = "done responding: \r\n" + txtResults.Text;
		}

		void _agent_SearchResponding(object sender, Discovery.SSDP.Events.SearchRespondingEventArgs e)
		{
			if (this.InvokeRequired)
				Invoke((MethodInvoker)delegate { _agent_SearchResponding(sender, e); });
			else
				txtResults.Text = "responding to: " + e.Recipient + "\r\n" + txtResults.Text;
		}

		void _agent_SearchReceived(object sender, Discovery.SSDP.Events.SearchReceivedEventArgs e)
		{
			if (this.InvokeRequired)
				Invoke((MethodInvoker)delegate { _agent_SearchReceived(sender, e); });
			else
				txtResults.Text = "received: " + e.ServiceType + " from " + e.Sender + "\r\n" + txtResults.Text;
		}

		private void lbServices_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnAnnounce.Enabled = lbServices.SelectedIndex > -1;
			btnBye.Enabled = lbServices.SelectedIndex > -1;

			if (lbServices.SelectedIndex > -1)
			{
				var s = _agent.Services[lbServices.SelectedIndex];

				txtServiceType.Text = s.ServiceType;
				txtName.Text = s.UniqueServiceName;
				txtLocation.Text = s.Location;
			}
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			btnStop.Enabled = false;

			_agent.StopListener();

			pbThinking.Style = ProgressBarStyle.Continuous;
			pbThinking.MarqueeAnimationSpeed = 0;

			btnStart.Enabled = true;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtServiceType.Text))
				return;
			if (string.IsNullOrEmpty(txtName.Text))
				return;
			if (string.IsNullOrEmpty(txtLocation.Text))
				return;

			var s = new Discovery.SSDP.Service();
			s.ServiceType = txtServiceType.Text;
			if (txtName.Text == "uuid:")
				s.UniqueServiceName = "uuid:" + Guid.NewGuid().ToString();
			else
				s.UniqueServiceName = txtName.Text;
			s.Location = txtLocation.Text;

			_agent.Services.Add(s);
			lbServices.Items.Add(s.ServiceType);

			txtServiceType.Text = null;
			txtName.Text = null;
			txtLocation.Text = null;
		}

		private void btnAnnounce_Click(object sender, EventArgs e)
		{
			_agent.Announce(lbServices.SelectedIndex);
		}

		private void btnBye_Click(object sender, EventArgs e)
		{
			_agent.Bye(lbServices.SelectedIndex);

			lbServices.Items.RemoveAt(lbServices.SelectedIndex);
		}
	}
}
