using System;
using System.Text;

namespace Discovery.SSDP.Messages
{
	/// <summary>
	/// Message to initiate discovery
	/// </summary>
	internal class DiscoveryMessage : MessageBase
	{
		private int _MaxWaitTime;

		/// <summary>
		/// Gets or sets the max seconds the response should be delayed by between 1 and 120
		/// </summary>
		public int MaxWaitTime
		{
			get { return _MaxWaitTime; }
			set
			{
				if (value < 1 || value > 120)
					throw new ArgumentOutOfRangeException();
				_MaxWaitTime = value;
			}
		}

		/// <summary>
		/// Convert the message to a byte array
		/// </summary>
		/// <returns>The byte array of message data</returns>
		public override byte[] ToArray()
		{
			if (Service == null || Service.ServiceType == null)
				throw new ArgumentNullException();

			StringBuilder sb = new StringBuilder();

			sb.Append("M-SEARCH * HTTP/1.1\r\n");
			sb.AppendFormat("{0}: {1}:{2}\r\n", Discovery.SSDP.Headers.Host, Host, Port);
			sb.Append("Man: \"ssdp:discover\"\r\n");
			sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.MaxWaitTime, MaxWaitTime);
			sb.AppendFormat("{0}: {1}\r\n", Discovery.SSDP.Headers.ServiceType, Service.ServiceType);
			sb.Append("\r\n");

			if (!string.IsNullOrEmpty(Content))
				sb.AppendFormat("{0}\r\n\r\n", Content);

			return Encoding.UTF8.GetBytes(sb.ToString());
		}
	}
}