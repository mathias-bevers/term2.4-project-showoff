using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saxion_provided;
using UnityEngine;

public class DataBaseCloud
{
	private readonly Server parentServer;

	private Queue<string> fileNames;

	public DataBaseCloud(Server parentServer)
	{
		this.parentServer = parentServer;
		fileNames = new Queue<string>(Settings.MAX_MEMES);
	}

	public void ProcessPacket(ServerClient sender, DataBaseObject serverObject)
	{
		switch (serverObject)
		{
			case AddFileName toAdd:
				Add(toAdd.fileName);
				break;

			case AddFileNames bulk:
				foreach (string fileName in bulk.fileNames) { Add(fileName); }

				break;
		}

		Packet packet = new();
		GetFileNames getFileNames = new(fileNames);
		packet.Write(getFileNames);
		parentServer.WriteToClient(sender, packet);
		Debug.Log($"Writing file-names to client#{sender.id}\n{ToString()}");
	}

	private void Add(string fileName)
	{
		if (fileNames.Contains(fileName)) { fileNames = new Queue<string>(fileNames.Where(s => s != fileName)); }

		if (fileNames.Count == Settings.MAX_MEMES) { fileNames.Dequeue(); }

		fileNames.Enqueue(fileName);
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