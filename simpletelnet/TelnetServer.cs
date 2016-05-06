using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Command.Shell;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Telnet.UI
{
	class TelnetServer
	{
		private ArrayList client_list;
		//private ConcurrentDictionary<TcpClient, string> clienttcp;
		private TcpListener listener;
		private Thread processor;
		public static TcpClient tcpClient;
		public Tools tools = new Tools();
		
		public static ConcurrentDictionary<int, NetworkStream> tcpStream;
		/// <summary>
		/// Cursor Postion Y,X
		/// </summary>
		public static ConcurrentDictionary<int, int[]> Postion;
		
		/// <summary>
		/// Screen Size Y,X
		/// </summary>
		public static ConcurrentDictionary<int, int[]> Screen;
		
		/// <summary>
		/// Lists Clients what clients that are connected.
		/// </summary>
		public static ConcurrentDictionary<int, bool> Connected;
		public int port;
		public IPAddress ipa;

		public TelnetServer(IPAddress ipadress, int portnum)
		{
			client_list = new ArrayList();
			//clienttcp = new ConcurrentDictionary<TcpClient, string>();
			
			tcpStream = new ConcurrentDictionary<int, NetworkStream>();
			Postion = new ConcurrentDictionary<int, int[]>();
			Screen = new ConcurrentDictionary<int, int[]>();
			Connected = new ConcurrentDictionary<int, bool>();
			
			ipa = ipadress;
			port = portnum;
			//this.password = password;

			processor = new Thread(new ThreadStart(start_listening));
			processor.Start();
		}

		// disable once FunctionNeverReturns
		private void start_listening()
		{
			listener = new TcpListener(ipa, port);
			listener.Start();
			Logger.Log(string.Format("Waiting for a connection on {0}, port {1} ... ", ipa, port));
			while (true) {
				try {
					//Logger.Log("Start Accept-" + Thread.CurrentThread.ManagedThreadId);
					tcpClient = listener.AcceptTcpClient();
					Thread client_thread = null;
					Client_Service client = new Client_Service(this, tcpClient);
					client_thread = new Thread(new ThreadStart(client.Connect));
					client_thread.Start();
					//Logger.Log("SL-ID: " + client_thread.ManagedThreadId);
				} catch (Exception ex) {
					Logger.Log("Listening Error: " + ex.Message);
				}

			}
		}
		public static IPAddress LocalIPAddress()
		{
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
				return null;
			}

			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			return host
        .AddressList
        .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
		}
		
		class Client_Service
		{
			TelnetServer service;

			public TelnetServer Service {
				get { return service; }
				set { service = value; }
			}
			
			TcpClient tcp_client;
			public Client_Service(TelnetServer service, TcpClient tcp_client)
			{
				this.Tcp_client = tcp_client;
				this.Service = service;
			}

			public TcpClient Tcp_client {
				get { return tcp_client; }
				set { tcp_client = value; }
			}
			
			byte[] GetBytes(string str)
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
						
			byte[] ReadStream()
			{
				byte[] returnbuf = new byte[0];
				if (tcpClient.GetStream().CanRead) {
					// Buffer to store the response bytes.
					byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];
					using (var writer = new MemoryStream()) {
						do {
							int numberOfBytesRead = tcpClient.GetStream().Read(readBuffer, 0, readBuffer.Length);
							if (numberOfBytesRead <= 0) {
								break;
							}
							writer.Write(readBuffer, 0, numberOfBytesRead);
						} while (tcpClient.GetStream().DataAvailable);
						returnbuf = writer.ToArray();
					}
				}
				return returnbuf;
			}
			public static bool IsASCII(string value)
			{
				// ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
				return Encoding.UTF8.GetByteCount(value) == value.Length;
			}
			
			public void Connect()
			{
				var tid = Thread.CurrentThread.ManagedThreadId;
				//var hash = tcpClient.GetStream().GetHashCode();
				tcpClient = Tcp_client;
				tcpStream.TryAdd(tid, tcpClient.GetStream());
				Postion.TryAdd(tid, new[]{ 0, 0 });
				Screen.TryAdd(tid, new[]{ 0, 0 });
				Connected.TryAdd(tid, true);
			
				var input = new InputCommand();
				var display = new Display();
				TelnetShell.StartCmd(input, display);	
			}
		}
	}
}
