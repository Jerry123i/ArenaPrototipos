using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpearScript : MonoBehaviour {

	[FormerlySerializedAs("speed")] public float Speed;
	[FormerlySerializedAs("damage")] public int Damage;

	private bool _moving;
	[FormerlySerializedAs("catchable")] public bool Catchable;

	private Collider2D _col;

	private SpecialsController _specialsController;

	public bool Moving
	{
		get
		{
			return _moving;
		}

		set
		{
			_moving = value;
			
		}
	}

	public bool Rotating;
	
	private void Awake()
	{
		_col = GetComponent<Collider2D>();
		Moving = true;
	}

	private void Start()
	{
		_specialsController = GameObject.Find("Player").GetComponent<SpecialsController>();

		// fast spear behaviour handler
		if (_specialsController.HasFastSpear && !_specialsController.SpecialOnCd && _specialsController.SpecialReady)
		{
			Damage = Damage * _specialsController.FastDamageMultiplier;	
		}
		
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Mouse2))
		{
			Debug.Log("Special ready: " + _specialsController.SpecialReady);
			Debug.Log("Special on CD: " + _specialsController.SpecialOnCd);	
		}
		
	}

	private void FixedUpdate()
	{
		// Fast spear behaviour handler
		if (Moving && _specialsController.HasFastSpear && !_specialsController.SpecialOnCd && _specialsController.SpecialReady)
		{
			transform.Translate(Vector3.up * Speed * _specialsController.FastSpeedMultiplier);
		}
		else if (Moving)
		{
			transform.Translate(Vector3.up * Speed);
		}
		// split shot behaviour handler
		else if (CompareTag("SpearClone") && !_moving)
		{
			Destroy(gameObject);
		}
		else if (Rotating)
		{
			Debug.Log("RODANDO");
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if((collision.CompareTag("Player") && !Moving ) | (collision.CompareTag("Player") && Moving && Catchable))
		{
			PlayerPickup(collision);
		}

		if (collision.gameObject.CompareTag("Wall"))
		{
			WallHit();
		}

		if(collision.gameObject.CompareTag("Enemy"))
		{
			EnemyHit(collision);
		}

	}

	public virtual void PlayerPickup(Collider2D collision)
	{
		var playerScript = collision.GetComponent<PlayerScript>();
		var specialsController = collision.GetComponent<SpecialsController>();
		playerScript.HasSpear = true;

		if (specialsController.HasKratos && Moving)
		{
			HypeMetter.instance.Notify(this, NotificationType.SPEAR_KRATOS_PICKUP);
		}

		Destroy(gameObject, 0.05f);
		
	}

	public virtual void WallHit()
	{
		
		if (!_specialsController.HasSpearBounce)
		{
			Moving = false;
			//HypeMetter.instance.FinishCombo();
		}
		
		// Fast spear cooldown counting
		if (_specialsController.HasFastSpear && !_specialsController.SpecialOnCd && Damage == 2) // GAMBIARRA
		{
			GameObject.Find("Player").GetComponent<SpecialsController>().FastSpearLeaveCd();
		}
		
		else if (_specialsController.HasSpearBounce && _specialsController.TotalBounces > 0 && !_specialsController.SpecialOnCd && _specialsController.SpecialReady) // wall hit when you have spear bounce on and bounces left
		{
			RaycastHit2D hit;
			Vector3 up;
			Transform tipOfLance;
			ShootRayFromTip(out hit, out up, out tipOfLance);
			Debug.Log("hit name: " + hit.transform.name);
			Debug.Log("hit point: " + hit.point);
			Debug.Log("hit normal: " + hit.normal);
			Debug.Log("rotação entrada: " + transform.eulerAngles.z);

			if (!hit.transform.gameObject.CompareTag("Wall")) return;
			var reflectDir = Vector2.Reflect(up, hit.normal);
			var rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg + 180;
			Debug.Log("reflect direction = " + reflectDir);
			Debug.Log("rotação de saida = " + rot);

			Debug.DrawRay(hit.point, new Vector3(reflectDir.y, reflectDir.x, 0) * 5, Color.green, 3f);
			Debug.DrawRay(hit.point, hit.normal, Color.black, 3f);
			
			
			
			transform.eulerAngles = new Vector3(0, 0, rot);
			
			_specialsController.TotalBounces--;
//			Debug.Log(_specialsController.TotalBounces);
		}
		else if (_specialsController.TotalBounces == 0)
		{
			StartCoroutine(_specialsController.SpecialCooldown(SpecialsController.Specials.SpearBounce));
		}
		else
		{
			Moving = false;
			//HypeMetter.instance.FinishCombo();
		}
		
		HypeMetter.instance.Notify(this, NotificationType.SPEAR_WALL);
	}

	public virtual void EnemyHit(Collider2D collision)
	{

	}

	public void RotateToTarget(Transform target)
	{
		var vectorToTarget = target.position - transform.position;
		//var angleBetween = Vector3.Angle(transform.forward, vectorToTarget);
		var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
		transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
	}

	private void ShootRayFromTip(out RaycastHit2D hit, out Vector3 up, out Transform tipOfLance)
	{
		tipOfLance = GameObject.Find("Tip").transform;
		up = tipOfLance.TransformDirection(Vector2.up);
		Debug.DrawRay(tipOfLance.position, up, Color.white, 2f);
		hit = Physics2D.Raycast(tipOfLance.position, up);
	}

}
