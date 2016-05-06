using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Telnet.UI
{
	static class Log
	{
		public static void debug(string s)
		{
			Console.WriteLine("[DEBUG]: " + s);
		}
		public static void info(string s)
		{
			Console.WriteLine("[INFO]: " + s);
		}
		public static void error(string s)
		{
			Console.WriteLine("[ERROR]: " + s);
		}
	}
		public static class Logger
	{

		public static void Log(String s)
		{
			Console.WriteLine(s);
		}
		public static bool Log(string format, params object[] args)
		{           
			var succeeded = false;
			var argRegex = new Regex(@"\{\d+\}");
			if ((args != null) && (argRegex.Matches(format).Count == args.Length)) {
				Console.WriteLine(format, args);
				succeeded = true;
			} else {
				Console.WriteLine(format);
			}
			return succeeded;
		}
	}
}

