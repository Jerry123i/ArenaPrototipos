using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

	private PlayerUIControler uIControler;
	private Rigidbody2D rb;

	public Sprite wSpear;
	public Sprite woutSpear;

	public GameObject spear;
	public GameObject shield;

	public float shieldCd = 2.0f;
	public float dodgeCd = 3.5f;

	Vector2 movmentDirection;

	bool isOnShieldCooldown	= false;
	bool isOnDodgeCooldown	= false;

	bool isDodging = false;

	public float moveVelocity;
	public float dodgeVelocity;
	public float dodgeTime;

	// specials
	private bool splitShot = false;
	private float _splitShotOffsset = 15f;

	public bool _hasKratos = true; 
	
	
	private bool hasSpear;
	public bool HasSpear
	{
		get
		{
			return hasSpear;
		}

		set
		{
			if(value == hasSpear)
			{
				return;
			}

			if (value)
			{
				hasSpear = true;
				GetComponent<SpriteRenderer>().sprite = wSpear;
			}
			else
			{
				hasSpear = false;
				GetComponent<SpriteRenderer>().sprite = woutSpear;
			}

		}
	}

	private int _health = 5;
	public int Health
	{
		get
		{
			return _health;
		}

		set
		{
			if(value < _health)
			{
				TakeDamage(_health - value);
			}

			_health = value;

			uIControler.UpdateHealth(_health);

			if(_health <= 0)
			{
				Die();
			}
		}
	}

	private void Start () {
		HasSpear = true;
		uIControler = GetComponent<PlayerUIControler>();
		uIControler.UpdateHealth(_health);
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Fire2") && HasSpear)
		{
			ShootSpear();
			
		}
		else if (Input.GetButtonDown("Fire2") && !HasSpear)
		{
			var c = GameObject.FindGameObjectWithTag("Spear").GetComponent<SpearScript>();
			if (!c.Moving && _hasKratos)
			{
				c.RotateToTarget(transform);
				c.Moving = true;
				c.catchable = true;
				_hasKratos = false;
			}
			
		}
		if (Input.GetButtonDown("Fire1") && !isOnShieldCooldown)
		{
			ShieldBash();
		}
		if (Input.GetButtonDown("Jump") && (movmentDirection.magnitude != 0) && !isDodging && !isOnDodgeCooldown)
		{
			StartCoroutine(Dodge(movmentDirection));
		}
	}

	private void FixedUpdate()
	{
		movmentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (isDodging) return;
		Rotation();
		Movement();
	}
	

	private void Rotation()
	{
		var mouseScreen = Input.mousePosition;
		var mouse = Camera.main.ScreenToWorldPoint(mouseScreen);

		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg - 90);
	}

	private void Movement()
	{
		if (Input.GetAxisRaw("Vertical") != 0)
		{
			transform.Translate(new Vector3(0, Input.GetAxisRaw("Vertical") * moveVelocity * Time.deltaTime, 0), Space.World);
		}

		if(Input.GetAxisRaw("Horizontal") != 0)
		{
			transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveVelocity * Time.deltaTime, 0, 0), Space.World);
		}
	}

	
	
	
	private void ShootSpear()
	{
		if (!hasSpear) return;
		HasSpear = false;

		if (!splitShot)
		{
			var newSpear = Instantiate(spear, transform.position, transform.rotation);
			newSpear.transform.Translate(0, 0.5f, 0, Space.Self);
		}
		else
		{
			var spear1 = Instantiate(spear, transform.position, transform.rotation);
			spear1.transform.Translate(0, 0.5f, 0, Space.Self);
			var spear2 = Instantiate(spear, transform.position, Quaternion.AngleAxis(-_splitShotOffsset, Vector3.forward) * transform.rotation);
			spear2.transform.Translate(0, 0.5f, 0, Space.Self);
			var spear3 = Instantiate(spear, transform.position, Quaternion.AngleAxis(_splitShotOffsset, Vector3.forward) * transform.rotation);
			spear3.transform.Translate(0, 0.5f, 0, Space.Self);
			spear2.tag = "SpearClone";
			spear3.tag = "SpearClone";
		}
	}

	private void ShieldBash()
	{
		var newShield = Instantiate(shield, new Vector3(transform.position.x, transform.position.y), transform.rotation, transform);
		newShield.transform.Translate(0, 0.7f, 0, Space.Self);
		StartCoroutine(ShieldCooldoown());
		
	}


	private IEnumerator Dodge(Vector2 direction)
	{
		isDodging = true;
		gameObject.layer = 8;
		GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
		StartCoroutine(DodgeCooldown());
		

		float clock = 0;

		while(clock <= dodgeTime)
		{
			transform.Translate(direction.normalized * dodgeVelocity * Time.deltaTime, Space.World);
			clock += Time.deltaTime;
			yield return null;
		}

		isDodging = false;
		gameObject.layer = 0;
		GetComponent<SpriteRenderer>().color = Color.white;

	}

	IEnumerator ShieldCooldoown()
	{
		isOnShieldCooldown = true;
		StartCoroutine(uIControler.StartShieldClock(shieldCd));
		yield return new WaitForSeconds(shieldCd);
		isOnShieldCooldown = false;
	}

	IEnumerator DodgeCooldown()
	{
		isOnDodgeCooldown = true;
		StartCoroutine(uIControler.StartDodgeClock(dodgeCd));
		yield return new WaitForSeconds(dodgeCd);
		isOnDodgeCooldown = false;
	}

	private void Die()
	{
		Destroy(gameObject);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void TakeDamage(int dmg)
	{
		Debug.Log("DAMAGE " + dmg.ToString());
	}

}
