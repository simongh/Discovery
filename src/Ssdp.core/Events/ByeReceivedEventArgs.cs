using System;

namespace Discovery.Ssdp.Events
{
	public class ByeReceivedEventArgs : EventArgs
	{
		public ServiceDescription Service { get; private set; }

		internal ByeReceivedEventArgs(Messages.ByeMessage message)
		{
			Service = message.Service;
		}
	}
}