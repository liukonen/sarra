using System;
using System.Collections.Generic;
using System.Linq;
using Sarra.Objects;

namespace Sarra
{
    class Bot
    {
        private string _wiki;
        List<string> _Items;
        Dictionary<string, string> _Values;

        public Bot(string FileToLoad)
        {
            _wiki = System.IO.File.ReadAllText(FileToLoad);
        }

        /// <summary>
        /// Executes the Base Objects to get there values
        /// </summary>
        public void compile()
        {
            _Items = new List<string>(ParseTheWiki(_wiki));
            List<BaseObject> Objects = new List<BaseObject>(GenerateObjects(_Items.ToArray()));
            _Values = GetCommandToValue(Objects.ToArray(), _Items.ToArray());
        }

        /// <summary>
        /// Returns the compiled text from the bot
        /// </summary>
        /// <returns></returns>
        public string speak()
        {
            string value = _wiki;
            foreach (KeyValuePair<string, string> lineItem in _Values)
            {
               value= value.Replace(string.Concat("[", lineItem.Key, "]"), lineItem.Value);
            }
            return value;
        }

        #region Support Methods

        /// <summary>
        /// Grabs the baseobject based on the name that is called
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        private static BaseObject GetObject(Sarra.Objects.BaseObject[] objects, string Item)
        {
            BaseObject value = (from BaseObject b in objects where string.Equals(b.ObjectName(), Item.Substring(0, Item.IndexOf(".")), StringComparison.OrdinalIgnoreCase) select b).First();
            return value;
        }

        /// <summary>
        /// Generates Memory objects from the array of items
        /// </summary>
        /// <param name="ObjectNames">Array of object Names</param>
        /// <returns></returns>
        private static BaseObject[] GenerateObjects(string[] ObjectNames)
        {
            List<BaseObject> Objects = new List<BaseObject>();
            foreach (string obj in DistinctObjectNames(ObjectNames))
            {
                switch (obj.ToLower())
                {
                    case "greeting":
                        Objects.Add(new GreetingObject()); break;
                    case "weather":
                        Objects.Add(new WeatherObject()); break;
                    case "rss":
                        //Objects.Add(new Sarra.Objects.CompressionXPathDocument);
                        break;
                    default: break;
                }

            }
            return Objects.ToArray();
        }


        private static string[] DistinctObjectNames(string[] ObjectNames)
        {
            List<string> items = new List<string>((from string objectCall in ObjectNames select getObjectName(objectCall)).Distinct());
            return items.ToArray();
        }

        private static string getObjectName(string ObjectCall){ return ObjectCall.Substring(0, ObjectCall.IndexOf(".")); }
        
        private static Dictionary<string, string> GetCommandToValue(Sarra.Objects.BaseObject[] objects, string[] Items)
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();
            foreach (string Item in Items)
            {
                Sarra.Objects.BaseObject obj = GetObject(objects, Item);
                string enu = Item.Substring(Item.IndexOf(".")+1);
                returnValue.Add(Item, GetValue(obj, enu));
            }
            return returnValue;
        }

        private static string GetValue(Sarra.Objects.BaseObject obj, string command)
        {
            return obj.GetValue(obj.EnumNameToInt(command));
        }

        /// <summary>
        /// Grabs the unique method calls done inside of the wiki
        /// </summary>
        /// <param name="wiki"></param>
        /// <returns></returns>
        private static string[] ParseTheWiki(string wiki)
        {
            int startindex = 0;
            List<string> SubPhrases = new List<string>();
            while (startindex != -1) { startindex = wiki.IndexOf('[', startindex + 1); if (startindex > -1) { SubPhrases.Add(wiki.Substring(startindex)); } }
            List<string> UniqueItems = new List<string>((from string phrase in SubPhrases select phrase.Substring(1, phrase.IndexOf(']') - 1)).Distinct());
            return UniqueItems.ToArray();
        }
        #endregion
    }
}
