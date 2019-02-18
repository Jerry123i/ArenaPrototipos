using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviour : EnemyBehaviour
{

	public override void SpearHit(GameObject spear)
	{
		base.SpearHit(spear);

		if(health.HealthPoints > 0)
		{
			spear.GetComponent<SpearScript>().WallHit();
			spear.transform.SetParent(transform);
		
		}
	}

}
