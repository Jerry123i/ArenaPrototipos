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
		else if (CompareTag("SpearClone"))
		{
			Destroy(gameObject);
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
		Destroy(gameObject, 0.05f);
	}

	public virtual void WallHit()
	{
		Moving = false;
		HypeMetter.instance.FinishCombo();
	}

	public virtual void EnemyHit(Collider2D collision)
	{

	}
	
	public IEnumerator Kratos()
	{
		var target = GameObject.FindGameObjectWithTag("Player").transform;
		var vectorToTarget = target.position - transform.position;
		var angleBetween = Vector3.Angle(transform.forward, vectorToTarget);
		print(vectorToTarget);
		var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
		var q = Quaternion.AngleAxis(angle, Vector3.up);
		transform.rotation = q;
		
		// moving towards player
		var pos = target.position;
		do
		{
			moving = true;
			speed = 0;
			print(transform.position.x);
			print(transform.position.y);
			transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y).normalized,
				pos, 2 * Time.deltaTime);
			yield return new WaitForSeconds(0.001f);
		} while (moving);
		//PlayerPickup(target.gameObject.GetComponent<Collider2D>());
	}

	public IEnumerator KratosFunc()
	{
		var target = GameObject.FindGameObjectWithTag("Player").transform;
		var vectorToTarget = target.position - transform.position;
		var angleBetween = Vector3.Angle(transform.forward, vectorToTarget);
		print(vectorToTarget);
		print(angleBetween);
		var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
		var q = Quaternion.AngleAxis(angle, Vector3.up);
		transform.rotation = q;
		
		var pos = target.position;
		do
		{
			moving = true;
			speed = 0;
			transform.Translate(transform.up * Time.deltaTime * 5);
			yield return new WaitForSeconds(0.001f);
		} while (moving);
		
		
		
	}

}
