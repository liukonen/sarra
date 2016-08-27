using System.Collections.Generic;
using System;

namespace Sarra.Objects
{
    public class WeatherObject:Sarra.Objects.CompressionXPathDocument
    {
        List<string> titles;
        List<string> newsNodes;

       public override string ObjectName() { return "Weather"; }
       public enum WeatherTypes:int
        {
            current = 0, forcastdescription = 1, forcastvalue = 2
        }
        public WeatherObject() : base(string.Concat("http://www.rssweather.com/zipcode/", System.Configuration.ConfigurationManager.AppSettings["Weather.Location"], "/rss.php"))
        {
            preFetch();
            titles = new List<string>(base.GetValues("rss/channel/item/title", 2));
            newsNodes = new List<string>(base.GetValues("rss/channel/item/description", 2));
        }
        public override string GetValue(int Value)
        {
            string returnvalue = string.Empty;
            switch ((WeatherTypes)Value)
                {
                case WeatherTypes.current:
                    returnvalue =  newsNodes[0].Replace("F ", " Degrees ");
                    break;
                case WeatherTypes.forcastdescription:
                    returnvalue =  titles[1];
                    break;
                case WeatherTypes.forcastvalue:
                    returnvalue = newsNodes[1];
                    break;
                default:
                    break;

            }
            return returnvalue;
        }

        public override int EnumNameToInt(string EnumName)
        {
          return  (int)Enum.Parse(typeof(WeatherTypes), EnumName);
           
        }
    }
}
