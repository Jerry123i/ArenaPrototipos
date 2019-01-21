using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum NotificationType { ENEMY_KILLED, ENEMY_HIT, PLAYER_TOOK_DAMAGE, PLAYER_SPEAR, SPEAR_WALL, SPEAR_KRATOS_PICKUP, TAUNT}

public class ImpatianceMetter : MonoBehaviour
{
	public static ImpatianceMetter instance;

	public TextMeshProUGUI rateText;
	public Image bar;

	
	private float _impatianceRate = 1;
	public float ImpatianceRate
	{
		get
		{
			return _impatianceRate;
		}

		set
		{
			_impatianceRate = value;

			if(_impatianceRate < 0)
			{
				_impatianceRate = 0;
			}
			if(_impatianceRate > 5.0f)
			{
				_impatianceRate = 5.0f;
			}

			rateText.text = "x" + _impatianceRate.ToString();
		}
	}

	private float max = 15;

	
	private float _impatiance;
	public float Impatiance
	{
		get
		{
			return _impatiance;
		}

		set
		{
			_impatiance = value;

			if(_impatiance < 0)
			{
				_impatiance = 0;
			}

			if(_impatiance >= max)
			{
				MaxImpatiance();
			}

			bar.fillAmount = _impatiance / max;

		}
	}

	private float timeWoutHitting = 0;
	public float limitTimeWoutHitting = 4.0f;

	private void Awake()
	{
		if(instance != null)
		{
			if(instance != this)
			{
				Destroy(this);
			}
		}
		else
		{
			instance = this;
		}
	}

	private void Update()
	{
		Impatiance += Time.deltaTime * ImpatianceRate;

		timeWoutHitting += Time.deltaTime;
		if (timeWoutHitting >= limitTimeWoutHitting)
		{
			ImpatianceRate *= 1.2f;
			timeWoutHitting = 0;
		}
	}

	public void Notify(object value, NotificationType notification)
	{
		switch (notification)
		{
			case NotificationType.ENEMY_KILLED:

				switch (SpawnerControler.NumberOfEnemies())
				{
					case 1:
						CallWave();
						break;
					case 2:
						ImpatianceRate *= 2f;
						break;
				}

				break;
			case NotificationType.ENEMY_HIT:
					timeWoutHitting = 0;
				break;
			case NotificationType.PLAYER_TOOK_DAMAGE:
				Impatiance -= max * 0.15f;
				break;

			case NotificationType.TAUNT:
				Taunt();
				break;
		}

	}

	private void MaxImpatiance()
	{
		CallWave();
	}

	private void CallWave()
	{
		SpawnerControler.instance.SpawnWave();
		ImpatianceRate = 1;
		Impatiance = 0;
	}

	private void Taunt()
	{
		CallWave();
	}

}