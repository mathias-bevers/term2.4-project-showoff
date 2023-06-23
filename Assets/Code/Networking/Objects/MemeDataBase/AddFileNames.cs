using System;
using System.Collections.Generic;
using System.Linq;
using saxion_provided;

public class AddFileNames : DataBaseObject
{
	public string[] fileNames { get; private set; }

	public AddFileNames() { fileNames = null; }

	public AddFileNames(IList<string> fileNames)
	{
		if (fileNames.IsNull()) { throw new ArgumentNullException(nameof(fileNames), $"Cannot add null to a {nameof(ServerObject)}"); }

		if (fileNames.Count > Settings.MAX_MEMES) { throw new ArgumentException("can only have 10 fileNames at the time!", nameof(fileNames)); }

		this.fileNames = fileNames.ToArray();
	}

	public override void Serialize(Packet packet)
	{
		int count = fileNames.Length;
		packet.Write(count);

		for (int i = 0; i < count; ++i) { packet.Write(fileNames[i]); }
	}

	public override void Deserialize(Packet packet)
	{
		int count = packet.ReadInt();
		fileNames = new string[count];

		for (int i = 0; i < count; ++i) { fileNames[i] = packet.ReadString(); }
	}
}