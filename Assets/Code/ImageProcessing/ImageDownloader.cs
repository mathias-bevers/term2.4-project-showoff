using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class ImageDownloader : MonoBehaviour
{
	private string staticImagePath;

	private void Awake()
	{
		staticImagePath = Application.persistentDataPath + "/player_uploads/";
		if (Directory.Exists(staticImagePath)) { return; }

		Directory.CreateDirectory(staticImagePath);
	}

	/// <summary>
	///     Tries to request an an image from the given URL. When response is valid the image is saved.
	///     The path of the image is returned.
	/// </summary>
	/// <param name="url">The url of the internet image</param>
	/// <returns>The path where the image is saved</returns>
	/// <exception cref="NoNullAllowedException">thrown when url is not set</exception>
	public string Download(string url)
	{
		if (string.IsNullOrEmpty(url)) { throw new NoNullAllowedException("url cannot be empty!"); }

		HttpWebResponse response = GetResponse(url);
		string savedImage = SaveImage(response);
		return savedImage;
	}

	private HttpWebResponse GetResponse(string url)
	{
		HttpWebRequest request = WebRequest.CreateHttp(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		if ((response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved && response.StatusCode != HttpStatusCode.Redirect) ||
		    !response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) { Debug.LogError("could not find an image from URL"); }

		return response;
	}

	private string SaveImage(HttpWebResponse response)
	{
		using Stream rs = response.GetResponseStream();

		if (rs == null) { throw new NoNullAllowedException("Could not get response stream"); }

		string fileName = FormatDateString();
		string extension = response.ContentType.Split('/').Last();
		string filePath = string.Concat(staticImagePath, fileName, ".", extension);

		using FileStream fs = File.OpenWrite(filePath);

		byte[] bytes = new byte[response.ContentLength];
		int read = 0;
		int passes = 0;

		do
		{
			read = rs.Read(bytes, 0, bytes.Length);
			fs.Write(bytes, 0, read);

			passes++;
			if (passes >= 1000) { throw new TimeoutException("Too many passes"); }
		} while (read != 0);

		rs.Close();
		fs.Close();


		Debug.Log($"Downloaded image to {filePath}");
		return filePath;
	}

	private string FormatDateString()
	{
		DateTime now = DateTime.Now;
		CultureInfo ci = new("nl");

		string[] splitTime = now.ToString(ci).Split(' ');
		splitTime[1] = splitTime[1].Replace(':', '-');
		Array.Reverse(splitTime);

		return now.ToString(string.Join('_', splitTime));
	}
}