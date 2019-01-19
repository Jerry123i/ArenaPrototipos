using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HypeMetter : MonoBehaviour {

	[Header("Point Calculation")]
	public float maxHype = 180;
	public float v;
	public float n;
	public float dropRate;

	[Header("Tiers")]
	public float tier1Value;
	public Image tier1;
	public float tier2Value;
	public Image tier2;


	private int kills = 0;
	private int hits = 0;
	private List<object> killList;
	
	public Image hypeBar;

	private float currentHype = 0;

	public float CurrentHype
	{
		get
		{
			return currentHype;
		}

		set
		{
			currentHype = value;

			if(currentHype > maxHype)
			{
				currentHype = maxHype;
			}
			if(currentHype < 0)
			{
				currentHype = 0;
			}

			hypeBar.fillAmount = currentHype / maxHype;
		}
	}

	public static HypeMetter instance;

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

		SetMarkers();
	}

	private void Update()
	{
		CurrentHype -= Time.deltaTime * dropRate * maxHype;
	}

	private void SetMarkers()
	{
		tier1.rectTransform.anchorMin = new Vector2(0, (tier1Value / maxHype) - 0.005f);
		tier1.rectTransform.anchorMax = new Vector2(1, (tier1Value / maxHype) + 0.005f);

		tier2.rectTransform.anchorMax = new Vector2(1, (tier2Value / maxHype) + 0.005f);
		tier2.rectTransform.anchorMin = new Vector2(0, (tier2Value / maxHype) - 0.005f);

	}

	public void FinishCombo()
	{
		hits -= kills;

		//Metodo 2 (Kills enquanto a lanca voam contam)
		//foreach(object value in killlist)
		//{
		//	if(value == null)
		//	{
		//		kills++;
		//	}
		//}
		
		string result = hits.ToString() + "/" + kills.ToString();

		Debug.Log("CalculatePoints(result) = " + CalculatePoints(result).ToString());

		CurrentHype += CalculatePoints(result);

		hits = 0;
		kills = 0;
		killList.Clear();

	}

	float CalculatePoints(string values)
	{

		Debug.Log("values = " + values);

		switch (values)
		{
			case "0/0":
				return 0;
			case "1/0":
				return 0;
			case "0/1":
				return n;
			case "2/0":
				return n;
			case "1/1":
				return n * 2;
			case "0/2":
				return (v / 6 );
			case "3/0":
				return n * 2;
			case "2/1":
				return (v / 6);
			case "1/2":
				return (v / 6 + n);
			case "0/3":
				return v / 2;
			case "4/0":
				return 4 * n;
			case "3/1":
				return (v / 2.5f);
			case "2/2":
				return v / 2;
			case "1/3":
				return ((v / 2) + (3 * n));
			case "0/4":
				return v;
			default:

				string[] data = values.Split('/');
				//int hits = int.Parse(data[0]);
				int kills = int.Parse(data[1]);

				return (v + ((kills - 5) * n));

				
		}
	}

	public void Notify(object value, NotificationType notification)
	{
		switch (notification)
		{
			case NotificationType.ENEMY_KILLED:
				kills++;
				break;
			case NotificationType.ENEMY_HIT:
				hits++;
				killList.Add(value);
				break;
			case NotificationType.PLAYER_TOOK_DAMAGE:
				break;
			case NotificationType.PLAYER_SPEAR:
				killList = new List<object>();
				break;
			case NotificationType.SPEAR_WALL:
				FinishCombo();
				break;
		}
	}

}