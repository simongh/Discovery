using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discovery.Ssdp
{
	internal class MessageParser
	{
		public Messages.MessageBase Parse(byte[] data)
		{
			using (var reader = new StreamReader(new MemoryStream(data)))
			{
				var line = reader.ReadLine();
				if (line == null)
					return null;
				var lines = Parse(reader);

				Messages.MessageBase result = TryGetDiscoveryMessage(line, lines);
				if (result == null)
					result = GetNotifyMessage(line, lines);
				if (result == null)
					result = GetHttpMessage(line, lines);
				//if (result == null)
				//	return null;

				//CompleteMessage(result, reader);
				return result;
			}
		}

		private IDictionary<string, string> Parse(StreamReader reader)
		{
			var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			char value;

			var s = new Queue<char>();
			while ((value = (char)reader.Read()) > -1 && !reader.EndOfStream)
			{
				if (value == ':')
				{
					if (reader.Peek() == ' ')
						reader.Read();

					result.Add(new string(s.ToArray()), reader.ReadLine());
					s.Clear();
				}
				else if (value == '\r' && s.Count == 0)
				{
					reader.Read();
					result.Add("", reader.ReadToEnd());
				}
				else
					s.Enqueue(value);
			}

			return result;
		}

		private Messages.DiscoveryResponseMessage GetHttpMessage(string line, IDictionary<string, string> headers)
		{
			if (line?.StartsWith("HTTP/1.1", StringComparison.OrdinalIgnoreCase) != true)
				return null;

			var tmp = line.Substring(8).Trim();
			if (tmp.Length < 3)
				return null;

			if (tmp.Substring(0, 3) != "200")
				throw new DiscoveryException(int.Parse(line.Substring(0, 3)));

			var msg = new Messages.DiscoveryResponseMessage();
			msg.Service = new ServiceDescription();

			//	//if (!GetServiceName(msg, lines))
			//	//	return null;

			//	//if (!GetServiceType(msg, HeaderNames.ServiceType, lines))
			//	//	return null;

			//	//tmp = GetValue(lines, "ext");
			//	//if (tmp == null)
			//	//	return null;

			//	//tmp = GetValue(lines, HeaderNames.Location);
			//	//if (tmp == null)
			//	//	tmp = GetValue(lines, HeaderNames.Location2);
			//	//if (tmp != null)
			//	//	msg.Service.Location = tmp;

			//	//msg.Service.Expiry = GetExpiry(lines);

			return msg;
		}

		private Messages.DiscoveryMessage TryGetDiscoveryMessage(string line, IDictionary<string, string> headers)
		{
			if (!string.Equals(line, "M-SEARCH * HTTP/1.1", StringComparison.OrdinalIgnoreCase))
				return null;

			if (!headers.ContainsKey("man") || !headers["man"].Equals("\"ssdp:discover\"", StringComparison.OrdinalIgnoreCase))
				return null;

			var msg = new Messages.DiscoveryMessage();
			msg.Service = new ServiceDescription();

			foreach (var item in headers)
			{
				if (item.Key.Equals(HeaderNames.ServiceType, StringComparison.OrdinalIgnoreCase))
					msg.Service.ServiceType = item.Value;
				else if (item.Key.Equals(HeaderNames.MaxWaitTime, StringComparison.OrdinalIgnoreCase))
					msg.MaxWaitTime = int.Parse(item.Value);
				else if (item.Key.Equals(HeaderNames.Host, StringComparison.OrdinalIgnoreCase))
					SetHost(msg, item.Value);
				else if (item.Key == "")
					msg.Content = item.Value;
				else
					msg.Headers.Add(item.Key, item.Value);
			}

			return msg;
		}

		private void SetHost(Messages.MessageBase message, string value)
		{
			var i = value.IndexOf(":");
			if (i >= 0)
			{
				message.Port = int.Parse(value.Substring(i + 1));
				message.Host = value.Substring(0, i);
			}
			else
				message.Host = value;
		}

		private Messages.MessageBase GetNotifyMessage(string line, IDictionary<string, string> headers)
		{
			if (!string.Equals(line, "NOTIFY * HTTP/1.1", StringComparison.OrdinalIgnoreCase))
				return null;

			if (!headers.ContainsKey(HeaderNames.NotificationType))
				return null;

			Messages.MessageBase result = null;

			if (headers[HeaderNames.NotificationType].Equals("ssdp:alive", StringComparison.OrdinalIgnoreCase))
				result = new Messages.AliveMessage(new ServiceDescription());
			else if (headers[HeaderNames.NotificationType].Equals("ssdp:byebye", StringComparison.OrdinalIgnoreCase))
				result = new Messages.ByeMessage(new ServiceDescription());
			else
				return null;

			foreach (var item in headers)
			{
				if (new[] { HeaderNames.Location, HeaderNames.Location2 }.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
					result.Service.Location = item.Value;
				else if (new[] { HeaderNames.CacheControl, HeaderNames.Expires }.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
				{
					var m = Regex.Match(item.Key, @"(?:max-age\s*=\s*)?(\d+)");
					if (m.Success)
						result.Service.Expiry = new TimeSpan(0, 0, int.Parse(m.Groups[1].Value));
				}
				else if (item.Key.Equals(HeaderNames.NotifiedServiceType, StringComparison.OrdinalIgnoreCase))
					result.Service.ServiceType = item.Value;
				else if (item.Key.Equals(HeaderNames.ServiceName, StringComparison.OrdinalIgnoreCase))
					result.Service.UniqueServiceName = item.Value;
				else if (item.Key.Equals(HeaderNames.Host, StringComparison.OrdinalIgnoreCase))
					SetHost(result, item.Value);
				else if (item.Key == "")
					result.Content = item.Value;
				else
					result.Headers.Add(item.Key, item.Value);
			}

			return result;
		}

		//private Messages.MessageBase GetByeMessage(ICollection<string> lines)
		//{
		//	return new Messages.ByeMessage(new ServiceDescription());
		//}

		//private Messages.AliveMessage GetAliveMessage(IDictionary<string, string> lines)
		//{
		//	var result = new Messages.AliveMessage(new ServiceDescription());

		//	foreach (var item in lines)
		//	{
		//		if (new[] { HeaderNames.Location, HeaderNames.Location2 }.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
		//			result.Service.Location = item.Value;
		//		else if (new[] { HeaderNames.CacheControl, HeaderNames.Expires }.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
		//		{
		//			var m = Regex.Match(lines[key], @"(?:max-age\s*=\s*)?(\d+)");
		//			if (m.Success)
		//				result.Service.Expiry = new TimeSpan(0, 0, int.Parse(m.Groups[1].Value));
		//		}
		//	}

		//	return result;
		//}

		//private ICollection<string> GetLines(byte[] data)
		//{
		//	var sr = new StringReader(Encoding.UTF8.GetString(data));
		//	var lines = new List<string>();

		//	var tmp = sr.ReadLine();
		//	while (tmp != null)
		//	{
		//		lines.Add(tmp);
		//		tmp = sr.ReadLine();
		//	}

		//	return lines;
		//}

		//private string GetValue(ICollection<string> lines, string header)
		//{
		//	var tmp = lines.FirstOrDefault(l => l.StartsWith(header, StringComparison.OrdinalIgnoreCase));

		//	if (tmp == null)
		//		return null;

		//	lines.Remove(tmp);

		//	var i = tmp.IndexOf(":");
		//	if (i < 0)
		//		return null;

		//	return tmp.Substring(i + 1).Trim();
		//}

		//private void CompleteMessage(Messages.MessageBase msg, StringReader reader)
		//{
		//	//if (lines.Count == 0)
		//	//	return;

		//	//var isBoundaryFound = false;
		//	//foreach (var item in lines)
		//	//{
		//	//	if (isBoundaryFound)
		//	//		msg.Content += item + "\r\n";
		//	//	else
		//	//	{
		//	//		isBoundaryFound = item == "";
		//	//		if (!isBoundaryFound)
		//	//			AddHeader(item, msg);
		//	//	}
		//	//}
		//}

		//private void AddHeader(string line, Messages.MessageBase message)
		//{
		//	var parts = line.Split(':');
		//	message.Headers.Add(parts[0].Trim(), string.Join(":", parts.Skip(1)));
		//}

		//private TimeSpan GetExpiry(ICollection<string> lines)
		//{
		//	var tmp = GetValue(lines, HeaderNames.CacheControl);
		//	if (tmp == null)
		//		tmp = GetValue(lines, HeaderNames.Expires);
		//	if (!string.IsNullOrEmpty(tmp))
		//	{
		//		var m = Regex.Match(tmp, @"(?:max-age\s*=\s*)?(\d+)");
		//		if (m.Success)
		//			return new TimeSpan(0, 0, int.Parse(m.Groups[1].Value));
		//	}
		//	return new TimeSpan();
		//}

		//private void GetHost(Messages.MessageBase message, IDictionary<string, string> lines)
		//{
		//	if (!lines.ContainsKey(HeaderNames.Host))
		//		return;

		//	var i = lines[HeaderNames.Host].IndexOf(":");
		//	if (i >= 0)
		//	{
		//		message.Port = int.Parse(lines[HeaderNames.Host].Substring(i + 1));
		//		message.Host = lines[HeaderNames.Host].Substring(0, i);
		//	}
		//	else
		//		message.Host = lines[HeaderNames.Host];
		//}

		//private bool GetServiceName(Messages.MessageBase message, IDictionary<string, string> lines)
		//{
		//	if (!lines.ContainsKey(HeaderNames.ServiceName))
		//		return false;

		//	message.Service.UniqueServiceName = lines[HeaderNames.ServiceName];
		//	return true;
		//}

		//private bool GetServiceType(Messages.MessageBase message, string header, IDictionary<string, string> lines)
		//{
		//	if (!lines.ContainsKey(header))
		//		return false;

		//	message.Service.ServiceType = lines[header];
		//	return true;
		//}
	}
}