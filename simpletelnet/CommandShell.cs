#region Copyright (c) 2016 jphilbert
//
// (C) Copyright 2016 jphilbert
// TIME: 1:36 AM
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
// 1DF0B32E-CBE7-443A-A33C-60A724053D5C 5.1.0.5134-RC
#endregion

using System;
using Telnet.UI;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Telnet.UI
{
	internal static class TelnetShell
	{
		Tools tools = new Tools();
		Color colors = new Color();
		
		public static void StartCmd()
		{
			List<String> cmd = new List<string>();          // command strings
			List<byte> accumcmd = new List<byte>();         // accumulation buffer for building cmds
			List<int> RunningClear = new List<int>();       // list of chars to remove
			Stack<int> cmdList = new Stack<int>(); 			// we use a stack so we don't disturb the ordering of the bytes we're removing
			List<string> cmdHistory = new List<string>(); 	// command history
			int cmdloc = new int();
			var command = string.Empty;
			bool cmdsent = false;
			bool firstconnect = true;

			try {
				byte[] buffer = tools.ReadStream();
				tools.Negotiate();
				tools.ScanBuffer(buffer);
				
				tools.Clearscreen();
				
				var tid = Thread.CurrentThread.ManagedThreadId;
				var gs = TelnetServer.tcpStream[tid];
				while (TelnetServer.Connected[tid]) {
					try {
						int i = gs.Read(buffer, 0, buffer.Length);
						var buf = Encoding.Default.GetString(buffer, 0, i);
						var bufarr = tools.GetBytes(buf);
						
						//First connetion settings.
						if (firstconnect) {
							tools.Login();
							tools.Clearscreen();
							tools.Banner();
							tools.Prompt();
						}
						
						
						//Key Logic
						switch (bufarr[0]) {
							case 32: //Line Feed or Space Bar
								tools.write(buf);
								cmd.Add(" ");
								break;
							case 13: //Enter Key
								
								if (cmd.Any()) {
									foreach (String str in cmd) {
										command = command + str;
									}
									cmdsent = true;
									cmdHistory.Add(command);
								}
								if (buf.Equals(Environment.NewLine, StringComparison.Ordinal)) {
									
									if (cmdsent) {
										tools.writeline("");
										tools.getpos();
									} else {
										tools.writeline("");
										//tools.write("1");
										tools.Prompt();
										tools.getpos();
									}
									cmdsent = false;
								}
								
								cmd.Clear();
								accumcmd.Clear();
								break;
							case 126: //Delete Key
								break;
							case 127: //Backspace
								if (cmd.Count != 0)
									tools.EraseBackspace();
								// delete

								if (cmd.Any()) {
									cmd.RemoveAt((cmd.Count() - 1));
								}
								RunningClear.Add(i);
								break;
							case 27: //Arrows and other Function Keys
								var cmdcount = cmdHistory.Count;
								if (buf.Contains("[A")) {  //UP
									lock (cmdHistory) {
										if (cmdHistory.Any()) {
											if (cmdloc <= -1 || cmdloc >= cmdHistory.Count())
												cmdloc = 0;
											
											tools.set_cursor_loc(TelnetServer.Postion[tid][0], 3);
											tools.clear_right_cursor();
											Logger.Log(string.Format("{0} : {1}", cmdloc, cmdHistory.Count()));
											string uu = cmdHistory.ElementAt(cmdloc);
											tools.write(uu);
											cmd.Clear();
											for (int xx = 0; xx < uu.Length; xx++) {
												cmd.Add(uu[xx].ToString());
											}
											
											
											//command = cmdHistory.ElementAt(cmdloc);
											//cmd.Add(cmdHistory.ElementAt(cmdloc));
											cmdloc++;
										}
									}
								}
								
								if (buf.Contains("[B")) { //DOWN
									lock (cmdHistory) {
										if (cmdHistory.Any()) {
											if (cmdloc <= -1 || cmdloc > cmdHistory.Count() - 1)
												cmdloc = cmdHistory.Count() - 1;
											Logger.Log(string.Format("{0} : {1}", cmdloc, cmdHistory.Count()));
										
											tools.set_cursor_loc(TelnetServer.Postion[tid][0], 3);
											tools.clear_right_cursor();
											string uu = cmdHistory.ElementAt(cmdloc);
											tools.write(uu);
											cmd.Clear();
											for (int xx = 0; xx < uu.Length; xx++) {
												cmd.Add(uu[xx].ToString());
											}
											//command = cmdHistory.ElementAt(cmdloc);
											//cmd.Add(cmdHistory.ElementAt(cmdloc));
											cmdloc--;
										}
									}
								}
								
								if (buf.Contains("[D")) //LEFT
									Logger.Log("LEFT");
								
								if (buf.Contains("[C")) //RIGHT
									Logger.Log("RIGHT");
								switch (buf) {
										
									case "[A":
										Logger.Log("UP");
										break;
										
									default:
										break;
								}
								//Logger.Log();
								break;
							default:
								if (tools.IsASCII(buf)) {
									tools.write(buf);
									lock (cmd) {
										cmd.Add(buf);
									}
									accumcmd.Add(bufarr[0]);
									RunningClear.Add(i);
									foreach (int r in RunningClear) {
										cmdList.Push(r);
									}
								}
								break;
						}
						
						Logger.Log(buf + ": " + bufarr[0]);
						
						//Logger.Log("cmd: " + command);

						if (command.Length >= 1) {
							switch (command) {
								case "clear":
									tools.Clearscreen();
									tools.Prompt();
									Console.Clear();
									break;

								case "help":
									colors.setForegroundColor(ConsoleColor.Cyan);
									tools.writeline("Basic Shell Commands - try `help full` for all commands");
									tools.writeline("=======================================================");
									colors.reset_color();
									tools.Prompt();
									tools.getpos();
									break;
								case "list":
									tools.writeline("List");
									tools.writeline("=======================================================");
									tools.writeline("");
									tools.Prompt();
									tools.getpos();
									break;
						
								case "users":
									tools.writeline("users");
									tools.writeline("=======================================================");
									tools.writeline("");
									break;
								case "ansi":
									tools.writeline("ANSI");
									tools.writeline("=======================================================");
									tools.Prompt();
									tools.getpos();
									break;
							
								case "type":
									tools.writeline("TYPE");
									tools.writeline("=======================================================");
									tools.writeline("");
									tools.Prompt();
									tools.getpos();
									break;
								default:
									if (TelnetServer.Connected[tid]) {
										colors.setForegroundColor(ConsoleColor.Red);
										tools.writeline(String.Format("{0} {1}", command, "Does Not Exsit"));
										tools.Prompt();
										tools.getpos();
										colors.reset_color();
									}
									break;
							}
						}
                        }
						tools.ScanBuffer(buffer);
						command = string.Empty;

					} catch (Exception ex) {
						Logger.Log("Error: " + ex.Message);
					}
					firstconnect = false;
				}//ENDWHILE

			} catch (Exception ex) {
				Logger.Log("Main Try: " + ex.Message);
				//Logger.Log("Client (" + tcc.GetHashCode() + ") Closed the session");
			}
		}

		public static void LogMessage(string msg)
		{
			//ConsoleColor.DarkGreen;
			//Console.ForegroundColor = ConsoleColor.DarkGreen;
			tools.writeline(msg);
		}
	}
}
