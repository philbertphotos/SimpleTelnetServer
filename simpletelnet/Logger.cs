#region Copyright (c) 2016 jphilbert
//
// (C) Copyright 2016 jphilbert
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Active Web Solutions Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// jphilbert, its employees, contractors or agents.
#endregion

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

