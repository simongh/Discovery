using System;

namespace Discovery.SSDP
{
	/// <summary>
	/// Represents a service
	/// </summary>
	public class Service : IEquatable<Service>
	{
		/// <summary>
		/// Gets or sets a unique name for the service
		/// </summary>
		public string UniqueServiceName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the location of the service
		/// </summary>
		public string Location
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the service type
		/// </summary>
		public string ServiceType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the length of time the service registration is valid for
		/// </summary>
		public TimeSpan Expiry
		{
			get;
			set;
		}

		public bool Equals(Service other)
		{
			if (other == null)
				return false;

			if (!string.Equals(Location, other.Location))
				return false;
			if (!string.Equals(UniqueServiceName, other.UniqueServiceName))
				return false;
			if (!string.Equals(ServiceType, other.ServiceType))
				return false;
			if (!TimeSpan.Equals(Expiry, other.Expiry))
				return false;

			return true;
		}
	}

}
