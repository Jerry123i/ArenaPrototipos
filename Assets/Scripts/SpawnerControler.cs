using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTemplate { NONE, HEAVY, FAST, SWARM, DOUBLE};

public class SpawnerControler : MonoBehaviour {

	public static SpawnerControler instance;

	public List<WaveConfiguration> possibleWaves;
	public List<SpawnPointScript> spawnPoints;

	public float impatiance;
	public float impatianceLimit = 20;
	private float impatianceFactor = 1;
	public float ImpatianceFactor
	{
		get
		{
			return impatianceFactor;
		}

		set
		{
			impatianceFactor = value;

			if(impatianceFactor > 2.5f)
			{
				impatianceFactor = 2.5f;
			}
			if (impatianceFactor < 0.6f)
			{
				impatianceFactor = 0.6f;
			}
		}
	}

	public int NOfEnemies
	{
		get
		{
			return GameObject.FindGameObjectsWithTag("Enemy").Length;
		}

		set
		{
			nOfEnemies = value;
		}
	}

	public float timeWoutHitting;

	private int nOfEnemies;


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
		CheckHits();

		impatiance += Time.deltaTime * ImpatianceFactor;
		timeWoutHitting += Time.deltaTime;

		if(impatiance >= impatianceLimit)
		{
			SpawnWave();			
		}	

	}

	public void EnemyDied()
	{
		if(NOfEnemies <= 1)
		{
			SpawnWave();
		}
		else if(nOfEnemies == 2)
		{
			impatianceFactor *= 1.5f;
		}
		else if(nOfEnemies == 3)
		{
			impatianceFactor *= 1.2f;
		}
	}

	void CheckHits()
	{
		if(timeWoutHitting >= 4.0f)
		{
			ImpatianceFactor *= 1.2f;
			timeWoutHitting = 0;
			Debug.Log("HIT SOMETHING!!");
		}
	}

	//Isso provavelmente deveria ser feito com listners
	public void AnouceHit()
	{
		timeWoutHitting = 0;
	}

	IEnumerator Loop()
	{
		SpawnWave();
		yield return new WaitForSeconds(35.0f);
		StartCoroutine(Loop());
	}

	void SpawnWave()
	{
		WaveConfiguration wave;
		int special = RollSpecialEnemy();
		int a = 99;
		int b = 99;
		wave = possibleWaves[Random.Range(0, possibleWaves.Count)];

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

		Debug.Log("Wave: " + wave.name);
		Debug.Log("Special: " + special.ToString());

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
					Debug.Log("Spawning Special");
					//Debug.Break();
				}
				else
				{
					GetRandomSpawnPoint().RecieveSpawnOrder(go, EnemyTemplate.NONE);
				}
			}
		}

		impatiance = 0;
		ImpatianceFactor = 1;

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
		int n = Random.Range(0, 4);

		switch (n)
		{
			case 0:
				return EnemyTemplate.SWARM;
			case 1:
				return EnemyTemplate.DOUBLE;
			case 2:
				return EnemyTemplate.FAST;
			case 3:
				return EnemyTemplate.HEAVY;
		}

		return EnemyTemplate.DOUBLE;

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
