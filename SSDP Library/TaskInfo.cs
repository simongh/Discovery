using System;
using System.Net;

namespace Discovery.SSDP
{
	/// <summary>
	/// Holds received message data for passing to the handler thread
	/// </summary>
	internal class TaskInfo
	{
		private readonly IPEndPoint _EndPoint;
		private readonly byte[] _Data;

		/// <summary>
		/// Gets the sender endpoint
		/// </summary>
		public IPEndPoint EndPoint
		{
			get { return _EndPoint; }
		}

		/// <summary>
		/// Gets the data received
		/// </summary>
		public byte[] Data
		{
			get { return _Data; }
		}

		public TaskInfo(IPEndPoint endPoint, byte[] data)
		{
			_EndPoint = endPoint;
			_Data = data;
		}
	}
}
