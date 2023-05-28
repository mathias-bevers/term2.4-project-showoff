using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using saxion_provided;
using UnityEngine;

public class Server
{
	private List<TcpClient> clients;
	private TcpListener listener;


	private bool shouldClose;

	public Server(IPAddress ip, int port)
	{
		listener = new TcpListener(ip, port);
		clients = new List<TcpClient>(2);
		listener.Start();

		Run();
	}

	private void Run()
	{
		while (true)
		{
			if (shouldClose) { break; }

			//TODO: process clients etc....
			ProcessNewClients();

			Thread.Sleep(100);
		}
	}

	private void ProcessNewClients()
	{
		while (listener.Pending())
		{
			if (clients.Count < 2)
			{
				TcpClient accepted = listener.AcceptTcpClient();
				clients.Add(accepted);
				Debug.Log("Accepted client");
				continue;
			}

			TcpClient rejected = listener.AcceptTcpClient();
			Packet packet = new();
			packet.Write("REJECTED");
			StreamUtil.Write(rejected.GetStream(), packet.GetBytes());
			rejected.Close();
		}
	}
}