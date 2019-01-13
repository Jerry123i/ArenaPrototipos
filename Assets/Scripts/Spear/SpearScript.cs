using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearScript : MonoBehaviour {

	public float speed;
	public int damage;

	private bool moving;

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
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player" && !Moving)
		{
			PlayerPickup(collision);
		}

		if (collision.gameObject.tag == "Wall")
		{
			WallHit();
		}

		if(collision.gameObject.tag == "Enemy")
		{
			EnemyHit(collision);
		}

	}

	public virtual void PlayerPickup(Collider2D collision)
	{
		collision.GetComponent<PlayerScript>().HasSpear = true;
		Destroy(this.gameObject);
	}

	public virtual void WallHit()
	{
		Moving = false;
		HypeMetter.instance.FinishCombo();
	}

	public virtual void EnemyHit(Collider2D collision)
	{

	}

}
