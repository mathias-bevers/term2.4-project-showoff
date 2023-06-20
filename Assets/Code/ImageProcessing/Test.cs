using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	[SerializeField] private bool useGoogleImages;
	
	[SerializeField] private Button confirmButton;
	[SerializeField] private InputField urlInputField;
	[SerializeField] private RawImage image;

	private ImageDownloader imageDownloader;
	private ImageSearcher imageSearcher;

	private void Start()
	{
		if (useGoogleImages)
		{
			imageSearcher = GetComponent<ImageSearcher>();
			confirmButton.onClick.AddListener(OnSearchClick);
			return;
		}

		imageDownloader = GetComponent<ImageDownloader>();
		confirmButton.onClick.AddListener(OnDownloadClick);

		TextEditor te = new();
		te.Paste();
		urlInputField.text = te.text;
	}

	private void OnDownloadClick()
	{
		string url = urlInputField.text;
		url = url.Trim();

		urlInputField.text = string.Empty;

		string savedImage = imageDownloader.Download(url);

		if (string.IsNullOrEmpty(savedImage)) { return; }

		byte[] rawData = File.ReadAllBytes(savedImage);
		Texture2D texture2D = new(1, 1);
		texture2D.LoadImage(rawData);

		image.texture = texture2D;
	}

	private void OnSearchClick() { }
}