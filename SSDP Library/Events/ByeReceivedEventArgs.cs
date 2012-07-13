using System;
using System.Net;

namespace Discovery.SSDP.Events
{
	public class ByeReceivedEventArgs : EventArgs
	{
		public Service Service
		{
			get;
			private set;
		}

		internal ByeReceivedEventArgs(Messages.ByeMessage message)
		{
			Service = message.Service;
		}
	}
}
