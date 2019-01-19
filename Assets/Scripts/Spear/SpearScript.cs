using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearScript : MonoBehaviour {

	public float speed;
	public int damage;

	private bool moving;
	public bool catchable;

	private Collider2D col;

	public bool Moving
	{
		get
		{
			return moving;
		}

		set
		{
			moving = value;
			
		}
	}

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		Moving = true;
	}

	private void FixedUpdate()
	{
		if (Moving)
		{
			transform.Translate(Vector3.up * speed);
		}
		else if (CompareTag("SpearClone"))
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if((collision.CompareTag("Player") && !Moving ) | (collision.CompareTag("Player") && Moving && catchable))
		{
			PlayerPickup(collision);
		}

		if (collision.gameObject.CompareTag("Wall"))
		{
			WallHit();
		}

		if(collision.gameObject.CompareTag("Enemy"))
		{
			EnemyHit(collision);
		}

	}

	public virtual void PlayerPickup(Collider2D collision)
	{
		var playerScript = collision.GetComponent<PlayerScript>();
		playerScript.HasSpear = true;
		playerScript.HasKratos = true;
		Destroy(gameObject, 0.05f);
		
	}

	public virtual void WallHit()
	{
		Moving = false;
		//HypeMetter.instance.FinishCombo();
		HypeMetter.instance.Notify(this, NotificationType.SPEAR_WALL);
	}

	public virtual void EnemyHit(Collider2D collision)
	{

	}

	public void RotateToTarget(Transform target)
	{
		var vectorToTarget = target.position - transform.position;
		//var angleBetween = Vector3.Angle(transform.forward, vectorToTarget);
		var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
		transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
	}

}
