using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { IDLE, CHASE, FLEE, STUNNED}

public class EnemyBrain : MonoBehaviour {

	public float speed = 0.1f;

	protected GameObject player;

	protected EnemyStates state;

	private Rigidbody2D rb;

	public GameObject Player
	{
		get
		{			
			return player;
		}

		set
		{
			player = value;
			if(player == null)
			{
				State = EnemyStates.IDLE;
			}
		}
	}

	public virtual EnemyStates State
	{
		get
		{
			return state;
		}

		set
		{
			state = value;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		LookForPlayer();
	}

	private void Update()
	{
		switch (State)
		{
			case EnemyStates.CHASE:
				if(player != null)
				{
					ChaseBehavior();
				}
				break;
			case EnemyStates.FLEE:
				FleeBehaviour();
				break;
			case EnemyStates.IDLE:
				IdleBehaviour();
				break;
			case EnemyStates.STUNNED:
				StunedBehaviour();
				break;

		}
	}

	virtual public void ChaseBehavior()
	{
		Vector3 playerPos = player.transform.position;

		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x) * Mathf.Rad2Deg - 90);

		if ((playerPos - transform.position).magnitude >= 0.2f)
		{
			transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
		}
	}

	virtual public void FleeBehaviour()
	{

	}

	virtual public void IdleBehaviour()
	{
		transform.Rotate(Vector3.back, Time.deltaTime * 30.0f);
		LookForPlayer();
	}

	virtual public void StunedBehaviour()
	{

	}

	void LookForPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			State = EnemyStates.CHASE;
		}
	}

}
