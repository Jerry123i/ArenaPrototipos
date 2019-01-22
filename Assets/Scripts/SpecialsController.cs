using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpecialsController : MonoBehaviour
{
    // generic stuff
    public enum Specials { Kratos, ReflectShield, SplitShot, SpearBounce}

    private enum SpecialsCooldowns
    {
        KratosCd = 5,
        ReflectShieldCd = 5,
        SplitShotCd = 5,
        SpearBounceCd = 5
    }
    
    public bool SpecialOnCd;
    private PlayerUIControler _uIControler;

    public bool SpecialReady;
    private GameObject _specialParticle;
    
    // kratos
    public bool HasKratos;
    
    // reflect shield
    public bool HasReflectShield;
    
    // split shot
    [FormerlySerializedAs("SplitShot")] [FormerlySerializedAs("_splitShot")] public bool HasSplitShot;
    [FormerlySerializedAs("_splitShotOffsset")] public float SplitShotOffsset = 15f;

    // spear bounce
    public bool HasSpearBounce = true;


    private void Awake()
    {
        HasKratos = false;
        HasReflectShield = false;
        HasSpearBounce = false;
        HasSplitShot = false;

        _specialParticle = GameObject.Find("Particle System (Special)");
        _specialParticle.SetActive(false);
    }

    private void Start()
    {
        _uIControler = GetComponent<PlayerUIControler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateSpecial(Specials.Kratos);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateSpecial(Specials.ReflectShield);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateSpecial(Specials.SplitShot);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateSpecial(Specials.SpearBounce);
        }


        if (SpecialReady)
        {
            _specialParticle.SetActive(true);
        }
        else
        {
            _specialParticle.SetActive(false);
        }
        
        if (SpecialOnCd)
        {
            SpecialReady = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!SpecialOnCd)
            {
                SpecialReady = true;
            }
        }
    }

    public IEnumerator SpecialCooldown(Specials special)
    {
        switch (special)
        {
            case Specials.Kratos:
                var c = GameObject.FindGameObjectWithTag("Spear").GetComponent<SpearScript>();
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.KratosCd));
                HasKratos = false;
                SpecialOnCd = true;
                c.RotateToTarget(transform);
                c.Moving = true;
                c.catchable = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.KratosCd);
                HasKratos = true;
                SpecialOnCd = false;
                break;
            case Specials.ReflectShield:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.ReflectShieldCd));
                yield return new WaitForSeconds(0.5f); // little delay so you can deflect multiple projectiles
                HasReflectShield = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.ReflectShieldCd);
                HasReflectShield = true;
                SpecialOnCd = false;
                break;
            case Specials.SplitShot:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.SplitShotCd));
                HasSplitShot = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.SplitShotCd);
                HasSplitShot = true;
                SpecialOnCd = false;
                break;
            case Specials.SpearBounce:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.SpearBounceCd));
                HasSpearBounce = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.SpearBounceCd);
                HasSpearBounce = true;
                SpecialOnCd = false;
                break;
        }
    }

    private void ActivateSpecial(Specials special)
    {
        var specialText = GameObject.Find("SpecialText").GetComponent<TextMeshProUGUI>();
        switch (special)
        {
            case Specials.Kratos:
                HasKratos = true;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = false;
                specialText.text = "Kratos";
                break;
            case Specials.ReflectShield:
                HasKratos = false;
                HasReflectShield = true;
                HasSplitShot = false;
                HasSpearBounce = false;
                specialText.text = "Reflect Shield";
                break;
            case Specials.SplitShot:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = true;
                HasSpearBounce = false;
                specialText.text = "Split Shot";
                break;
            case Specials.SpearBounce:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = true;
                specialText.text = "Spear Bounce";
                break;
        }
    }
}
