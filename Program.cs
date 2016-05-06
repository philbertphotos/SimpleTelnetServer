#region Copyright (c) 2016 jphilbert
//
// (C) Copyright 2016 jphilbert
// TIME: 11:33 AM
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
// A23B4ECD-01F5-4FEE-8714-F77E21C9B2D2 5.1.0.5134-RC
#endregion

using System;
using Telnet.UI;
using System.Net;

namespace SimpleTelnetServer_master
{
	class Program
	{
		public static void Main(string[] args)
		{
			// TODO: Implement Functionality Here
			TelnetServer service = new TelnetServer(IPAddress.Any, 23);
		}
	}
}
