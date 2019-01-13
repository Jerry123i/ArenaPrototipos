using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

	public float speed;

	private void FixedUpdate()
	{
		transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		string tag = collision.gameObject.tag;

		switch (tag)
		{
			case "Wall":
				Destroy(this.gameObject);
				break;

			case "Player":
				collision.gameObject.GetComponent<PlayerScript>().Health -= 1;
				Destroy(this.gameObject);
				break;
		}


	}

}
