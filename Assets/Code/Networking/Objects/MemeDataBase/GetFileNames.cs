using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saxion_provided;

public class GetFileNames : DataBaseObject
{
	public GetFileNames() { fileNames = null; }
	public GetFileNames(IEnumerable<string> fileNames) { this.fileNames = fileNames.ToArray(); }

	public string[] fileNames { get; private set; }

	public override void Serialize(Packet packet)
	{
		packet.Write(fileNames.Length);

		foreach (string fileName in fileNames) { packet.Write(fileName); }
	}

	public override void Deserialize(Packet packet)
	{
		int count = packet.ReadInt();
		fileNames = new string[count];

		for (int i = 0; i < count; ++i)
		{
			string fileName = packet.ReadString();
			fileNames[i] = fileName;
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new("GetFileNames containing:" + Environment.NewLine);

		foreach (string fileName in fileNames)
		{
			sb.Append("\t");
			sb.AppendLine(fileName);
		}

		return sb.ToString();
	}
}