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
                voice.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female);
                Sarra.Bot Robot = new Bot("wiki.txt");
                Robot.compile();
                voice.Speak(Robot.speak());
			}
		}






	}

}
