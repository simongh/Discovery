using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Discovery.Ssdp
{
	internal class MessageParser
	{
		public Messages.MessageBase Parse(byte[] data)
		{
			var lines = GetLines(data);
			if (lines == null || lines.Count == 0)
				return null;

			Messages.MessageBase result = GetDiscoveryMessage(lines);
			if (result == null)
				result = GetNotifyMessage(lines);
			if (result == null)
				result = GetHttpMessage(lines);
			if (result == null)
				return null;

			CompleteMessage(result, lines);
			return result;
		}

		private Messages.DiscoveryResponseMessage GetHttpMessage(ICollection<string> lines)
		{
			if (!lines.First()?.StartsWith("HTTP/1.1", StringComparison.OrdinalIgnoreCase) ?? false)
				return null;

			var tmp = lines.First().Substring(8).Trim();
			if (tmp.Length < 3)
				return null;

			if (tmp.Substring(0, 3) != "200")
				throw new DiscoveryException(int.Parse(lines.First().Substring(0, 3)));

			lines.Remove(lines.First());
			var msg = new Messages.DiscoveryResponseMessage();
			msg.Service = new ServiceDescription();

			if (!GetServiceName(msg, lines))
				return null;

			if (!GetServiceType(msg, HeaderNames.ServiceType, lines))
				return null;

			tmp = GetValue(lines, "ext");
			if (tmp == null)
				return null;

			tmp = GetValue(lines, HeaderNames.Location);
			if (tmp == null)
				tmp = GetValue(lines, HeaderNames.Location2);
			if (tmp != null)
				msg.Service.Location = tmp;

			msg.Service.Expiry = GetExpiry(lines);

			return msg;
		}

		private Messages.DiscoveryMessage GetDiscoveryMessage(ICollection<string> lines)
		{
			if (lines.First()?.ToUpper() != "M-SEARCH * HTTP/1.1")
				return null;

			lines.Remove(lines.First());

			var msg = new Messages.DiscoveryMessage();
			msg.Service = new ServiceDescription();

			if (!GetServiceType(msg, HeaderNames.ServiceType, lines))
				return null;

			var tmp = GetValue(lines, "man");
			if (!tmp.Equals("\"ssdp:discover\"", StringComparison.OrdinalIgnoreCase))
				return null;

			tmp = GetValue(lines, HeaderNames.MaxWaitTime);
			if (!string.IsNullOrEmpty(tmp))
			{
				msg.MaxWaitTime = int.Parse(tmp);
			}

			GetHost(lines, msg);

			return msg;
		}

		private Messages.MessageBase GetNotifyMessage(ICollection<string> lines)
		{
			if (lines.First().ToUpper() != "NOTIFY * HTTP/1.1")
				return null;

			lines.Remove(lines.First());

			Messages.MessageBase result = null;
			string tmp = GetValue(lines, HeaderNames.NotificationType);
			if (tmp.Equals("ssdp:alive", StringComparison.OrdinalIgnoreCase))
				result = GetAliveMessage(lines);
			else if (tmp.Equals("ssdp:byebye", StringComparison.OrdinalIgnoreCase))
				result = GetByeMessage(lines);
			else
				return null;

			if (!GetServiceType(result, HeaderNames.NotifiedServiceType, lines))
				return null;

			if (!GetServiceName(result, lines))
				return null;
			GetHost(lines, result);

			return result;
		}

		private Messages.MessageBase GetByeMessage(ICollection<string> lines)
		{
			return new Messages.ByeMessage(new ServiceDescription());
		}

		private Messages.AliveMessage GetAliveMessage(ICollection<string> lines)
		{
			var result = new Messages.AliveMessage(new ServiceDescription());

			string tmp = GetValue(lines, HeaderNames.Location);
			if (tmp == null)
				tmp = GetValue(lines, HeaderNames.Location2);
			if (tmp != null)
				result.Service.Location = tmp;

			result.Service.Expiry = GetExpiry(lines);

			return result;
		}

		private ICollection<string> GetLines(byte[] data)
		{
			var sr = new StringReader(Encoding.UTF8.GetString(data));
			var lines = new List<string>();

			var tmp = sr.ReadLine();
			while (tmp != null)
			{
				lines.Add(tmp);
				tmp = sr.ReadLine();
			}

			return lines;
		}

		private string GetValue(ICollection<string> lines, string header)
		{
			var tmp = lines.FirstOrDefault(l => l.StartsWith(header, StringComparison.OrdinalIgnoreCase));

			if (tmp == null)
				return null;

			lines.Remove(tmp);

			var i = tmp.IndexOf(":");
			if (i < 0)
				return null;

			return tmp.Substring(i + 1).Trim();
		}

		private void CompleteMessage(Messages.MessageBase msg, ICollection<string> lines)
		{
			if (lines.Count == 0)
				return;

			var isBoundaryFound = false;
			foreach (var item in lines)
			{
				if (isBoundaryFound)
					msg.Content += item + "\r\n";
				else
				{
					isBoundaryFound = item == "";
					if (!isBoundaryFound)
						AddHeader(item, msg);
				}
			}
		}

		private void AddHeader(string line, Messages.MessageBase message)
		{
			var parts = line.Split(':');
			message.Headers.Add(parts[0].Trim(), string.Join(":", parts.Skip(1)));
		}

		private TimeSpan GetExpiry(ICollection<string> lines)
		{
			var tmp = GetValue(lines, HeaderNames.CacheControl);
			if (tmp == null)
				tmp = GetValue(lines, HeaderNames.Expires);
			if (!string.IsNullOrEmpty(tmp))
			{
				var m = Regex.Match(tmp, @"(?:max-age\s*=\s*)?(\d+)");
				if (m.Success)
					return new TimeSpan(0, 0, int.Parse(m.Groups[1].Value));
			}
			return new TimeSpan();
		}

		private void GetHost(ICollection<string> lines, Messages.MessageBase message)
		{
			var tmp = GetValue(lines, "host");
			if (string.IsNullOrEmpty(tmp))
				return;

			var i = tmp.IndexOf(":");
			if (i >= 0)
			{
				message.Port = int.Parse(tmp.Substring(i + 1));
				message.Host = tmp.Substring(0, i);
			}
			else
				message.Host = tmp;
		}

		private bool GetServiceName(Messages.MessageBase message, ICollection<string> lines)
		{
			var tmp = GetValue(lines, HeaderNames.ServiceName);
			if (string.IsNullOrEmpty(tmp))
				return false;

			message.Service.UniqueServiceName = tmp;
			return true;
		}

		private bool GetServiceType(Messages.MessageBase message, string header, ICollection<string> lines)
		{
			string tmp = GetValue(lines, header);
			if (string.IsNullOrEmpty(tmp))
				return false;

			message.Service.ServiceType = tmp;
			return true;
		}
	}
}