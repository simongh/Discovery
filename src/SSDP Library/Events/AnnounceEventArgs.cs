using System;
using System.Net;

namespace Discovery.SSDP.Events
{
	public class AnnounceEventArgs : EventArgs
	{
		public Service Service
		{
			get;
			private set;
		}

		internal AnnounceEventArgs(Messages.AliveMessage message)
		{
			Service = message.Service;
		}
	}
}
