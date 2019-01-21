using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabScript : MonoBehaviour
{

	public float duration;
	public float holdTime;

	public PlayerScript player;

	public Animator animator;

	public Coroutine enumerator;

	public EnemyBrain grabedEnemy;

	private void Awake()
	{
		enumerator = StartCoroutine(HoldLifetime());		
	}

	IEnumerator HoldLifetime()
	{
		yield return new WaitForSeconds(duration);
		Debug.Log("HoldLifetime");
		Destroy(this.gameObject);
	}

	IEnumerator HoldDuration()
	{		
		GetComponent<Collider2D>().enabled = false;
		animator.SetTrigger("Start");
		yield return new WaitForSeconds(holdTime);
		grabedEnemy.GetComponent<EnemyHealth>().Die(DamageType.GRAB);
		player.FinishGrab();
		Destroy(this.gameObject);
	}

	public void ConfirmGrab(EnemyBrain enemy)
	{
		Debug.Log("Confirm grabed " + enemy.name);
		StopCoroutine(enumerator);
		enumerator = StartCoroutine(HoldDuration());
		grabedEnemy = enemy;
		player.isGrabing = true;
	}

	public void Interupt()
	{
		Debug.Log("GRAB INTERRUPTED");
		StopCoroutine(enumerator);
		grabedEnemy.State = EnemyStates.IDLE;
		player.isGrabing = false;
		Destroy(this.gameObject);
	}

}
