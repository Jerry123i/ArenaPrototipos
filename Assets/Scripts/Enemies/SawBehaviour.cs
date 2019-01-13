using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBehaviour : EnemyBehaviour {

	public override void OnTriggerEnter2D(Collider2D collision)
	{
	}

	public override void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			HitPlayer(collision.gameObject);
		}
	}

}
