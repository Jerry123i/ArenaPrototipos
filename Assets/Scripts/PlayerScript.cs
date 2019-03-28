using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[Flags] public enum EnemyDamageTypes{
	NONE = 0,
	MELEE = 1,
	RANGED = 2,
	ALL = 3
}

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
	
	public SpecialsController _specialsController;
	

	public float shieldCd = 2.0f;
	public float dodgeCd = 3.5f;

	Vector2 movmentDirection;

	bool isOnShieldCooldown;
	bool isOnDodgeCooldown;

	bool isDodging;
	private bool isGrabing;
	public bool isLockedMovment;
	public bool isLockedAtack;
	private EnemyDamageTypes isInvulnerable;
	[FormerlySerializedAs("_isCharging")] public bool IsCharging;

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

	public bool IsGrabing
	{
		get
		{
			return isGrabing;
		}

		set
		{
			isGrabing = value;
			isLockedMovment = value;
			isLockedAtack = value;
		}
	}

	public EnemyDamageTypes IsInvulnerable
	{
		protected get
		{
			return isInvulnerable;
		}

		set
		{
			isInvulnerable = value;
			if(isInvulnerable == EnemyDamageTypes.ALL)
			{
				animator.SetBool("Invulnerable", true);
			}
			else
			{
				animator.SetBool("Invulnerable", false);
			}
		}
	}

	public bool GetIsInvulnerable(EnemyDamageTypes damageType)
	{
		return (damageType & IsInvulnerable) != 0;
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
		if (Input.GetButtonUp("Fire2") && HasSpear && !isLockedAtack)
		{
			ShootSpear(); // this method contains normal shooting and split shot shooting
		}
		else if (Input.GetButtonDown("Fire2") && !HasSpear && !isLockedAtack) // kratos handler is here
		{
			var c = GameObject.FindGameObjectWithTag("Spear").GetComponent<SpearScript>();
			// kratos power pull the spear back from wall
			if (!c.Moving && _specialsController.HasKratos && !_specialsController.SpecialOnCd)
			{
				StartCoroutine(_specialsController.SpecialCooldown(SpecialsController.Specials.Kratos));
			}
			
		}
		if (Input.GetButtonDown("Fire1") && !isOnShieldCooldown && !isLockedAtack)
		{
			ShieldBash();
		}
		// shield charge special handler
		if (Input.GetButtonUp("Fire1") && !_specialsController.SpecialOnCd && _specialsController.SpecialReady &&!isLockedAtack)
		{
			ShieldCharge();
			
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

		if (isLockedMovment) return;

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


	private void Taunt()
	{
		ImpatianceMetter.instance.Notify(this, NotificationType.TAUNT);
		animator.SetTrigger("Taunt");
	}
	
	private void ShootSpear()
	{
		if (!hasSpear) return;
		HasSpear = false;

		// normal shooting
		if ((!_specialsController.HasSplitShot || !_specialsController.SpecialReady))
		{
			var newSpear = Instantiate(spear, transform.position, transform.rotation);
			newSpear.transform.Translate(0, 0.5f, 0, Space.Self);
			HypeMetter.instance.Notify(this, NotificationType.PLAYER_SPEAR);
		}
		
		// split shot shooting
		else if (_specialsController.HasSplitShot && !_specialsController.SpecialOnCd && _specialsController.SpecialReady && !_specialsController.HasFastSpear)
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
		else if (_specialsController.HasFastSpear && !_specialsController.SpecialOnCd && _specialsController.SpecialReady)
		{
			var newSpear = Instantiate(spear, transform.position, transform.rotation);
			newSpear.transform.Translate(0, 0.5f, 0, Space.Self);
			HypeMetter.instance.Notify(this, NotificationType.PLAYER_SPEAR);
		}
	}

	private void ShieldBash()
	{
		var newShield = Instantiate(shield, new Vector3(transform.position.x, transform.position.y), transform.rotation, transform);
		newShield.transform.Translate(0, 0.7f, 0, Space.Self);
		StartCoroutine(ShieldCooldoown());
		StartCoroutine(InvulnerabilityCooldown(newShield.GetComponent<ShieldScript>().shieldDuration + 1f, EnemyDamageTypes.MELEE));
	}

	private void ShieldCharge()
	{
		if (!_specialsController.HasShieldCharge)
			return;
		StartCoroutine(_specialsController.SpecialCooldown(SpecialsController.Specials.ShieldCharge));
		var newShield = Instantiate(shield, new Vector3(transform.position.x, transform.position.y), transform.rotation, transform);
		newShield.transform.localScale = new Vector3(1.7f, 1.7f, 0);
		newShield.transform.Translate(0, 0.7f, 0, Space.Self);
		StartCoroutine(ChargeForward());
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
		isLockedMovment = true;
		gameObject.layer = 8;		
		StartCoroutine(DodgeCooldown());
		

		float clock = 0;

		while(clock <= (dodgeTime*0.9f))
		{
			transform.Translate(direction.normalized * dodgeVelocity * Time.deltaTime, Space.World);
			clock += Time.deltaTime;
			yield return null;
		}

		isLockedMovment = false;

		while(clock <= dodgeTime)
		{
			clock += Time.deltaTime;
			yield return null;
		}

		IsDodging = false;


		gameObject.layer = 0;		

	}

	private IEnumerator InvulnerabilityCooldown(float time, EnemyDamageTypes damageTypes)
	{
		IsInvulnerable = damageTypes;
		yield return new WaitForSeconds(time);
		IsInvulnerable = EnemyDamageTypes.NONE;
	}

	private IEnumerator ShieldCooldoown()
	{
		isOnShieldCooldown = true;
		StartCoroutine(uIControler.StartShieldClock(shieldCd));
		yield return new WaitForSeconds(shieldCd);
		isOnShieldCooldown = false;
	}

	private IEnumerator DodgeCooldown()
	{
		isOnDodgeCooldown = true;
		StartCoroutine(uIControler.StartDodgeClock(dodgeCd));
		yield return new WaitForSeconds(dodgeCd);
		isOnDodgeCooldown = false;
	}

	private IEnumerator ChargeForward()
	{
		var i = 0;
		var direction = Vector3.up / 3;
		IsCharging = true;
		var rb = GetComponent<Rigidbody2D>();
		//rb.isKinematic = true;
		//GetComponent<CircleCollider2D>().enabled = false;
		do
		{
			rb.velocity = Vector3.zero;
			transform.Translate(direction);
			yield return new WaitForSeconds(0.01f);
			i++;
		} while (i < 30);
		IsCharging = false;
		_specialsController.Charge = 0;
		//rb.isKinematic = false;
		//GetComponent<CircleCollider2D>().enabled = true;
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
		InvulnerabilityCooldown(1.0f, (EnemyDamageTypes.MELEE | EnemyDamageTypes.RANGED));
	}

}
