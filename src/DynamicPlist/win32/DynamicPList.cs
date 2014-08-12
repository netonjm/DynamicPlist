using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace System
{
	public static class StringExtension
	{
		public static float ToFloat(this string sender)
		{
			return float.Parse(sender, new CultureInfo("es-MX").NumberFormat);
		}

		

	}
}

namespace Data
{

	public class DynamicPList : Dictionary<string, dynamic>
	{

		public DynamicPList()
		{
		}

		public DynamicPList(string file)
		{
			Load(file);
		}

		public dynamic TryGetValue(string value)
		{
			dynamic output;
			if (TryGetValue(value, out output))
				return output;
			return null;
		}

		public void Load(string file)
		{
			Clear();

			XDocument doc = XDocument.Load(file);
			XElement plist = doc.Element("plist");
			XElement dict = plist.Element("dict");

			var dictElements = dict.Elements();
			Parse(this, dictElements);
		}

		private void Parse(DynamicPList dict, IEnumerable<XElement> elements)
		{
			for (int i = 0; i < elements.Count(); i += 2)
			{
				XElement key = elements.ElementAt(i);
				XElement val = elements.ElementAt(i + 1);

				dict[key.Value] = ParseValue(val);
			}
		}

		private List<dynamic> ParseArray(IEnumerable<XElement> elements)
		{
			List<dynamic> list = new List<dynamic>();
			foreach (XElement e in elements)
			{
				dynamic one = ParseValue(e);
				list.Add(one);
			}

			return list;
		}


		private dynamic ParseValue(XElement val)
		{
			switch (val.Name.ToString())
			{
				case "string":
					return val.Value;
				case "integer":
					return int.Parse(val.Value);
				case "real":
					return val.Value.ToFloat();
				case "true":
					return true;
				case "false":
					return false;
				case "dict":
					DynamicPList plist = new DynamicPList();
					Parse(plist, val.Elements());
					return plist;
				case "array":
					List<dynamic> list = ParseArray(val.Elements());
					return list;
				case "key":
					return val.Value.ToString();
				default:
					throw new ArgumentException("Unsupported");
			}
		}
	}
}