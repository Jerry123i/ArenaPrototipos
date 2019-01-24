using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

	public float speed;
	public bool reflected;

	private void FixedUpdate()
	{
		transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		var objectTag = collision.gameObject.tag;

		switch (objectTag)
		{
			case "Wall":
				Destroy(gameObject);
				break;

			case "Player":
				//if (!collision.gameObject.GetComponent<PlayerScript>().IsCharging)
				if (!collision.gameObject.GetComponent<PlayerScript>().GetIsInvulnerable(EnemyDamageTypes.RANGED))
				{
					collision.gameObject.GetComponent<PlayerScript>().Health -= 1;
					collision.gameObject.GetComponent<SpecialsController>().Charge = 0;
				}
				Destroy(gameObject);
				break;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Shield"))
		{
			var specialsController = GameObject.FindGameObjectWithTag("Player").GetComponent<SpecialsController>();
			if (specialsController.HasReflectShield && !specialsController.SpecialOnCd)
			{
				StartCoroutine(specialsController.SpecialCooldown(SpecialsController.Specials.ReflectShield));
				speed = -speed;
				tag = "Reflected";
				reflected = true;
				gameObject.layer = 0;
			}
		}
	}
}
