using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
	public static GameCanvas Instance;

	void Awake()
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

	}

	public void EnableCanvas()
	{
		gameObject.SetActive(false);
	}
	public void DisableCanvas()
	{
		gameObject.SetActive(false);
	}
}
