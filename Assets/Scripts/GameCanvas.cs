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
			Destroy(this.gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}

	}

	public void EnableCanvas()
	{
		this.gameObject.SetActive(false);
	}
	public void DisableCanvas()
	{
		this.gameObject.SetActive(false);
	}
}
