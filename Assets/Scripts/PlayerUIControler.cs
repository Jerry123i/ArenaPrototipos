using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIControler : MonoBehaviour {

	public GameObject healthBar;
	public GameObject healthDot;

	public GameObject cdDodge;
	public GameObject cdShield;
	public GameObject SpecialBar;

	public float dodgeClock = 0;
	public float shieldClock = 0;

	private void Awake()
	{
		SpecialBar = GameObject.Find("SpecialCD");
		SpecialBar.SetActive(false);
	}
	
	public void UpdateHealth(int hp)
	{
		int currentHp = healthBar.transform.childCount;

		if(hp == currentHp)
		{
			return;
		}

		if(currentHp < hp)
		{
			for(int i = 0; i < hp - currentHp ; i++)
			{
				Instantiate(healthDot, healthBar.transform);
			}
		}

		if(currentHp > hp)
		{
			for (int i = 0; i < currentHp - hp; i++)
			{
				Destroy(healthBar.transform.GetChild(0).gameObject);
			}
		}

	}

	public IEnumerator StartDodgeClock(float maxTime)
	{
		cdDodge.SetActive(true);
		cdDodge.GetComponent<Image>().fillAmount = 0;

		do
		{
			dodgeClock += Time.deltaTime;
			cdDodge.GetComponent<Image>().fillAmount = dodgeClock / maxTime;
			yield return null;

		} while (dodgeClock < maxTime);

		cdDodge.SetActive(false);
		dodgeClock = 0;
	}

	public IEnumerator StartShieldClock(float maxTime)
	{
		cdShield.SetActive(true);
		cdShield.GetComponent<Image>().fillAmount = 0;

		do
		{
			shieldClock += Time.deltaTime;
			cdShield.GetComponent<Image>().fillAmount = shieldClock / maxTime;
			yield return null;

		} while (shieldClock < maxTime);

		cdShield.SetActive(false);
		shieldClock = 0;
	}

	public IEnumerator SpecialCooldownBarController(float cd)
	{
		SpecialBar.SetActive(true);
		SpecialBar.GetComponent<Image>().fillAmount = 0;
		var startTime = 0f;
        
		do
		{
			startTime += Time.deltaTime;
			SpecialBar.GetComponent<Image>().fillAmount = startTime / cd;
			yield return null;

		} while (startTime < cd);

		SpecialBar.SetActive(false);
	}

}
