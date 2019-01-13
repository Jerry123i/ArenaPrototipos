using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBrain : EnemyBrain {

	public float maxDistance;
	public float minDistance;

	public GameObject projectile;

	public float timeBetweenShots;

	private void Awake()
	{
		StartCoroutine(FiringLoop());
	}

	public override void ChaseBehavior()
	{
		Vector3 playerPos = player.transform.position;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x) * Mathf.Rad2Deg - 90);

		float distance = (playerPos - transform.position).magnitude;
		
		if(distance > maxDistance)
		{
			transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
		}

		if(distance < minDistance)
		{
			transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);
		}

	}

	public virtual void FireProjectile()
	{
		GameObject p;

		p = Instantiate(projectile, transform.position, transform.rotation);
		p.transform.Translate(0, 0.5f, 0, Space.Self);
	}

	public IEnumerator FiringLoop()
	{
		yield return new WaitForSeconds(timeBetweenShots);
		FireProjectile();
		StartCoroutine(FiringLoop());
	}

}
