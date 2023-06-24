using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
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

			Transform letterParent = inputGroup.GetChild(0);
			Button upButton = inputGroup.GetChild(1).GetComponent<Button>();
			Button downButton = inputGroup.GetChild(2).GetComponent<Button>();

			letterInputs[i] = new LetterInput(upButton, downButton, letterParent);
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

		foreach (LetterInput letterInput in letterInputs) { sb.Append(letterInput.displayingChar); }

		return sb.ToString();
	}

	private class LetterInput
	{
		public Button upButton { get; }
		public Button downButton { get; }
		public char displayingChar { get; private set; }

		private int letterIndex { get; set; }
		private readonly Transform letterParent;

		public LetterInput(Button upButton, Button downButton, Transform letterParent)
		{
			this.upButton = upButton;
			this.downButton = downButton;
			this.letterParent = letterParent;
			letterIndex = 0;

			displayingChar = 'A';
			letterParent.SetChildrenText(displayingChar.ToString());
		}

		public void CycleLetter(int amount)
		{
			letterIndex += amount;
			letterIndex %= ALPHABET_COUNT;
			if (letterIndex < 0) { letterIndex = ALPHABET_COUNT - 1; }

			int indexToChar = letterIndex + CHAR_ALPHABET_START;
			displayingChar = Convert.ToChar(indexToChar);
			letterParent.SetChildrenText(displayingChar.ToString());
		}
	}
}