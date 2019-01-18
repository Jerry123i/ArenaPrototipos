using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

	public int damage;
	public float pushMultiplier = 1;

	public float shieldDuration = 0.6f;

	private void Awake()
	{
		StartCoroutine(ShieldLifetime());
	}

	IEnumerator ShieldLifetime()
	{
		yield return new WaitForSeconds(shieldDuration);
		Destroy(gameObject);
	}

}
