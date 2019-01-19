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
				HypeMetter.instance.Notify(this, NotificationType.ENEMY_KILLED);
				Die();
			}

		}
	}
	
	void Start () {
		HealthPoints = maxHealth;	
	}
	
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
		ImpatianceMetter.instance.Notify(this, NotificationType.ENEMY_KILLED);
		Destroy(this.gameObject);
	}
}
