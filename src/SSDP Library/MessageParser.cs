using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Discovery.SSDP
{
	internal class MessageParser
	{
		public Messages.MessageBase Parse(byte[] data)
		{
			List<string> lines = GetLines(data);
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

		private Messages.DiscoveryResponseMessage GetHttpMessage(List<string> lines)
		{
			if (!lines[0].StartsWith("HTTP/1.1", StringComparison.InvariantCultureIgnoreCase))
				return null;

			string tmp = lines[0].Substring(8).Trim();
			if (tmp.Length < 3)
				return null;

			if (tmp.Substring(0, 3) != "200")
				throw new ApplicationException("Response received: " + lines[0]);

			lines.RemoveAt(0);
			Messages.DiscoveryResponseMessage msg = new Messages.DiscoveryResponseMessage();
			msg.Service = new Service();

			if (!GetServiceName(msg, lines))
				return null;

			if (!GetServiceType(msg, Headers.ServiceType, lines))
				return null;

			tmp = GetValue(lines, "ext");
			if (tmp == null)
				return null;

			tmp = GetValue(lines, Headers.Location);
			if (tmp == null)
				tmp = GetValue(lines, Headers.Location2);
			if (tmp != null)
				msg.Service.Location = tmp;

			msg.Service.Expiry = GetExpiry(lines);

			return msg;
		}

		private Messages.DiscoveryMessage GetDiscoveryMessage(List<string> lines)
		{
			if (lines[0].ToUpper() != "M-SEARCH * HTTP/1.1")
				return null;

			lines.RemoveAt(0);

			Messages.DiscoveryMessage msg = new Messages.DiscoveryMessage();
			msg.Service = new Service();

			if (!GetServiceType(msg, Headers.ServiceType, lines))
				return null;

			string tmp = GetValue(lines, "man");
			if (!tmp.Equals("\"ssdp:discover\"", StringComparison.InvariantCultureIgnoreCase))
				return null;

			tmp = GetValue(lines, Headers.MaxWaitTime);
			if (!string.IsNullOrEmpty(tmp))
			{
				msg.MaxWaitTime = int.Parse(tmp);
			}

			GetHost(lines, msg);

			return msg;
		}

		private Messages.MessageBase GetNotifyMessage(List<string> lines)
		{
			if (lines[0].ToUpper() != "NOTIFY * HTTP/1.1")
				return null;

			lines.RemoveAt(0);

			Messages.MessageBase result = null;
			string tmp = GetValue(lines, Headers.NotificationType);
			if (tmp.Equals("ssdp:alive", StringComparison.InvariantCultureIgnoreCase))
				result = GetAliveMessage(lines);
			else if (tmp.Equals("ssdp:byebye", StringComparison.InvariantCultureIgnoreCase))
				result = GetByeMessage(lines);
			else
				return null;

			if (!GetServiceType(result, Discovery.SSDP.Headers.NotifiedServiceType, lines))
				return null;

			if (!GetServiceName(result, lines))
				return null;
			GetHost(lines, result);

			return result;
		}

		private Messages.MessageBase GetByeMessage(List<string> lines)
		{
			return new Messages.ByeMessage(new Service());
		}

		private Messages.AliveMessage GetAliveMessage(List<string> lines)
		{
			var result = new Messages.AliveMessage(new Service());

			string tmp = GetValue(lines, Headers.Location);
			if (tmp == null)
				tmp = GetValue(lines, Headers.Location2);
			if (tmp != null)
				result.Service.Location = tmp;

			result.Service.Expiry = GetExpiry(lines);

			return result;
		}

		private List<string> GetLines(byte[] data)
		{
			StringReader sr = new StringReader(Encoding.UTF8.GetString(data));
			List<string> lines = new List<string>();

			string tmp = sr.ReadLine();
			while (tmp != null)
			{
				lines.Add(tmp);
				tmp = sr.ReadLine();
			}

			return lines;
		}

		private string GetValue(List<string> lines, string header)
		{
			string tmp = lines.Find(l => l.StartsWith(header, StringComparison.InvariantCultureIgnoreCase));

			if (tmp == null)
				return null;

			lines.Remove(tmp);

			int i = tmp.IndexOf(":");
			if (i < 0)
				return null;

			return tmp.Substring(i + 1).Trim();
		}

		private void CompleteMessage(Messages.MessageBase msg, List<string> lines)
		{
			if (lines.Count == 0)
				return;

			bool isBoundaryFound = false;
			foreach (var item in lines)
			{
				if (isBoundaryFound)
					msg.Content += item + "\r\n";
				else
				{
					isBoundaryFound = item == "";
					if (!isBoundaryFound)
						msg.Headers.Add(item);
				}
			}
		}

		private TimeSpan GetExpiry(List<string> lines)
		{
			string tmp = GetValue(lines, Headers.CacheControl);
			if (tmp == null)
				tmp = GetValue(lines, Headers.Expires);
			if (!string.IsNullOrEmpty(tmp))
			{
				Match m = Regex.Match(tmp, @"(?:max-age\s*=\s*)?(\d+)");
				if (m.Success)
					return new TimeSpan(0, 0, int.Parse(m.Groups[1].Value));
			}
			return new TimeSpan();
		}

		private void GetHost(List<string> lines, Messages.MessageBase message)
		{
			string tmp = GetValue(lines, "host");
			if (string.IsNullOrEmpty(tmp))
				return;

			int i = tmp.IndexOf(":");
			if (i >= 0)
			{
				message.Port = int.Parse(tmp.Substring(i + 1));
				message.Host = tmp.Substring(0, i);
			}
			else
				message.Host = tmp;
		}

		private bool GetServiceName(Messages.MessageBase message, List<string> lines)
		{
			string tmp = GetValue(lines, Headers.ServiceName);
			if (string.IsNullOrEmpty(tmp))
				return false;

			message.Service.UniqueServiceName = tmp;
			return true;
		}

		private bool GetServiceType(Messages.MessageBase message, string header, List<string> lines)
		{
			string tmp = GetValue(lines, header);
			if (string.IsNullOrEmpty(tmp))
				return false;

			message.Service.ServiceType = tmp;
			return true;
		}
	}
}