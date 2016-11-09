using System;

namespace Discovery.Ssdp
{
	internal static class Guard
	{
		public static void NotNull(object value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException(paramName);
		}

		public static void NotNullOrEmpty(string value, string paramName)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Value must not be null or blank", paramName);
		}
	}
}