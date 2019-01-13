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

	private int health = 5;
	public int Health
	{
		get
		{
			return health;
		}

		set
		{
			if(value < health)
			{
				TakeDamage(health - value);
			}

			health = value;

			uIControler.UpdateHealth(health);

			if(health <= 0)
			{
				Die();
			}
		}
	}

	void Start () {
		HasSpear = true;
		uIControler = GetComponent<PlayerUIControler>();
		uIControler.UpdateHealth(health);
		rb = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
	{
		
	}

	private void FixedUpdate()
	{
		movmentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (!isDodging)
		{
			Rotation();
			Movement();
		}

		if (Input.GetButton("Fire2"))
		{
			ShootSpear();
		}
		if (Input.GetButton("Fire1") && !isOnShieldCooldown)
		{
			ShieldBash();
		}
		if (Input.GetButton("Jump") && (movmentDirection.magnitude != 0) && !isDodging && !isOnDodgeCooldown)
		{
			StartCoroutine(Dodge(movmentDirection));
		}

	}

	private void Rotation()
	{
		Vector3 mouseScreen = Input.mousePosition;
		Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);

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
		if (hasSpear)
		{
			HasSpear = false;

			GameObject newSpear;

			newSpear = Instantiate(spear, transform.position, transform.rotation);
			newSpear.transform.Translate(0, 0.5f, 0, Space.Self);
		}
	}

	private void ShieldBash()
	{
		GameObject newShield;
		newShield = Instantiate(shield, new Vector3(transform.position.x, transform.position.y), transform.rotation, transform);
		newShield.transform.Translate(0, 0.7f, 0, Space.Self);
		StartCoroutine(ShieldCooldoown());
		
	}


	private IEnumerator Dodge(Vector2 direction)
	{
		isDodging = true;
		this.gameObject.layer = 8;
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
		this.gameObject.layer = 0;
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
		Destroy(this.gameObject);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void TakeDamage(int dmg)
	{
		Debug.Log("DAMAGE " + dmg.ToString());
	}

}
