using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTemplate { NONE, HEAVY, FAST, DOUBLE};

public class SpawnerControler : MonoBehaviour {

	public static SpawnerControler instance;
	
	public List<SpawnPointScript> spawnPoints;

	[Header("Waves")]
	public List<WaveConfiguration> possibleWaves1;
	public List<WaveConfiguration> possibleWaves2;
	public List<WaveConfiguration> possibleWaves3;
	public List<WaveConfiguration> possibleWavesGtfo;

	private void Awake()
	{
		if(instance != null)
		{
			if(instance != this)
			{
				Destroy(this);
			}
		}
		else
		{
			instance = this;
		}

	}

	private void Start()
	{
		SpawnWave();
	}

	private void Update()
	{

	}

	public void EnemyDied()
	{
	}


	//Isso provavelmente deveria ser feito com listners
	public void AnouceHit()
	{
	}

	public void SpawnWave()
	{
		WaveConfiguration wave;
		int special = RollSpecialEnemy();
		int a = 99;
		int b = 99;

		switch (ArenaControler.instance.CurrentStage)
		{
			case ArenaStage.STARTING:
				wave = possibleWaves1[Random.Range(0, possibleWaves1.Count)];
				break;
			case ArenaStage.FIRST_STAGE:
				wave = possibleWaves1[Random.Range(0, possibleWaves1.Count)];
				break;
			case ArenaStage.SECOND_STAGE:
				wave = possibleWaves2[Random.Range(0, possibleWaves2.Count)];
				break;
			case ArenaStage.TIRD_STAGE:
				wave = possibleWaves3[Random.Range(0, possibleWaves3.Count)];
				break;
			case ArenaStage.GTFO:
				wave = possibleWavesGtfo[Random.Range(0, possibleWavesGtfo.Count)];
				break;
			default:
				wave = possibleWaves1[Random.Range(0, possibleWaves1.Count)];
				break;
		}

		

		if(special > 0)
		{
			a = Random.Range(0, wave.enemies.Count);
			b = a;

			if(special > 1)
			{
				do
				{
					b = Random.Range(0, wave.enemies.Count);
				} while (a == b);
			}
		}

		//Debug.Log("Wave: " + wave.name);

		for(int i = 0; i< wave.enemies.Count; i++)
		{
			GameObject go = wave.enemies[i];

			if (special == 0)
			{
				GetRandomSpawnPoint().RecieveSpawnOrder(go, EnemyTemplate.NONE);
			}
			else
			{
				if(i == a || i == b)
				{
					GetRandomSpawnPoint().RecieveSpawnOrder(go, GetRandomEnemyTemplate());
				}
				else
				{
					GetRandomSpawnPoint().RecieveSpawnOrder(go, EnemyTemplate.NONE);
				}
			}
		}

	}

	int RollSpecialEnemy()
	{
		//25 % de chance de 1 especial
		//10 % de chance de 2
		//Usar meta-randons mais inteligentes que isso no jogo final

		int n = Random.Range(1, 101);

		if (n < 11)
		{
			return 2;
		}
		else if (n < 36)
		{
			return 1;
		}

		return 0;

	}

	EnemyTemplate GetRandomEnemyTemplate()
	{
		int n = Random.Range(1, 4);

		switch (n)
		{			
			case 1:
				return EnemyTemplate.DOUBLE;
			case 2:
				return EnemyTemplate.FAST;
			case 3:
				return EnemyTemplate.HEAVY;
		}

		return EnemyTemplate.DOUBLE;

	}

	static public int NumberOfEnemies()
	{
		return GameObject.FindGameObjectsWithTag("Enemy").Length;
	}

	SpawnPointScript GetRandomSpawnPoint()
	{
		return spawnPoints[Random.Range(0, spawnPoints.Count)];
	}

}


//Isso foi um erro

public struct EnemyAndCount
{
	public GameObject enemy;
	public int count;

	public EnemyAndCount(GameObject e, int c)
	{
		enemy = e;
		count = c;
	}
}
