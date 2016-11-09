using System;

namespace Discovery.Ssdp.Events
{
	/// <summary>
	/// Arguments when responding to a search request
	/// </summary>
	public class SearchRespondingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets any body content
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Gets the recipient end point
		/// </summary>
		public System.Net.IPEndPoint Recipient { get; private set; }

		public SearchRespondingEventArgs(string content, System.Net.IPEndPoint recipient)
		{
			Content = content;
			Recipient = recipient;
		}
	}
}