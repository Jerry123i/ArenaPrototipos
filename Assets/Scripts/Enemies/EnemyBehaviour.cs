﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBehaviour : MonoBehaviour {
	
	protected EnemyHealth health;
	protected EnemyBrain brain;
	protected Rigidbody2D rb;

	public float slipTime;

	public bool sliping;
	public bool shieldInvencibility;

	private bool _invincibleSpearHit;

	private PlayerScript player;

	private void Awake()
	{
		health = GetComponent<EnemyHealth>();
		brain = GetComponent<EnemyBrain>();
		rb = GetComponent<Rigidbody2D>();
	}

	virtual public void OnTriggerEnter2D(Collider2D collision)
	{
		player = GameObject.Find("Player").GetComponent<PlayerScript>();
		
		if(collision.gameObject.CompareTag("Spear") || collision.gameObject.CompareTag("SpearClone"))
		{
			if (collision.gameObject.GetComponent<SpearScript>().Moving && !_invincibleSpearHit)
			{
				GetHit(collision);
			}
		}

		if(collision.gameObject.CompareTag("Shield") && !shieldInvencibility && !player.IsCharging)
		{
			ShieldHit(collision.gameObject);
		}

		if (collision.gameObject.CompareTag("Shield") && !shieldInvencibility && player.IsCharging)
		{
			ShieldChargeHit(collision.gameObject);
		}

		if (collision.gameObject.CompareTag("Grab") && health.HealthPoints == 1){
			GrabHit(collision.gameObject);
		}

	}

	virtual public void SpearHit(GameObject spear)
	{
		Stagger(transform.position - spear.transform.position);
		ImpatianceMetter.instance.Notify(this, NotificationType.ENEMY_HIT);
		HypeMetter.instance.Notify(this, NotificationType.ENEMY_HIT);
		health.ModifyHealth(-spear.GetComponent<SpearScript>().Damage, DamageType.SPEAR);
	}

	virtual public void ShieldHit(GameObject shield)
	{
		var shieldScript = shield.GetComponent<ShieldScript>();
		health.ModifyHealth(-shieldScript.damage, DamageType.SHIELD);
		Slip((transform.position - shield.transform.position).normalized, shieldScript.pushMultiplier);
		StartCoroutine(RemoveInvencibility(shieldScript.shieldDuration));
		ImpatianceMetter.instance.Notify(this, NotificationType.ENEMY_HIT);
	}

	private void ShieldChargeHit(GameObject shield)
	{
		var shieldScript = shield.GetComponent<ShieldScript>();
		health.ModifyHealth(-shieldScript.damage, DamageType.SHIELD);
		Slip((transform.position - shield.transform.position).normalized, shieldScript.pushMultiplier * 4);
		StartCoroutine(RemoveInvencibility(shieldScript.shieldDuration));
		ImpatianceMetter.instance.Notify(this, NotificationType.ENEMY_HIT);
	}

	/*IEnumerator test(GameObject shield)
	{
		var i = 0;
		var x = Random.Range(-2f, 2f);
		var y = Random.Range(-2f, 2f);
		var v = new Vector3(x, y, 0);
		do
		{
			transform.position = shield.transform.position + v;
			i++;
		} while (i < 10);

	}*/

	virtual public void GrabHit(GameObject grab)
	{
		brain.State = EnemyStates.STUNNED;
		grab.GetComponent<GrabScript>().ConfirmGrab(brain);
	}

	void Slip(Vector2 direction , float strenghtK, float timeK = 1)
	{
		rb.AddForce(direction * strenghtK, ForceMode2D.Impulse);
		sliping = true;
		StartCoroutine(StopMovement(slipTime * timeK));
		brain.State = EnemyStates.STUNNED;
	}

	void Stagger(Vector2 direction)
	{
		Slip(direction, 5, 0.15f);
	}

	IEnumerator StopMovement(float time)
	{
		yield return new WaitForSeconds(time);

		rb.velocity = Vector3.zero;
		rb.angularVelocity = 0;
		brain.State = EnemyStates.IDLE;
		sliping = false;
	}

	IEnumerator RemoveInvencibility(float time)
	{
		shieldInvencibility = true;
		yield return new WaitForSeconds(time);
		shieldInvencibility = false;
	}

	public IEnumerator InvincibleAfterSpearHit(float time)
	{
		_invincibleSpearHit = true;
		yield return new WaitForSeconds(time);
		_invincibleSpearHit = false;
	}

	public void GetHit(Collider2D collision)
	{
		SpearHit(collision.gameObject);
		StartCoroutine(InvincibleAfterSpearHit(0.5f));
	}

	virtual public void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerScript>().GetIsInvulnerable(EnemyDamageTypes.MELEE))
		{
			HitPlayer(collision.gameObject);
		}
		if(collision.gameObject.CompareTag("Enemy"))
		{			
			if (collision.gameObject.GetComponent<EnemyBehaviour>().sliping && !sliping)
			{
				Slip(transform.position - collision.transform.position, 3, 0.5f);
			}
		}
		if(collision.gameObject.CompareTag("Reflected"))
		{
			health.ModifyHealth(-1, DamageType.PROJECTILE);
			Stagger(transform.position - transform.position);
			SpawnerControler.instance.AnouceHit();
		}

	}

	virtual public void HitPlayer(GameObject player)
	{
		var playerScript= player.GetComponent<PlayerScript>();
		if (!playerScript.IsCharging)
		{
			playerScript.Health -= 1;
			player.GetComponent<SpecialsController>().Charge = 0;
		}
		SpawnerControler.instance.AnouceHit();
	}

}
