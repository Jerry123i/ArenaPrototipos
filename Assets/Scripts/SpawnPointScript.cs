using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour {

	public List<IEnumerator> list;
	public Coroutine current;

	public AnimationClip animationClip;

	private void Awake()
	{
		list = new List<IEnumerator>();
	}

	//Isso aqui ta uma zona generalizada
	public void RecieveSpawnOrder(GameObject enemy, EnemyTemplate template)
	{
		EnemyAndCount eAc;

		int n;

		switch (template)
		{
			case EnemyTemplate.NONE:
				n = 1;
				break;
			case EnemyTemplate.FAST:
				n = 1;
				break;
			case EnemyTemplate.HEAVY:
				n = 1;
				break;
			case EnemyTemplate.DOUBLE:
				n = 2;
				break;
			default:
				n = 1;
				break;
		}

		list.Add(Spawn(eAc = new EnemyAndCount(enemy, n), template));
		
		if(current == null)
		{
			current = StartCoroutine(list[0]);
		}

	}

	private IEnumerator Spawn(EnemyAndCount eAc, EnemyTemplate template)
	{
		float time = Random.Range(1.0f, 2.0f);

		yield return new WaitForSeconds(time - animationClip.averageDuration);

		GetComponent<Animator>().SetTrigger("Spawn");

		yield return new WaitForSeconds(time - (time - animationClip.averageDuration));

		for(int i = 0; i<eAc.count; i++)
		{
			GameObject instance =  Instantiate(eAc.enemy, transform.position, transform.rotation);

			switch (template)
			{
				case EnemyTemplate.NONE:
					//
					break;
				case EnemyTemplate.FAST:
					AlterLight(instance);
					break;
				case EnemyTemplate.HEAVY:
					AlterHeavy(instance);
					break;
				case EnemyTemplate.DOUBLE:
					//
					break;
				default:
					//
					break;
			}


		}

		if (list.Count > 1)
		{
			current = StartCoroutine(list[1]);
		}
		else
		{
			current = null;
		}

		list.RemoveAt(0);
		
	}

	EnemyAndCount AlterHeavy(GameObject go)
	{
		Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
		EnemyBehaviour behaviour = go.GetComponent<EnemyBehaviour>();
		EnemyBrain brain = go.GetComponent<EnemyBrain>();
		Transform transform = go.transform;
		SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
		EnemyHealth healyh = go.GetComponent<EnemyHealth>();

		transform.localScale *= 1.3f;
		brain.speed *= 0.6f;
		behaviour.slipTime *= 0.5f;
		healyh.maxHealth += 2;

		sprite.color = Color.yellow;

		return new EnemyAndCount(go, 1);

	}

	EnemyAndCount AlterLight(GameObject go)
	{
		Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
		EnemyBehaviour behaviour = go.GetComponent<EnemyBehaviour>();
		EnemyBrain brain = go.GetComponent<EnemyBrain>();
		Transform transform = go.transform;
		SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();

		transform.localScale *= 0.7f;
		brain.speed *= 1.7f;
		behaviour.slipTime *= 1.5f;

		sprite.color = Color.blue;

		return new EnemyAndCount(go, 1);
	}

	EnemyAndCount AlterSwarm(GameObject go)
	{
		Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
		EnemyBehaviour behaviour = go.GetComponent<EnemyBehaviour>();
		EnemyBrain brain = go.GetComponent<EnemyBrain>();
		Transform transform = go.transform;
		SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
		EnemyHealth health = go.GetComponent<EnemyHealth>();

		if(health.maxHealth > 1)
		{
			health.maxHealth -= 1;
		}	
		transform.localScale *= 0.5f;
		brain.speed *= 2.5f;
		behaviour.slipTime *= 1.6f;


		sprite.color = Color.cyan;

		return new EnemyAndCount(go, 3);

	}


}
