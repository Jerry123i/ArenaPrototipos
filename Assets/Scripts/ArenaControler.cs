using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ArenaControler : MonoBehaviour
{
	public Image blackScreen;
	public TextMeshProUGUI text;

	bool endScreenCompleted = false;

	private void Awake()
	{
		blackScreen.color = Color.clear;
		text.color = new Color(1,1,1,0);
	}

	public void EndArena()
	{
		HypeMetter hypeMetter = HypeMetter.instance;
		SpawnerControler spawner = SpawnerControler.instance;

		StopAllEnemies();
		spawner.enabled = false;
		hypeMetter.dropRate = 0;
		hypeMetter.baseDropRate = 0;
		
		//hypeMetter.enabled = false;
	
		if(hypeMetter.CurrentHype >= hypeMetter.tier2Value)
		{
			StartCoroutine(EndScreen(2));
		}
		else if (hypeMetter.CurrentHype >= hypeMetter.tier1Value)
		{
			StartCoroutine(EndScreen(1));
		}
		else
		{
			StartCoroutine(EndScreen(0));
		}

	}

	IEnumerator EndScreen(int result)
	{
		while(blackScreen.color.a < 0.75f)
		{
			blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (0.3f * Time.deltaTime));
			yield return null;
		}

		switch (result)
		{
			case 0:
				text.text = "Parece que a platéia achou uma bosta";
				break;
			case 1:
				text.text = "Até que foi Ok ein";
				break;
			case 2:
				text.text = "BRABO DE MAIS!";
				break;
		}

		while(text.color.a < 1)
		{
			blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (0.3f * Time.deltaTime));
			text.color = new Color(1, 1, 1, text.color.a + (0.5f * Time.deltaTime));
			yield return null;
		}

		endScreenCompleted = true;

	}

	private void Update()
	{
		if(Input.GetKeyDown("escape") && endScreenCompleted)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	public void StopAllEnemies()
	{
		foreach (EnemyBrain eB in FindObjectsOfType<EnemyBrain>())
		{
			eB.State = EnemyStates.STUNNED;
		}
	}

}
