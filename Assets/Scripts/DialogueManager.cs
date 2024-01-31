using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
	#region Static Variables
	[HideInInspector]
    public static DialogueManager instance;

	/// <summary>
	/// This variable is used to block interactions while dialogue is occuring
	/// </summary>
	[HideInInspector]
	public static bool inDialogue = false;
	#endregion

	#region References
	[SerializeField]
	private GameObject _dialogueBox, _dialogueChoices;

	[SerializeField]
	private Image _dialoguePortrait, _continueIndicator;

	[SerializeField]
	private TextMeshProUGUI _nameTextBox, _dialogueTextBox;

	[SerializeField]
	private TextMeshProUGUI[] _dialogueChoiceBoxes;
	#endregion

	[SerializeField, Min(0f), Tooltip("Time interval between each character appearing in the textbox.")]
	private float _timeInterval;

	[SerializeField]
	Dialogue test;

	private Coroutine _currentLine;

	private string _text;
	private int _choice = -1;

    #region System Functions
    private void Awake()
	{
		if (instance == null)
		{
			instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneChange;
		}
		else
			Destroy(gameObject);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && _currentLine != null)
		{
			StopCoroutine(_currentLine);
			_currentLine = null;
		}
	}
	#endregion

	public void SetChoice(int i) => _choice = i;

	private void OnSceneChange(Scene scene, LoadSceneMode mode)
	{
		string currentScene = scene.name;
		Dialogue openingDialogue = null;

		
		if (openingDialogue != null)
		{
			StartCoroutine(PlayDialogue(openingDialogue));
		}
	}

	/// <summary>
	/// Function to play a give Dialogue object
	/// </summary>
	/// <param name="dialogue"></param>
	public IEnumerator PlayDialogue(Dialogue dialogue)
	{
		inDialogue = true;
		_dialogueBox.SetActive(true);

		// Split dialogue into lines for parsing
		string[] lines = dialogue.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < lines.Length; i++)
		{
			// Tag beginning a line indicates a sprite/speaker change
			if (lines[i][0] == '<')
			{
				int start, end;

				start = lines[i].IndexOf("<end>");
				if (start != -1)
				{
					break;
				}

				// Speaker tag indicates a different character
				start = lines[i].IndexOf("speaker=\"");
				if (start != -1)
				{
					start += 10;
					end = lines[i].IndexOf('"', start);
					_nameTextBox.text = lines[i][start..end];
				}

				// Sprite tag indicates a different sprite
				start = lines[i].IndexOf("sprite=\"");
				if (start != -1)
				{
					start += 9;
					end = lines[i].IndexOf('"', start);
					_nameTextBox.text = lines[i][start..end];
				}

				start = lines[i].IndexOf("<choice>");
				if (start != -1)
				{
					++i;
					// Obtain choices and jump Labels
					List<string> labels = new();
					List<string> choices = new();
					while (lines[i] != "</choice>")
					{
						start = lines[i].IndexOf("<");
						end = lines[i].IndexOf(">");
						labels.Add(lines[i][start..end]);
						choices.Add(lines[i][(end + 1)..]);
						++i;
					}

					for (int j = 0; j < Mathf.Min(choices.Count, _dialogueChoiceBoxes.Length); j++)
					{
						_dialogueChoiceBoxes[j].text = choices[j];
					}
					_dialogueChoices.SetActive(true);
					yield return new WaitUntil(() => _choice != -1);
					_dialogueChoices.SetActive(false);
					for (int j = 0; j < lines.Length; j++)
					{
						start = lines[i].IndexOf("<label=\"" + labels[_choice] + "\">");
						if (start != -1)
						{
							i = j;
							_choice = -1;
							break;
						}
					}
				}
				// Skip to next line for actual dialogue
				++i;
			}

			// Wait until the current line is processed before moving on
			_currentLine = StartCoroutine(ProcessLine(lines[i]));
			yield return new WaitWhile(() => _currentLine != null);
			_continueIndicator.enabled = true;
			// This yield is here to prevent accidentally reading the same mouse
			// press to skip dialogue as the mouse press to advance to the next dialogue
			yield return null;

			// Ensure the dialogue box contains all text, then wait for user input
			_dialogueTextBox.text = lines[i];
			while (true)
			{
				if (Input.GetMouseButtonDown(0))
					break;
				yield return null;
			}
			_continueIndicator.enabled = false;
		}

		_dialogueBox.SetActive(false);
		inDialogue = false;
	}

	public IEnumerator ProcessLine(string line)
	{
		// change to use _text and update to _dialogueTextbox.text in update
		yield return new WaitForSeconds(_timeInterval);
		_dialogueTextBox.text = line[0].ToString();
		for (int j = 1; j < line.Length; j++)
		{
			// If the next word would be too long for the textbox, add a newline
			if (line[j - 1] == ' ')
			{
				int nextSpace = line.IndexOf(' ', j);
                nextSpace = nextSpace == -1 ? line.Length : nextSpace;
				string nextWord = line[j..nextSpace];
				string currText = _dialogueTextBox.text;
				_dialogueTextBox.text += nextWord;
				float modifiedTextWidth = _dialogueTextBox.preferredWidth;
				float textBoxWidth = _dialogueTextBox.rectTransform.rect.width;
				if (modifiedTextWidth > textBoxWidth) currText += '\n';
				_dialogueTextBox.text = currText;
			}

			// Assign extra pauses to special characters
			int multiplier = 1;
			if (char.IsWhiteSpace(line[j - 1]))
				multiplier = 2;
			else if (char.IsPunctuation(line[j - 1]) && line[j -1 ] != '\'' && line[j - 1] != '"')
				multiplier = 4;

			yield return new WaitForSeconds(multiplier * _timeInterval);
			char letter = line[j];
			if (line[j] == '\\')
			{
				if (line[j + 1] == '<') letter = '<';
				else if (line[j + 1] == 'n') letter = '\n';
				else if (line[j + 1] == 't') letter = '\t';
			}
			_dialogueTextBox.text += letter;
		}
		_currentLine = null;
	}
}
