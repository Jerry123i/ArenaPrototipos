using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIControler : MonoBehaviour {

	public GameObject healthBar;
	public GameObject healthDot;

	public GameObject cdDodge;
	public GameObject cdShield;

	public float dodgeClock = 0;
	public float shieldClock = 0;

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



}
