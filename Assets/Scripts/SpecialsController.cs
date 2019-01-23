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

    private float _charge;
    private readonly float _maxCharge = 1.5f;
    
    // kratos
    public bool HasKratos;
    
    // reflect shield
    public bool HasReflectShield;
    
    // split shot
    [FormerlySerializedAs("SplitShot")] [FormerlySerializedAs("_splitShot")] public bool HasSplitShot;
    [FormerlySerializedAs("_splitShotOffsset")] public float SplitShotOffsset = 15f;

    // spear bounce
    public bool HasSpearBounce = true;
    public int TotalBounces = 3;


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
        _charge = 0;
    }

    private void Update()
    {
        if (HasSpearBounce || HasSplitShot)
        {
            ChargeSpecial();
        }
        
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


        _specialParticle.SetActive(_charge > 0 && GetComponent<PlayerScript>().HasSpear);
        var emissionModule = _specialParticle.GetComponent<ParticleSystem>().emission;
        emissionModule.rateOverTime = Mathf.Pow(17, _charge);

        if (SpecialOnCd)
        {
            SpecialReady = false;
        }

//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            if (!SpecialOnCd)
//            {
//                SpecialReady = true;
//            }
//        }
    }

    private void ChargeSpecial()
    {
        if (_charge >= _maxCharge && GetComponent<PlayerScript>().HasSpear)
        {
            SpecialReady = true;
        }
        if (Input.GetButtonUp("Fire2") && _charge >= _maxCharge)
        {
            StartCoroutine(test());
        }
        if (Input.GetButton("Fire2") && _charge < _maxCharge && !SpecialOnCd)
        {
            _charge += Time.deltaTime;
            SpecialReady = false;
        }

        if (Input.GetButtonUp("Fire2") && _charge < _maxCharge)
        {
            _charge = 0;
            SpecialReady = false;
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
                var c2 = GameObject.FindGameObjectWithTag("Spear").GetComponent<SpearScript>();
                c2.Moving = false;
                yield return new WaitForSeconds((float)SpecialsCooldowns.SpearBounceCd);
                HasSpearBounce = true;
                SpecialOnCd = false;
                TotalBounces = 3;
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

    IEnumerator test()
    {
        yield return new WaitForSeconds(0.2f);
        _charge = 0;
    }
}
