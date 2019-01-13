using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	private int healthPoints;
	public int maxHealth;

	public int HealthPoints
	{
		get
		{
			return healthPoints;
		}

		set
		{
			healthPoints = value;

			if(healthPoints == 1)
			{
				SetVulnerability(true);
			}

			if (healthPoints <= 0)
			{
				HypeMetter.instance.comboTracker++;
				Die();
			}

		}
	}
	
	void Start () {
		HealthPoints = maxHealth;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetVulnerability(bool isVulnerable)
	{
		if (isVulnerable){
			GetComponent<SpriteRenderer>().color = Color.red;
		}
		else
		{
			GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	virtual public void Die()
	{
		Destroy(this.gameObject);
		SpawnerControler.instance.EnemyDied();
	}
}
