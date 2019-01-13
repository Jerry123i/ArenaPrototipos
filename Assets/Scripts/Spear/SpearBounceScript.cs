using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearBounceScript : SpearScript {

	public int bounces = 1;
	public float xLimit;
	public float yLimiy;

	public override void WallHit()
	{
		if(bounces > 0)
		{
			bounces--;

			Debug.Log(transform.eulerAngles.ToString());

			transform.Rotate(0, 0, -90, Space.Self);

			//base.WallHit();

		}
		else
		{
			base.WallHit();
		}

	}

}
