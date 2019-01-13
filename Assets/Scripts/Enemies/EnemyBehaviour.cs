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

	private void Awake()
	{
		health = GetComponent<EnemyHealth>();
		brain = GetComponent<EnemyBrain>();
		rb = GetComponent<Rigidbody2D>();
	}

	virtual public void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Spear")
		{
			if (collision.gameObject.GetComponent<SpearScript>().Moving)
			{
				SpearHit(collision.gameObject);
			}
		}

		if(collision.gameObject.tag == "Shield" && !shieldInvencibility)
		{
			ShieldHit(collision.gameObject);
		}

	}

	virtual public void SpearHit(GameObject spear)
	{
		health.HealthPoints -= spear.GetComponent<SpearScript>().damage;
		Stagger(transform.position - spear.transform.position);
		SpawnerControler.instance.AnouceHit();
	}

	virtual public void ShieldHit(GameObject shield)
	{
		ShieldScript shieldScript = shield.GetComponent<ShieldScript>();		
		health.HealthPoints -= shieldScript.damage;
		Slip((transform.position - shield.transform.position).normalized, shieldScript.pushMultiplier);
		StartCoroutine(RemoveInvencibility(shieldScript.shieldDuration));
		SpawnerControler.instance.AnouceHit();
	}

	void Slip(Vector2 direction , float strenghtK, float timeK = 1)
	{
		rb.AddForce(direction * strenghtK, ForceMode2D.Impulse);
		sliping = true;
		StartCoroutine(StopMovement(slipTime * timeK));
		brain.state = EnemyStates.STUNNED;
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
		brain.state = EnemyStates.IDLE;
		sliping = false;
	}

	IEnumerator RemoveInvencibility(float time)
	{
		shieldInvencibility = true;
		yield return new WaitForSeconds(time);
		shieldInvencibility = false;
	}

	virtual public void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Player")
		{
			HitPlayer(collision.gameObject);
		}
		if(collision.gameObject.tag == "Enemy")
		{			
			if (collision.gameObject.GetComponent<EnemyBehaviour>().sliping && !sliping)
			{
				Slip(transform.position - collision.transform.position, 3, 0.5f);
			}
		}

	}

	virtual public void HitPlayer(GameObject player)
	{
		player.GetComponent<PlayerScript>().Health -= 1;
		SpawnerControler.instance.AnouceHit();
	}

}