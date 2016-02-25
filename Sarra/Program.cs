using System;
using System.Speech.Synthesis;
using System.Xml.XPath;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace Sarra
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using(	System.Speech.Synthesis.SpeechSynthesizer voice = new System.Speech.Synthesis.SpeechSynthesizer ())
			{
			voice.SelectVoiceByHints (System.Speech.Synthesis.VoiceGender.Female);

				string Greeting = GreetTheUser ();
				voice.Speak(Greeting);
			//Console.WriteLine ("Hello World!");
			}
		}


		private static string GreetTheUser()
		{
			List<BaseObject> L = new List<BaseObject> ();
			L.Add (new Weather (System.Configuration.ConfigurationManager.AppSettings.GetValues ("WeatherLocation") [0]));
			L.Add(new News("http://www.foxnews.com/xmlfeed/rss/0,4313,0,00.rss", "Fox News", 2));
			System.Threading.Tasks.ParallelOptions I = new System.Threading.Tasks.ParallelOptions();
			I.MaxDegreeOfParallelism = 4;
			Parallel.ForEach(L, I, (BaseObject r) =>
				{
					try {
						r.CallPath();

					} catch {
					}
				});
			StringBuilder d = new StringBuilder();
			d.Append(Greeting.Greet());

			d.Append(" Here is the Weather.");
			d.Append(L[0].Value());
			d.Append(L[1].Value());
			return d.ToString();
		}


	}



	public abstract class BaseObject
	{
		private XPathDocument _Value;
		private string Path;
		public XPathDocument document {get { return _Value; }}

		public virtual void CallPath(){_Value = new XPathDocument(Path);}

		public abstract string Value();

		public string[] GetValues(string path, int Count)
		{
			XPathNavigator Navigator = _Value.CreateNavigator();
			XPathNodeIterator iTER = Navigator.Select(path);
			int Counter = Count;
			if (iTER.Count < Count)
				Counter = iTER.Count;
			
			List<string> Results = new List<string>();
			for (int I = 0; I <= Counter - 1; I++) {
				iTER.MoveNext ();
				Results.Add (iTER.Current.Value);
			}
			return Results.ToArray();
		}

		public BaseObject(string PlaceToGo){Path = PlaceToGo;}
	}



	public sealed class Greeting
	{
		//private Greeting(){}
		public static string Greet()
		{
			System.Text.StringBuilder output = new StringBuilder ();
			output.Append ("Good ");
			int Hour = System.DateTime.Now.TimeOfDay.Hours;
			if (Hour > 18) {
				output.Append ("evening. ");
			} else if (Hour > 12) {
				output.Append ("afternoon. ");
			}
			else{output.Append ("morning. ");}
			output.Append ("It's ").Append (DateTime.Now.ToShortTimeString ().Replace ("AM", "A M ")).Append (".");
			return output.ToString ();
		}


	}

	public sealed partial class Weather: BaseObject
	{
		public Weather(string Zip): base(string.Concat("http://www.rssweather.com/zipcode/", Zip, "/rss.php"))
		{
		}

		public override string Value ()
		{
			try{
				List<string> Titles = new List<string>(base.GetValues("rss/channel/item/title", 2));
				List<string> newsNodes = new List<string>(base.GetValues("rss/channel/item/description", 2));
				StringBuilder strb = new StringBuilder();
				strb.Append(" Right now it's ");
				strb.Append(newsNodes[0].Replace("F ", " degrees "));
				strb.Append(". ").Append(Titles[1]).Append(", ");
				strb.Append(newsNodes[1]).Append(" ").Append(Titles[2]).Append(",").Append(newsNodes[2]);
				return strb.ToString();

			}
			catch
			{
				return "There was an issue returning the weather. A developer should look into this.";
			}
		}
	}

	public sealed partial class News: BaseObject
	{
		private string _SourceName; private int _ItemCount; 

		public News(string Path, string SourceName, int ItemCount): base(Path)
		{
			_SourceName = SourceName; _ItemCount = ItemCount;
		}
private static string getdescriptions(XmlDocument xmldoc, int maxvalue)
	{
		StringBuilder strb = new StringBuilder();
	XmlNodeList newsnodes = xmldoc.SelectNodes("rss/channel/item/description");
		if (newsnodes.Count > maxvalue) {
			for (int i = 0; i <= maxvalue - 1; i++) {
					strb.Append (" News Story Number ").Append ((i + 1).ToString ());
					strb.Append(". ");
					string NNIT = newsnodes [i].InnerText;
					string FF = FilterFox (NNIT);
					strb.Append (FF);
					strb.Append(" ");
			}
		} else {
			foreach (XmlNode node in newsnodes) {strb.Append(node.InnerText).Append(" ");}
		}
			return StripTags(strb.ToString());
	}

		private static string FilterFox(string I)
		{
			if (I.Contains("<li>"))
			{return I.Substring (0, I.IndexOf ("<li>"));}
			return I;
		}

private static string StripTags(string StringToBeStriped)
	{
		StringToBeStriped = StringToBeStriped.Replace("<![CDATA[", string.Empty);
		StringToBeStriped = StringToBeStriped.Replace("]]>", string.Empty);
		Regex objRegEx = new Regex("<[^>]*>");
		string SB = objRegEx.Replace(StringToBeStriped, string.Empty);
		if (SB.Contains("http://")) {
			int startindex = SB.IndexOf("http://");
			int EndIndex = SB.IndexOf(" ", startindex);
			if (EndIndex == -1)
				EndIndex = SB.Length;
			SB.Remove(startindex, EndIndex - startindex);
		}
		return SB;
	}
public override string Value()
	{
		StringBuilder StrB = new StringBuilder();
		List<string> NewsNodes = new List<string>(base.GetValues("rss/channel/item/description", _ItemCount));

		StrB.Append("News From ");
		StrB.Append(_SourceName);
		StrB.Append(" ");
		int I = 1;
		foreach (string S in NewsNodes) {
			StrB.Append(" News Story Number ");
				StrB.Append(I.ToString());
			StrB.Append(". ");
			StrB.Append(FilterFox(S));
			StrB.Append(" ");
			I += 1;
		}
		StrB.Append(". ");
			return StripTags(StrB.ToString());
	}

	}

}
