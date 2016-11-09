using System;

namespace Discovery.Ssdp.Events
{
	public class AnnounceEventArgs : EventArgs
	{
		public ServiceDescription Service { get; private set; }

		internal AnnounceEventArgs(Messages.AliveMessage message)
		{
			Service = message.Service;
		}
	}
}