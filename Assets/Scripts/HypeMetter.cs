using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HypeMetter : MonoBehaviour {

	public Image hypeBar;

	public float maxHype = 180;
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
			hypeBar.fillAmount = currentHype / maxHype;
		}
	}

	public static HypeMetter instance;

	public int comboTracker;

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

	public void FinishCombo()
	{
		if(comboTracker <= 2)
		{
			CurrentHype += comboTracker * 20;
		}

		else if(comboTracker >= 3)
		{
			CurrentHype += comboTracker * 30;

			CurrentHype += (comboTracker - 3) * 20;

		}

		comboTracker = 0;
	}

}
