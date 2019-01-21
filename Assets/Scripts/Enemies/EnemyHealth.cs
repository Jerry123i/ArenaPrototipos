using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { SPEAR, SHIELD, PROJECTILE, UNDEFINED, GRAB};

public class EnemyHealth : MonoBehaviour {

	private int healthPoints;
	public int maxHealth;

	public int HealthPoints
	{
		get
		{
			return healthPoints;
		}

		private set
		{
			healthPoints = value;

			if (healthPoints > maxHealth)
			{
				healthPoints = maxHealth;
			}

			if (healthPoints == 1)
			{
				SetVulnerability(true);
			}

			if(healthPoints <= 0)
			{
				Die(DamageType.UNDEFINED);
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

	public void ModifyHealth(int value, DamageType damageType)
	{
		healthPoints += value;

		if (healthPoints == 1)
		{
			SetVulnerability(true);
		}

		if (healthPoints <= 0)
		{
			Die(damageType);
		}
	}

	virtual public void Die(DamageType damageType)
	{
		ImpatianceMetter.instance.Notify(this, NotificationType.ENEMY_KILLED);

		if(damageType == DamageType.SPEAR)
		{
			HypeMetter.instance.Notify(this, NotificationType.ENEMY_KILLED);

		}

		Destroy(this.gameObject);
	}
}
