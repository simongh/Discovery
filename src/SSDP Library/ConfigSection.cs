using System;
using System.Configuration;

namespace Discovery.SSDP
{
	public class ConfigSection : ConfigurationSection
	{
		private const string PORT = "port";
		private const string SECTION = "discovery";
		private const string MCASTADDR = "address";
		private const string MESSAGECOUNT = "messageCount";
		private const string TTL = "ttl";
		private const string DISCOVERYTIMEOUT = "discoveryTimeout";
		private const string MINDISCOVERYCOUNT = "discoveryCount";
		private const string MAXWAITTIME = "maxWaitTime";

		public static ConfigSection Instance
		{
			get { return (ConfigSection)ConfigurationManager.GetSection(SECTION); }
		}

		[ConfigurationProperty(PORT, DefaultValue = 1900)]
		[IntegerValidator(MinValue = 0, MaxValue = 0xffff)]
		public int Port
		{
			get { return (int)this[PORT]; }
			set { this[PORT] = value; }
		}

		[ConfigurationProperty(MCASTADDR, DefaultValue = "239.255.255.250")]
		public string Address
		{
			get { return (string)this[MCASTADDR]; }
			set { this[MCASTADDR] = value; }
		}

		[ConfigurationProperty(MESSAGECOUNT, DefaultValue = 3)]
		[IntegerValidator(MinValue = 0)]
		public int MessageCount
		{
			get { return (int)this[MESSAGECOUNT]; }
			set { this[MESSAGECOUNT] = value; }
		}

		[ConfigurationProperty(TTL, DefaultValue = 4)]
		[IntegerValidator(MinValue = 0, MaxValue = 0xff)]
		public int Ttl
		{
			get { return (int)this[TTL]; }
			set { this[TTL] = value; }
		}

		[ConfigurationProperty(DISCOVERYTIMEOUT, DefaultValue = "0:0:20")]
		[PositiveTimeSpanValidator()]
		public TimeSpan DiscoveryTimeout
		{
			get { return (TimeSpan)this[DISCOVERYTIMEOUT]; }
			set { this[DISCOVERYTIMEOUT] = value; }
		}

		[ConfigurationProperty(MINDISCOVERYCOUNT, DefaultValue = 0)]
		[IntegerValidator(MinValue = 0)]
		public int MinDiscoveryCount
		{
			get { return (int)this[MINDISCOVERYCOUNT]; }
			set { this[MINDISCOVERYCOUNT] = value; }
		}

		[ConfigurationProperty(MAXWAITTIME, DefaultValue = 3)]
		[IntegerValidator(MinValue = 0, MaxValue = 120)]
		public int MaxWaitTime
		{
			get { return (int)this[MAXWAITTIME]; }
			set { this[MAXWAITTIME] = value; }
		}
	}
}