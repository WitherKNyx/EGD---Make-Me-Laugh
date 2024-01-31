using UnityEngine;
using UnityEngine.UI;

public class LoveMeter : MonoBehaviour
{
	private const int MAX_LOVE = 5;
	private const int MAX_MISTAKES = 3;

	public static LoveMeter Instance;

	#region Properties
	public int LoveValue { get { return _loveValue; } }

	public int Mistakes { get { return _mistakes; } }

	#endregion

	#region References
	public Sprite[] _sprites;

	[SerializeField]
	private Image _Image;
	#endregion

	#region Private Variables
	private int _loveValue;
	private int _mistakes;
	#endregion

	#region System Functions
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		ResetValues();
	}

	private void Update()
	{
		_Image.sprite = _sprites[_loveValue];
	}
	#endregion

	public void ResetValues() { ResetLove(); ResetMistakes(); }

	public void ResetLove() => _loveValue = 0;

	public void ResetMistakes() => _mistakes = 0;

	public void AddLove(int loveValue) => _loveValue = Mathf.Clamp(loveValue + _loveValue, 0, MAX_LOVE);

	public void AddMistake() => _mistakes = Mathf.Min(_mistakes + 1, MAX_MISTAKES);
}
