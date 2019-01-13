using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBrain : MonoBehaviour {

	public Vector2 direction;

	public float xLimit;
	public float yLimit;

	public float speed;

	// Use this for initialization
	void Start () {
		direction = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
		direction = direction.normalized;
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(direction*Time.deltaTime*speed, Space.World);

		if(transform.position.x > xLimit)
		{
			direction = new Vector2(-(Mathf.Abs(direction.x)), direction.y  + (Random.Range(-0.2f,0.2f)));
			direction = direction.normalized;
		}
		
		if(transform.position.x < -xLimit)
		{
			direction = new Vector2((Mathf.Abs(direction.x)), direction.y + (Random.Range(-0.2f, 0.2f)));
			direction = direction.normalized;
		}


		if(transform.position.y > yLimit)
		{
			direction = new Vector2(direction.x + (Random.Range(-0.2f, 0.2f)), -(Mathf.Abs(direction.y)));
			direction = direction.normalized;
		}

		if(transform.position.y < -yLimit)
		{
			direction = new Vector2(direction.x + (Random.Range(-0.2f, 0.2f)), (Mathf.Abs(direction.y)));
			direction = direction.normalized;
		}

	}
}
