using System;

namespace Discovery.Ssdp
{
	internal static class Guard
	{
		public static T NotNull<T>(T value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException(paramName);

			return value;
		}

		public static void NotNullOrEmpty(string value, string paramName)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Value must not be null or blank", paramName);
		}
	}
}