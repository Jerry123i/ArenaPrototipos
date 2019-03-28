using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialsMenuManager : MonoBehaviour
{
	public GameObject menuStuff;

	public SpawnerControler spawnerControler;
	public ArenaControler arenaControler;
	
	[Header("Lança")]
	public List<Toggle> lancaToggles;

	[Header("Escudo")]
	public List<Toggle> escudoToggles;

	[Header("Mao")]
	public List<Toggle> maoToggles;

	private void Start()
	{
		spawnerControler.enabled = false;
		arenaControler.enabled = false;
	}

	private void Update()
	{
		bool l = false;
		foreach(Toggle t in lancaToggles)
		{
			if(t.isOn && l)
			{
				t.isOn = false;
			}
			if(t.isOn && !l)
			{
				l = true;
			}
		}

		l = false;
		foreach (Toggle t in escudoToggles)
		{
			if (t.isOn && l)
			{
				t.isOn = false;
			}
			if (t.isOn && !l)
			{
				l = true;
			}
		}

		l = false;
		foreach (Toggle t in maoToggles)
		{
			if (t.isOn && l)
			{
				t.isOn = false;
			}
			if (t.isOn && !l)
			{
				l = true;
			}
		}
	}

	public void PassInfo()
	{
		SpecialsController sc = FindObjectOfType<PlayerScript>()._specialsController;

		sc.HasSplitShot = lancaToggles[0].isOn;
		sc.HasSpearBounce = lancaToggles[1].isOn;
		sc.HasFastSpear = lancaToggles[2].isOn;

		sc.HasReflectShield = escudoToggles[0].isOn;
		sc.HasShieldCharge = escudoToggles[1].isOn;

		sc.HasKratos = maoToggles[0].isOn;
	}

	public void Open()
	{
		menuStuff.SetActive(true);			
	}

	public void Close()
	{
		PassInfo();
		menuStuff.SetActive(false);

		spawnerControler.enabled = true;
		arenaControler.enabled = true;

	}
}
