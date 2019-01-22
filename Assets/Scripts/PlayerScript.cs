using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour {

	private PlayerUIControler uIControler;
	private Rigidbody2D rb;

	private Animator animator;

	public Sprite wSpear;
	public Sprite woutSpear;

	public GameObject spear;
	public GameObject shield;
	public GameObject grab;
	private GameObject grabInstance;
	
	private SpecialsController _specialsController;
	

	public float shieldCd = 2.0f;
	public float dodgeCd = 3.5f;

	Vector2 movmentDirection;

	bool isOnShieldCooldown;
	bool isOnDodgeCooldown;

	bool isDodging;
	public bool isGrabing;

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

	public bool IsDodging
	{
		get
		{
			return isDodging;
		}

		set
		{
			isDodging = value;
			animator.SetBool("Dodging", value);
		}
	}

	private void Start () {
		HasSpear = true;
		uIControler = GetComponent<PlayerUIControler>();
		uIControler.UpdateHealth(_health);
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		_specialsController = GetComponent<SpecialsController>();
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Fire2") && HasSpear)
		{
			ShootSpear(); // this method contains normal shooting and split shot shooting
		}
		else if (Input.GetButtonDown("Fire2") && !HasSpear) // kratos handler is here
		{
			var c = GameObject.FindGameObjectWithTag("Spear").GetComponent<SpearScript>();
			// kratos power pull the spear back from wall
			if (!c.Moving && _specialsController.HasKratos && !_specialsController.SpecialOnCd && _specialsController.SpecialReady)
			{
				Debug.Log("CARAI");
				StartCoroutine(_specialsController.SpecialCooldown(SpecialsController.Specials.Kratos));
			}
			
		}
		if (Input.GetButtonDown("Fire1") && !isOnShieldCooldown)
		{
			ShieldBash();
		}
		if (Input.GetButtonDown("Jump") && (movmentDirection.magnitude != 0) && !IsDodging && !isOnDodgeCooldown)
		{
			StartCoroutine(Dodge(movmentDirection));
		}
		if (Input.GetButtonDown("Taunt"))
		{
			Taunt();
		}
		if (Input.GetButtonDown("Grab"))
		{
			Grab();
		}
	}

	private void FixedUpdate()
	{
		movmentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (IsDodging || isGrabing) return;
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

	
	void Taunt()
	{
		ImpatianceMetter.instance.Notify(this, NotificationType.TAUNT);
		animator.SetTrigger("Taunt");
	}
	
	private void ShootSpear()
	{
		if (!hasSpear) return;
		HasSpear = false;

		// normal shooting
		if (!_specialsController.HasSplitShot | !_specialsController.SpecialReady)
		{
			var newSpear = Instantiate(spear, transform.position, transform.rotation);
			newSpear.transform.Translate(0, 0.5f, 0, Space.Self);
			HypeMetter.instance.Notify(this, NotificationType.PLAYER_SPEAR);
		}
		
		// split shot shooting
		else if (_specialsController.HasSplitShot && !_specialsController.SpecialOnCd && _specialsController.SpecialReady)
		{
			StartCoroutine(_specialsController.SpecialCooldown(SpecialsController.Specials.SplitShot));
			var spear1 = Instantiate(spear, transform.position, transform.rotation);
			spear1.transform.Translate(0, 0.5f, 0, Space.Self);
			var spear2 = Instantiate(spear, transform.position, Quaternion.AngleAxis(-_specialsController.SplitShotOffsset, Vector3.forward) * transform.rotation);
			spear2.transform.Translate(0, 0.5f, 0, Space.Self);
			var spear3 = Instantiate(spear, transform.position, Quaternion.AngleAxis(_specialsController.SplitShotOffsset, Vector3.forward) * transform.rotation);
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

	private void Grab()
	{
		grabInstance = Instantiate(grab, new Vector3(transform.position.x, transform.position.y), transform.rotation, transform);
		grabInstance.transform.Translate(0, 0.7f, 0, Space.Self);
		grabInstance.GetComponent<GrabScript>().player = this;
	}
	public void FinishGrab()
	{
		//GG
		isGrabing = false;
		Debug.Log("FINISHED GRAB");
		gameObject.layer = 8;
		HypeMetter.instance.CurrentHype += HypeMetter.instance.n * 3f;
		FindObjectOfType<ArenaControler>().EndArena();
	}

	private IEnumerator Dodge(Vector2 direction)
	{
		IsDodging = true;
		gameObject.layer = 8;		
		StartCoroutine(DodgeCooldown());
		

		float clock = 0;

		while(clock <= dodgeTime)
		{
			transform.Translate(direction.normalized * dodgeVelocity * Time.deltaTime, Space.World);
			clock += Time.deltaTime;
			yield return null;
		}

		IsDodging = false;
		gameObject.layer = 0;		

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
		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		HypeMetter.instance.CurrentHype -= HypeMetter.instance.tier1Value * 0.4f; //Fazer isso por Notify depois
		FindObjectOfType<ArenaControler>().EndArena();
	}

	private void TakeDamage(int dmg)
	{
		GetComponent<Animator>().SetTrigger("Damage");
		ImpatianceMetter.instance.Notify(this, NotificationType.PLAYER_TOOK_DAMAGE);
		if (isGrabing)
		{
			grabInstance.GetComponent<GrabScript>().Interupt();
		}
	}

}
