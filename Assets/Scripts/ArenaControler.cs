using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum ArenaStage { STARTING, FIRST_STAGE, SECOND_STAGE, TIRD_STAGE, GTFO}

public class ArenaControler : MonoBehaviour
{
	public static ArenaControler instance;

	[Header("Stages")]
	private ArenaStage currentStage = ArenaStage.STARTING;
	public int startingDuration;
	public int firstStageDuration;
	public int secondStageDuration;
	public int tirdStageDuration;

	[Header("UI")]
	public Image blackScreen;
	public TextMeshProUGUI text;
	public TextMeshProUGUI clockText;
	public TextMeshProUGUI stageText;
	private Coroutine clockRoutine;

	private float clock = 0;

	public GameObject nextStageButton;
	public GameObject returnButton;

	bool endScreenCompleted = false;

	public float Clock
	{
		get
		{
			return clock;
		}

		private set
		{
			clock = value;
			clockText.text = clock.ToString("0.00");

			if(clock > tirdStageDuration)
			{
				CurrentStage = ArenaStage.GTFO;				
			}
			else if(clock > secondStageDuration)
			{
				CurrentStage = ArenaStage.TIRD_STAGE;
			}
			else if(clock > firstStageDuration)
			{
				CurrentStage = ArenaStage.SECOND_STAGE;
			}
			else if(clock > startingDuration)
			{
				CurrentStage = ArenaStage.FIRST_STAGE;
			}
		}
	}

	public ArenaStage CurrentStage
	{
		get
		{
			return currentStage;
		}

		set
		{
			currentStage = value;
			stageText.text = currentStage.ToString();
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			if (instance != this)
			{
				Destroy(this);
			}
		}
		else
		{
			instance = this;
		}

		blackScreen.color = Color.clear;
		text.color = new Color(1,1,1,0);
		clockRoutine = StartCoroutine(TimePassing());
	}

	public void EndArena()
	{
		HypeMetter hypeMetter = HypeMetter.instance;
		SpawnerControler spawner = SpawnerControler.instance;

		StopAllEnemies();
		StopCoroutine(clockRoutine);
		spawner.enabled = false;
		hypeMetter.dropRate = 0;
		hypeMetter.baseDropRate = 0;
				
	
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

	IEnumerator TimePassing()
	{
		while (true)
		{
			Clock += Time.deltaTime;
		yield return null;
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

		returnButton.SetActive(true);

		if(result != 0 && nextStageButton != null)
		{
			nextStageButton.SetActive(true);
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

	public static void StopAllEnemies()
	{
		foreach (EnemyBrain eB in FindObjectsOfType<EnemyBrain>())
		{
			eB.State = EnemyStates.STUNNED;
		}
	}

	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

}
