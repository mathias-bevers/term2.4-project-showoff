using System;
using System.Collections.Generic;
using System.Text;
using saxion_provided;

public class DataBaseCloud
{
	private const int MAX_SIZE = 10;
	private readonly Queue<string> fileNames;

	private readonly Server parentServer;

	public DataBaseCloud(Server parentServer)
	{
		this.parentServer = parentServer;
		fileNames = new Queue<string>(MAX_SIZE);
	}

	public void ProcessPacket(ServerClient sender, DataBaseObject serverObject)
	{
		if (serverObject is AddFileName toAdd)
		{
			if (fileNames.Count == MAX_SIZE) { _ = fileNames.Dequeue(); }
				
			fileNames.Enqueue(toAdd.fileName);
		}

		Packet packet = new();
		GetFileNames getFileNames = new(fileNames);
		packet.Write(getFileNames);
		parentServer.WriteToClient(sender, packet);
	}

	public override string ToString()
	{
		StringBuilder sb = new($"There are {fileNames.Count} file-name entries in the cloud:" + Environment.NewLine);

		foreach (string fileName in fileNames)
		{
			sb.Append('\t');
			sb.Append(fileName);
			sb.Append(Environment.NewLine);
		}

		return sb.ToString();
	}
}