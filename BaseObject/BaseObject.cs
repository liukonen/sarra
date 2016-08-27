using System.Collections.Generic;
using System.Xml.XPath;
using System;

namespace Sarra.Objects
{
    interface IBaseObject { void preFetch(); string GetValue(int ValueCode); string ObjectName(); }
    public abstract class BaseObject : IBaseObject {public virtual string ObjectName() { return "baseObject"; } public virtual void preFetch() { } public abstract string GetValue(int Value); public abstract int EnumNameToInt(string EnumName); }

    public abstract class XPathBaseObject : BaseObject
    {
        public XPathDocument _Value;
        public string Path;
        public override string ObjectName() { return "XPathBase"; }
        public override void preFetch() { _Value = new XPathDocument(Path); }
        public XPathBaseObject(string PlaceToGo) { Path = PlaceToGo; }

        public XPathDocument document { get { return _Value; } }
        public string[] GetValues(string path, int Count)
        {
            XPathNavigator navigator = _Value.CreateNavigator();
            XPathNodeIterator iter = navigator.Select(path);
            System.Collections.Generic.List<string> results = new List<string>();

            int counter = 0;
            while (iter.MoveNext())
            {
                if (Count > counter)
                {
                    results.Add(iter.Current.Value);
                }
                counter++;
            }
            return results.ToArray();
        }
    }

    public abstract class CompressionXPathDocument : XPathBaseObject
    {
        public override string ObjectName() { return "CompressionXPath"; }
        public override void preFetch()
        {
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(Path);
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            using (System.Net.WebResponse response = request.GetResponse())
            {
                _Value = new XPathDocument(response.GetResponseStream());
            }

        }
        public CompressionXPathDocument(string PlaceToGo) : base(PlaceToGo) { }

    }

    public class GreetingObject : BaseObject
    {
        public override string ObjectName() { return "greeting"; }
        enum Types : int { timeofday = 1, name = 2, time = 3, readabletime = 4 }
        public override int EnumNameToInt(string EnumName) { return (int)Enum.Parse(typeof(Types), EnumName.ToLower()); }
        public override string GetValue(int Value)
        {
            string val = string.Empty;
            switch ((Types)Value)
            {
                case Types.name:
                    val = System.Configuration.ConfigurationManager.AppSettings["Greeting.Name"];
                    break;
                case Types.timeofday:
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    int hour = DateTime.Now.TimeOfDay.Hours;
                    if (hour > 18) { val = "evening"; }
                    else if (hour > 12) { val = "afternoon"; }
                    else { val = "morning"; }
                    break;
                case Types.time:
                    val = DateTime.Now.ToShortTimeString();
                    break;
                case Types.readabletime:
                    val = DateTime.Now.ToShortTimeString().Replace("AM", "A M ");
                    break;
                default: break;

            }
            return val;
        }

    }
}
