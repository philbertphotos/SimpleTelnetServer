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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Telnet.UI
{
	public class Tools
	{
		private const int TimeOutMs = 100;
		public string password = "password";
		public List<byte> ClientBuf;

		Color colors = new Color();

		private struct Codes
		{
			/* TELNET commands */
			public const byte GA = 249;
			public const byte WILL = 251;
			public const byte WONT = 252;
			public const byte DO = 253;
			public const byte DONT = 254;
			public const byte IAC = 255;
			public const byte NAWS = 31;
			public const byte NEW_ENVIRON = 39;
			public const byte TTYPE = 24;
			public const byte TSPEED = 32;
			public const byte LINEMODE = 34;
			public static byte NOP = 241;
			// no operation
			public static byte SB = 250;
			// begin subnegotiation
			public static byte SE = 240;
			// end subnegotiation parameters

			/* TELNET options */
			public const byte ECHO = 1;
			public const byte SUPP = 3;
			public const byte ESC = 0x1B;
		}
		#region Screen output correlation

		//--Screen output
		public void ask_no_clinet_echo()
		{
			writebytes(new byte[] { 0xff, 251, 1 });
		}
		
		public void ask_clinet_echo()
		{
			writebytes(new byte[] { 0xff, 252, 1 });
		}
		
		public void DisplayReceivedData(byte[] data, int dataLength)
		{
			for (int i = 0; i < dataLength; ++i) {
				byte dataByte = data[i];

				if (dataByte != 0) {
					switch (dataByte) {
						case TelnetHelper.SB:
							Console.Write("SB:   ");
							break;
						case TelnetHelper.SE:
							Console.Write("SE:    ");
							break;
						case TelnetHelper.WILL:
							Console.Write("WILL:  ");
							break;
						case TelnetHelper.WONT:
							Console.Write("WON'T: ");
							break;
						case TelnetHelper.DO:
							Console.Write("DO:    ");
							break;
						case TelnetHelper.DONT:
							Console.Write("DON'T: ");
							break;
						case TelnetHelper.IAC:
							Console.Write("\nIAC  ");
							break;
						case TelnetHelper.ESC:
							Console.Write("\nESC: ");
							break;
						default:
							Console.Write(TelnetHelper.GetOptionDescription(dataByte) + " ");
							break;
					} //dataByte.ToString("X")
				}
			}

			Logger.Log("");
		}
		
		public string GetPassword(int max_length, char password_char)
		{
			int readclinet = 0;
			List<byte> get_line = new List<byte>();
			while (readclinet != '\r') {
				readclinet = TelnetServer.tcpClient.GetStream().ReadByte();
				if (readclinet != '\r') {
					if (readclinet != 0x08 && readclinet != 0x7f && get_line.Count < max_length)
						TelnetServer.tcpClient.GetStream().WriteByte((byte)'*');
					if (readclinet == 0x08 && get_line.Count != 0)
						TelnetServer.tcpClient.GetStream().Write(new byte[] { 0x08, 0x20, 08 }, 0, 3);

					if (readclinet == 0x7f && get_line.Count != 0)
						TelnetServer.tcpClient.GetStream().Write(new byte[] { 0x08, 0x20, 08 }, 0, 3);
				}
				if (readclinet != 0x08 && readclinet != 0x7f && get_line.Count < max_length)
					get_line.Add((byte)readclinet);
				if ((readclinet == 0x08 || readclinet == 0x7f) && get_line.Count != 0)
					get_line.RemoveAt(get_line.Count - 1);
			}
			return Encoding.GetEncoding("Big5").GetString(get_line.ToArray()).Replace("\r", "");
		}
			
		public void getpos()
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'6',
				(byte)'n'
			});
		}
		//Write string in pos
		public void writepos(string str, int x, int y)
		{
			byte[] buf_str = Encoding.GetEncoding("Big5").GetBytes(str);
			int buf_len = buf_str.Length;
			List<byte> buf_loc = Encoding.GetEncoding("Big5").GetBytes("[" + x + ";" + y + "H").ToList();
			buf_loc.Insert(0, 0x1b);
			TelnetServer.tcpClient.GetStream().Write(buf_loc.ToArray(), 0, buf_loc.Count);
			TelnetServer.tcpClient.GetStream().Write(buf_str, 0, buf_len);
		}
		
		//Write string with no CR
		public void write(string str)
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamWriter streamWriter = new StreamWriter(sw);
			streamWriter.Write(str);
			streamWriter.Flush();
		}
		
		//Write string with CR
		public void writeline(string str)
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamWriter streamWriter = new StreamWriter(sw);
			streamWriter.WriteLine(str);
			streamWriter.Flush();
		}
		
		//Write Byte
		public void writebyte(byte bit)
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamWriter streamWriter = new StreamWriter(sw);
			sw.WriteByte(bit);
		}
		
		public void writebytes(byte[] bytes)
		{
			foreach (byte b in bytes) {
				writebyte(b);
			}
		}
		
		public bool IsASCII(string value)
		{
			// ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
			return Encoding.UTF8.GetByteCount(value) == value.Length;
		}
		
		public void EraseBackspace()
		{
			writebytes(new byte[] {
				0x08,
				0x20,
				0x08,
			});
		}
		
		public int Prompt()
		{
			colors.setForegroundColor(ConsoleColor.White);
			//string prm = Environment.MachineName + "# ";
			string prm = "> ";
			write(prm);
			colors.reset_color();
			return prm.Length;
		}
		
		public bool Login()
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamReader streamReader = new StreamReader(sw);

			for (int tries = 0; tries < 3; tries++) {
				write("Password: ");
				string response = streamReader.ReadLine();
				Logger.Log(response + " : " + tries);

				if (response.Contains("password")) {
					return true;
				}
				if (tries == 2) {
					Logger.Log("CLOSE LOGIN");
					Close();
					break;
				}
				writeline("");
			}

			return false;
		}
		
		public void Banner()
		{
			
			writeline(string.Format("{0} {1}", Environment.OSVersion, Environment.MachineName));
		}
		
		public string Read()
		{
			if (!TelnetServer.tcpClient.Connected)
				return null;
			var sb = new StringBuilder();
			do {
				ParseTelnet(sb);
				Thread.Sleep(TimeOutMs);
			} while (TelnetServer.tcpClient.Available > 0);
			return sb.ToString();
		}
		
		public String GetNvtString(byte b)
		{
			switch (b) {
				case 1:
					return ("ECHO");
				case 2:
					return ("RECONNECTION");
				case 3:
					return ("SUPPRESS_GOAHEAD");
				case 24:
					return ("Terminal Type");
				case 31:
					return ("Windows Size");
				case 32:
					return ("Terminal Speed");
				case 34:
					return ("LINEMODE");
				case 35:
					return ("X_DISPLAY_LOC");
				case 39:
					return ("Environ Option");
				case 240:
					return ("SE");
				case 241:
					return ("NOP");
				case 250:
					return ("SB");
				case 251:
					return ("WILL");
				case 252:
					return ("WONT");
				case 253:
					return ("DO");
				case 254:
					return ("DONT");
				case 255:
					return ("IAC");
				default:
					return (Convert.ToString(b));
			}
		}
		#region Telnet sub-responses as WILL, WONT ..
		
		/// <summary>
		/// Add a "ESC" response, e.g. "WILL negotiate about terminal size"
		/// </summary>
		/// <param name = "cmd1"></param>
		/// <param name = "cmd2"></param>
		public void TelnetESC(byte cmd1, byte cmd2)
		{
			writebytes(new [] { Codes.ESC, cmd1, cmd2 });
		}
		
		/// <summary>
		/// Add a "WILL" response, e.g. "WILL negotiate about terminal size"
		/// </summary>
		/// <param name="willDoWhat"></param>
		public void TelnetWill(byte willDoWhat)
		{
			writebytes(new [] {
				Codes.IAC,
				Codes.WILL,
				willDoWhat
			});
		}

		/// <summary>
		/// Add a "WONT" response, e.g. "WONT negotiate about terminal size"
		/// </summary>
		/// <param name="wontDoWhat"></param>
		public void TelnetWont(byte wontDoWhat)
		{
			writebytes(new [] {
				Codes.IAC,
				Codes.WONT,
				wontDoWhat
			});
		}

		/// <summary>
		/// Add a "DO" response, e.g. "DO ..."
		/// </summary>
		/// <param name="doWhat"></param>
		public void TelnetDo(byte doWhat)
		{
			writebytes(new [] {
				Codes.IAC,
				Codes.DO,
				doWhat
			});
		}

		/// <summary>
		/// Add a "DONT" response, e.g. "DONT ..."
		/// </summary>
		/// <param name="dontDoWhat"></param>
		public void TelnetDont(byte dontDoWhat)
		{
			writebytes(new byte[] {
				Codes.IAC,
				Codes.DONT,
				dontDoWhat
			});
		}
		
		#endregion Telnet sub-responses as WILL, WONT ..
		// TODO: Update the negotiate logic
		public void Negotiate()
		{
			Logger.Log("Negotiating...");
			TelnetDo(Codes.TTYPE);
			TelnetDo(Codes.NAWS);
			TelnetWill(Codes.ECHO);
			TelnetDont(Codes.TSPEED);
			TelnetDont(Codes.NEW_ENVIRON);
			TelnetDo(Codes.SUPP);
			TelnetWill(Codes.SUPP);
			TelnetWont(Codes.LINEMODE);
		}
		
		public void HandleClientComm(int bytesRead)
		{
			NetworkStream clientStream = TelnetServer.tcpClient.GetStream();

			byte[] message = new byte[4096];
			byte[] cmd = new byte[4096];
			int cmdcnt;
			int bpos;
			//int bytesRead;
			
			const int tmode_normal = 0;
			const int tmode_iac = 1;
			const int tmode_option = 2;
			const int tmode_do = 3;
			const int tmode_will = 4;
			int mode;
			byte thebyte;
			
			cmdcnt = 0;
			bpos = 0;
			mode = tmode_normal;
			
			/*while (true)
			{
				bytesRead = 0;

				try
				{
					//blocks until a client sends a message
					bytesRead = clientStream.Read(message, 0, 4096);
				}
				catch
				{
					//a socket error has occured
					break;
				}

				if (bytesRead == 0)
				{
					//the client has disconnected from the server
					break;
				}*/
			//bytesRead = clientStream.Read(message, 0, 4096);
			bpos = 0;
			while ((cmdcnt < 4096) && (bpos < bytesRead)) {
				thebyte = message[bpos++];
				switch (mode) {
					case tmode_normal:
						switch (thebyte) {
							case Codes.IAC:
								mode = tmode_iac;
								Logger.Log("mode: " + mode);
								continue;
							case 0x0a: // LineFeed
								break;
							case 0x0d:
								string thecmd = Encoding.ASCII.GetString(cmd, 0, cmdcnt);
								if (thecmd.Length > 0) {
									write(thecmd);
									Logger.Log("thecmd: " + thecmd);
								}
								cmdcnt = 0;
								continue;
							default:
								break;
						}
						cmd[cmdcnt++] = thebyte;
						break;
					case tmode_iac:
						switch (thebyte) {
							case Codes.DO:
								mode = tmode_do;
								Logger.Log("mode: " + mode);
								break;
							case 240:  // End Subnegotiation
								mode = tmode_normal;
								Logger.Log("mode: " + mode);
								write(""); // Let the lower level know to force a screen refresh
								break;
							case 250:
								mode = tmode_option;
								Logger.Log("mode: " + mode);
								break;
							case Codes.WILL:
								mode = tmode_will;
								Logger.Log("mode: " + mode);
								break;
							default:
								mode = tmode_normal;
								Logger.Log("mode: " + mode);
								break;
						}
						break;
					case tmode_do:
						switch (thebyte) {
							default:
								mode = tmode_normal;
								Logger.Log("mode: " + mode);
								break;
						}
						break;
					case tmode_will:
						switch (thebyte) {
							default:
								mode = tmode_normal;
								Logger.Log("mode: " + mode);
								break;
						}
						break;
					case tmode_option:
						switch (thebyte) {
							default:
								mode = tmode_normal;
								Logger.Log("mode: " + mode);
								break;
						}
						break;
				}
			}
			
			//}

			//TelnetServer.tcpClient.GetStream().Close();
		}
		
		public int Negotiate2(byte[] Buffer, int Count)
		{
			int resplen = 0;
			int index = 0;

			while (index < Count) {
				if (Buffer[index] == Codes.IAC) {
					try {
						switch (Buffer[index + 1]) {
						/* If two IACs are together they represent one data byte 255 */
							case Codes.IAC:
								{
									Buffer[resplen++] = Buffer[index];
									index += 2;
									break;
								}
								
						/* Ignore the Go-Ahead command */
							case Codes.GA:
								{
									index += 2;
									break;
								}
								
						/* Respond WONT to all DOs and DONTs */
							case Codes.DO:
							case Codes.DONT:
								{
									Buffer[index + 1] = Codes.WONT;
									lock (TelnetServer.tcpClient.GetStream()) {
										TelnetServer.tcpClient.GetStream().Write(Buffer, index, 3);
									}
									index += 3;
									break;
								}

						/* Respond DONT to all WONTs */
							case Codes.WONT:
								{
									Buffer[index + 1] = Codes.DONT;
									lock (TelnetServer.tcpClient.GetStream()) {
										TelnetServer.tcpClient.GetStream().Write(Buffer, index, 3);
									}
									index += 3;
									break;
								}
								
						/* Respond DO to WILL ECHO and WILL SUPPRESS GO-AHEAD */
						/* Respond DONT to all other WILLs                    */
							case Codes.WILL:
								{
									byte action = Codes.DONT;
									if (Buffer[index + 2] == Codes.ECHO) {
										action = Codes.DO;
									} else if (Buffer[index + 2] == Codes.SUPP) {
										action = Codes.DO;
									}
									
									Buffer[index + 1] = action;
									lock (TelnetServer.tcpClient.GetStream()) {
										TelnetServer.tcpClient.GetStream().Write(Buffer, index, 3);
									}
									
									index += 3;
									break;
								}
						}
					} catch (IndexOutOfRangeException) {
						/* If there aren't enough bytes to form a command, terminate the loop */
						index = Count;
					}
				} else {
					if (Buffer[index] != 0) {
						Buffer[resplen++] = Buffer[index];
					}
					index++;
				}
			}
			return (resplen);
		}

		public static byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream()) {
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
		
		public void ReadBuffer()
		{
			// Loop to receive all the data sent by the client.
			Byte[] bytes = new Byte[256];
			//ScanBuffer(bytes);
			//String data = null;
			int i;
			while ((i = TelnetServer.tcpClient.GetStream().Read(bytes, 0, bytes.Length)) != 0) {
				Logger.Log(bytes.ToString());
				new Thread(() => ScanBuffer(bytes)).Start();
				
			}
		}
		
		public void Close()
		{
			var tid = Thread.CurrentThread.ManagedThreadId;
			Console.WriteLine("Clean up on Connection {0}", tid);
			
			//Remove all ThreadID Keys
			/*foreach (var key in TelnetServer.tcpStream) {
				Logger.Log("ThreadID: " + key);
			}
			foreach (var key in TelnetServer.Postion) {
				Logger.Log("Postion: " + key);
			}
			foreach (var key in TelnetServer.Screen) {
				Logger.Log("Screen: " + key);
			}
			try {
				NetworkStream retValue;
				if (TelnetServer.tcpStream.TryGetValue(tid, out retValue)) {
					//TelnetServer.tcpStream.Remove(tid);
				} else {
					throw new Exception(String.Format("Key {0} was not found", tid));
				}
				int[] pv;
				if (TelnetServer.Postion.TryGetValue(tid, out pv)) {
					//TelnetServer.Postion.Remove(tid);
				} else {
					throw new Exception(String.Format("Postion Key {0} was not found", tid));
				}
				int[] sv;
				if (TelnetServer.Screen.TryGetValue(tid, out sv)) {
					//TelnetServer.Screen.Remove(tid);
				} else {
					throw new Exception(String.Format("Screen Key {0} was not found", tid));
				}
				//if (TelnetServer.tcpStream.ContainsKey(tid))
				//	TelnetServer.tcpStream.Remove(tid);
				//if (Postion.ContainsKey(tid))
				//	Postion.Remove(tid);
				//if (Screen.ContainsKey(tid))
				//	Screen.Remove(tid);
			} catch (Exception ex) {
				Logger.Log(ex.Message);
			}*/
			
			//Close Thread
			TelnetServer.tcpStream[tid].Flush();
			TelnetServer.tcpStream[tid].Close();
			TelnetServer.Connected[tid] = false;
		}
		
		public string ReadLine()
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamReader streamReader = new StreamReader(sw);
			string text = string.Empty;
			try {
				text = streamReader.ReadLine();
			} catch (Exception) {
				Close();
				//Console.WriteLine("ReadLine: " + ex);
			}
			return text;
		}
		
		public byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
		
		public void ScanBuffer(byte[] buffer)
		{
			ClientBuf = buffer.ToList();
			//Logger.Log("Start scan with " + ClientBuf.Count() + " to go.");
			lock (ClientBuf) {
				bool IsESCCode = false;
				bool IsTelnetNeg = false;
				int ScreenX = 0;
				int ScreenY = 0;
				int b = 0;
				var tid = Thread.CurrentThread.ManagedThreadId;
				
				/*string ss = null;
				foreach (int prime in ClientBuf) { // Loop through List with foreach.
					ss = ss + prime + ":";
				}*/
				//Console.WriteLine(ss);
				
				int Len = ClientBuf.Count();
				//Logger.Log("ClientBuff count:" + Len);
				Stack<int> RemList = new Stack<int>(); // we use a stack so we don't disturb the ordering of the bytes we're removing

				while (b < Len) {
					//Logger.Log("ScanClientBuf:" + ClientBuf.Count);
					//Console.WriteLine("byte:" + ClientBuf[b]);
					byte cb = ClientBuf[b];

					if (cb == 27 && !IsESCCode) {
						var str = Encoding.UTF8.GetString(ClientBuf.ToArray());
						Match match = Regex.Match(str, @"\[([0-9]{1,2});([0-9]{1,2})",
							              RegexOptions.IgnoreCase);

						// Here we check the Match instance.
						if (match.Success) {
							// Finally, we get the Group value and display it.
							string y = match.Groups[1].Value;
							string x = match.Groups[2].Value;
	
							TelnetServer.Postion[tid] = new [] {
								Convert.ToInt32(y),
								Convert.ToInt32(x)
							};
							//Logger.Log("MATCH: " + y + " : " + x);
							
						}
					}

					if (cb == Codes.IAC && !IsTelnetNeg) {
						IsTelnetNeg = true;
						// let's advance through and see if we can get a telnet neg or subneg

						// IAC NOP is used as a keepalive; right now, we ignore it
						if (b < (Len - 1) && ClientBuf[b + 1] == Codes.NOP) {
							RemList.Push(b);
							b++;
							RemList.Push(b);
							IsTelnetNeg = false;
							// next, check to see if it's an IAC [DO|DONT|WILL|WONT] tuple
						} else if (b < (Len - 2) && ClientBuf[b + 1] >= 251 && ClientBuf[b + 1] <= 254) {
							// TODO: handle negotiations
							
							Logger.Log(GetNvtString(ClientBuf[b]) + "|" + GetNvtString(ClientBuf[b + 1]) + "|" + GetNvtString(ClientBuf[b + 2]));
							
							RemList.Push(b);
							b++;
							RemList.Push(b);
							b++;
							RemList.Push(b);
							IsTelnetNeg = false;
							// check for IAC SB ... IAC SE for subnegotiation parameters
						} else if (b < (Len - 1) && ClientBuf[b + 1] == Codes.SB) {
							// handle NAWS subneg
							if (ClientBuf[b] == Codes.IAC && ClientBuf[b + 1] == Codes.SB && ClientBuf[b + 2] == Codes.NAWS) {
								// TODO: This doesn't handle 255s properly (255s should be doubled in the bytestream to prevent confusion with IAC)
								if (BitConverter.IsLittleEndian) {
									ScreenX = Convert.ToInt32(BitConverter.ToInt16(new byte[] {
										ClientBuf[b + 4],
										ClientBuf[b + 3]
									}, 0));
									ScreenY = Convert.ToInt32(BitConverter.ToInt16(new byte[] {
										ClientBuf[b + 6],
										ClientBuf[b + 5]
									}, 0));
								} else {
									ScreenX = Convert.ToInt32(BitConverter.ToInt16(new byte[] {
										ClientBuf[b + 3],
										ClientBuf[b + 4]
									}, 0));
									ScreenY = Convert.ToInt32(BitConverter.ToInt16(new byte[] {
										ClientBuf[b + 5],
										ClientBuf[b + 6]
									}, 0));
								}
								Logger.Log("ID " + TelnetServer.tcpStream[tid].GetHashCode() + " screen size " + ScreenX + "x" + ScreenY);
								TelnetServer.Screen[tid] = new [] {
									Convert.ToInt32(ScreenY),
									Convert.ToInt32(ScreenX)
								};
							}
							int yy = b + 1;
							bool FoundSE = false;
							while (yy < Len && !FoundSE) {
								if (ClientBuf[yy] == Codes.IAC && yy < (Len - 1)) {
									if (ClientBuf[yy + 1] == Codes.SE) {
										FoundSE = true;
										IsTelnetNeg = false;
									}
								}
								yy++;
							}
							if (FoundSE) {
								String s = "";
								// we found a subneg! Right now, log it.
								for (int zz = b; zz <= yy; zz++) {
									s = s + GetNvtString(ClientBuf[zz]) + "|";
									//Logger.Log("Pushing " + zz + " with bufsz " + ClientBuf.Count()+ " " + GetNvtString(ClientBuf[zz])) ;
									RemList.Push(zz);
								}
								Logger.Log(s);
							} else {
								Logger.Log("Failed to find IAC SE in telnet subneg between " + b + " and " + yy);
							}
						}
					} 
					b++;
				}
				// remove inspected characters
				lock (ClientBuf) {
					foreach (int i in RemList) {
						
						//Logger.Log("Removing " + i + " of " + ClientBuf.Count());
						
						if (i < ClientBuf.Count()) {
							ClientBuf.RemoveAt(i);
						} else {
							Logger.Log(i + " not in range " + ClientBuf.Count());
						}
					}
				}
				RemList.Clear();
			}
		}
		
		public	byte[] ReadStream()
		{
			var sw = TelnetServer.tcpStream[Thread.CurrentThread.ManagedThreadId];
			StreamWriter streamWriter = new StreamWriter(sw);
			byte[] returnbuf = new byte[0];
			if (sw.CanRead) {
				// Buffer to store the response bytes.
				byte[] readBuffer = new byte[TelnetServer.tcpClient.ReceiveBufferSize];
				using (var writer = new MemoryStream()) {
					do {
						int numberOfBytesRead = sw.Read(readBuffer, 0, readBuffer.Length);
						if (numberOfBytesRead <= 0) {
							break;
						}
						writer.Write(readBuffer, 0, numberOfBytesRead);
					} while (sw.DataAvailable);

					returnbuf = writer.ToArray();
				}
			}
			return returnbuf;
		}
		
		public void ClearBuffer()
		{
			lock (ClientBuf) {
				ClientBuf.Clear();
			}
		}
		
		public void ParseTelnet(StringBuilder sb)
		{
			while (TelnetServer.tcpClient.Available > 0) {
				int input = TelnetServer.tcpClient.GetStream().ReadByte();
				Logger.Log(input.ToString());

				switch (input) {
					case -1:
						break;
					case (int) Codes.IAC:
						// interpret as command
						int inputverb = TelnetServer.tcpClient.GetStream().ReadByte();
						if (inputverb == -1)
							break;
						switch (inputverb) {
							case (int) Codes.IAC:
								//literal Iac = 255 escaped, so append char 255 to string
								sb.Append(inputverb);
								break;
							case (int) Codes.DO:
							case (int) Codes.DONT:
							case (int) Codes.WILL:
							case (int) Codes.WONT:
								// reply to all commands with "Wont", unless it is Sga (suppres go ahead)
								int inputoption = TelnetServer.tcpClient.GetStream().ReadByte();
								if (inputoption == -1)
									break;
								TelnetServer.tcpClient.GetStream().WriteByte((byte)Codes.IAC);
								if (inputoption == (int)Codes.SUPP)
									TelnetServer.tcpClient.GetStream().WriteByte(inputverb == (int)Codes.DO
									                                             ? (byte)Codes.WILL
									                                             : (byte)Codes.DO);
								else
									TelnetServer.tcpClient.GetStream().WriteByte(inputverb == (int)Codes.DO
									                                             ? (byte)Codes.WONT
									                                             : (byte)Codes.DONT);
								TelnetServer.tcpClient.GetStream().WriteByte((byte)inputoption);
								break;
							default:
								break;
						}
						break;
					default:
						sb.Append((char)input);
						break;
				}
			}
		}
		
		public void print_inputbox(int length, ConsoleColor fcolor, ConsoleColor bcolor)
		{
			//colors.setColor(fcolor, bcolor);

			int c = 0;

			while (c < length) {
				c++;
				TelnetServer.tcpClient.GetStream().Write(new byte[] { 0x20 }, 0, 1);
			}

			while (length > 0) {
				length--;
				TelnetServer.tcpClient.GetStream().Write(new byte[] { (byte)'\b' }, 0, 1);
			}

		}

		//--Clear screen
		public void clearscreen() //Clear Full Screen
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'2',
				(byte)'J'
			});
			set_cursor_loc(0, 0);
			TelnetServer.Postion[Thread.CurrentThread.ManagedThreadId] = new []{ 1, 3 };
		}
		
		//--Clear screen
		public void Clearscreen() //Clear Full Screen
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'2',
				(byte)'J'
			});
			set_cursor_loc(0, 0);
			TelnetServer.Postion[Thread.CurrentThread.ManagedThreadId] = new []{ 1, 3 };
		}
		/// <summary>
		/// Clear line from cursor right
		/// </summary>
		public void clear_right_cursor()//Remove from the line where the cursor to the beginning of the cursor
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'0',
				(byte)'K'
			});
		}

		public void clear_cursor2LineEnd()//Clear from the cursor position to the end of the line
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'K'
			});
		}

		//Setting the cursor position
		/// <summary>
		/// Sets cursor position
		/// </summary>
		/// <param name="h"></param> Horizontal
		/// <param name="v"></param> Vertical
		public void set_cursor_loc(int h, int v)
		{
			List<byte> buf_loc = Encoding.Default.GetBytes("[" + h + ";" + v + "H").ToList();
			buf_loc.Insert(0, 0x1b);
			writebytes(buf_loc.ToArray());
		}
		
		/// <summary>
		/// Orginal Location
		/// </summary>
		public void set_cursor_home()
		{
			writebytes(new byte[] {
				0x1b,
				(byte)'[',
				(byte)'H'
			});
		}
		#endregion
	}
}
