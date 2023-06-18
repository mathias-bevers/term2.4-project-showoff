using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
	private const int ALPHABET_COUNT = 26;
	private const int CHAR_ALPHABET_START = 65;

	[SerializeField] private Transform inputGroups;
	
	private EventSystem eventSystem;
	private LetterInput[] letterInputs;
	private Transform cachedTransform;

	private void Awake()
	{
		cachedTransform = transform;

		letterInputs = new LetterInput[inputGroups.childCount];
		for (int i = 0; i < letterInputs.Length; ++i)
		{
			Transform inputGroup = inputGroups.GetChild(i);

			Text letterText = inputGroup.GetChild(0).GetComponent<Text>();
			Button upButton = inputGroup.GetChild(1).GetComponent<Button>();
			Button downButton = inputGroup.GetChild(2).GetComponent<Button>();

			letterInputs[i] = new LetterInput(upButton, downButton, letterText);
			letterInputs[i].CycleLetter(0);
		}
	}

	private void OnEnable()
	{
		eventSystem = EventSystem.current;
		eventSystem.SetSelectedGameObject(letterInputs[0].upButton.gameObject);

		foreach (LetterInput letterInput in letterInputs)
		{
			letterInput.upButton.onClick.AddListener(() => letterInput.CycleLetter(1));
			letterInput.downButton.onClick.AddListener(() => letterInput.CycleLetter(-1));
		}
	}

	public string GetName()
	{
		StringBuilder sb = new();

		foreach (LetterInput t in letterInputs) { sb.Append(t.letterText.text); }

		return sb.ToString();
	}
	
	private struct LetterInput
	{
		public Button upButton { get; }
		public Button downButton { get; }
		public Text letterText { get; }
		
		private int letterIndex { get; set; }


		public LetterInput(Button upButton, Button downButton, Text letterText)
		{
			this.upButton = upButton;
			this.downButton = downButton;
			this.letterText = letterText;
			letterIndex = 0;

			letterText.text = "A";
		}
		
		public void CycleLetter(int amount)
		{
			letterIndex += amount;
			letterIndex %= ALPHABET_COUNT;
			if (letterIndex < 0) { letterIndex = ALPHABET_COUNT - 1; }

			int indexToChar = letterIndex + CHAR_ALPHABET_START;
			letterText.text = Convert.ToChar(indexToChar).ToString();
		}

	}
}