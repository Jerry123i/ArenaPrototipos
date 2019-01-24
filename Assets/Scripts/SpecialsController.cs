using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpecialsController : MonoBehaviour
{
    // generic stuff
    public enum Specials { Kratos, ReflectShield, SplitShot, SpearBounce, ShieldCharge, FastSpear, RotatingSpear}

    private enum SpecialsCooldowns
    {
        KratosCd = 2,
        ReflectShieldCd = 2,
        SplitShotCd = 2,
        SpearBounceCd = 2,
        ShieldChargeCd = 2,
        FastSpearCd = 2,
        RotatingSpearCd = 2
    }
    
    public bool SpecialOnCd;
    private PlayerUIControler _uIControler;

    public bool SpecialReady;
    private GameObject _specialParticle;

    [FormerlySerializedAs("_charge")] public float Charge;
    private readonly float _maxCharge = 1.5f;

    private bool _choseSpecial;
    
    // kratos
    public bool HasKratos;
    
    // reflect shield
    public bool HasReflectShield;
    
    // split shot
    [FormerlySerializedAs("SplitShot")] [FormerlySerializedAs("_splitShot")] public bool HasSplitShot;
    [FormerlySerializedAs("_splitShotOffsset")] public float SplitShotOffsset = 15f;

    // spear bounce
    public bool HasSpearBounce;
    public int TotalBounces = 3;
    
    // shield charge
    public bool HasShieldCharge;
    
    // fast spear
    public bool HasFastSpear;
    public float FastSpeedMultiplier;
    public int FastDamageMultiplier;
    
    // Rotating spear
    public bool HasRotatingSpear;


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
        Charge = 0;
        FastSpeedMultiplier = 3;
        FastDamageMultiplier = 2;
    }

    private void Update()
    {
        //if(SpecialReady) Debug.Log(SpecialReady);
        if ((HasSpearBounce || HasSplitShot || HasFastSpear || HasRotatingSpear) && GetComponent<PlayerScript>().HasSpear)
        {
            ChargeSpecial();
        }

        if (HasShieldCharge)
        {
            ChargeSpecial();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && !_choseSpecial)
        {
            ActivateSpecial(Specials.Kratos);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !_choseSpecial)
        {
            ActivateSpecial(Specials.ReflectShield);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !_choseSpecial)
        {
            ActivateSpecial(Specials.SplitShot);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !_choseSpecial)
        {
            ActivateSpecial(Specials.SpearBounce);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && !_choseSpecial)
        {
            ActivateSpecial(Specials.ShieldCharge);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && !_choseSpecial)
        {
            ActivateSpecial(Specials.FastSpear);
            _choseSpecial = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && !_choseSpecial)
        {
            ActivateSpecial(Specials.RotatingSpear);
            _choseSpecial = true;
        }

        if (!HasShieldCharge)
        {
            _specialParticle.SetActive(Charge > 0 && GetComponent<PlayerScript>().HasSpear);
        }
        else
        {
            _specialParticle.SetActive(Charge > 0);
        }
        
        var emissionModule = _specialParticle.GetComponent<ParticleSystem>().emission;
        emissionModule.rateOverTime = Mathf.Pow(17, Charge);

        if (SpecialOnCd)
        {
            SpecialReady = false;
        }
    }

    private void ChargeSpecial()
    {
        //Debug.Log(_charge);
        // Here the special is ready to be used when button is lifted up
        if (Charge >= _maxCharge && GetComponent<PlayerScript>().HasSpear && !HasShieldCharge)
        {
            SpecialReady = true;
        }
        if (Charge >= _maxCharge && HasShieldCharge)
        {
            SpecialReady = true;
        }
        
        // Little delay so the special can be used before we set Special Ready to false
        if (Input.GetButtonUp("Fire2") && Charge >= _maxCharge && !HasShieldCharge)
        {
            StartCoroutine(LittleDelay(0.2f));
        }
        
        if (Input.GetButton("Fire2") && Charge < _maxCharge && !SpecialOnCd && !HasShieldCharge)
        {
            Charge += Time.deltaTime;
            SpecialReady = false;
        }

        if (Input.GetButton("Fire1") && Charge < _maxCharge && !SpecialOnCd && HasShieldCharge)
        {
            Charge += Time.deltaTime;
            SpecialReady = false;
        }
        if (Input.GetButtonUp("Fire2") && Charge < _maxCharge && !HasShieldCharge)
        {
            Charge = 0;
            SpecialReady = false;
        }
        if (Input.GetButtonUp("Fire1") && Charge < _maxCharge && HasShieldCharge)
        {
            Charge = 0;
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
                c.Catchable = true;
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
            case Specials.ShieldCharge:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.ShieldChargeCd));
                HasShieldCharge = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.ShieldChargeCd);
                HasShieldCharge = true;
                SpecialOnCd = false;
                break;
            case Specials.FastSpear:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.FastSpearCd));
                HasFastSpear = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.FastSpearCd);
                HasFastSpear = true;
                SpecialOnCd = false;
                break;
            case Specials.RotatingSpear:
                StartCoroutine(_uIControler.SpecialCooldownBarController((float) SpecialsCooldowns.RotatingSpearCd));
                HasRotatingSpear = false;
                SpecialOnCd = true;
                yield return new WaitForSeconds((float)SpecialsCooldowns.RotatingSpearCd);
                HasRotatingSpear = true;
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
                HasShieldCharge = false;
                HasFastSpear = false;
                HasRotatingSpear = false;
                specialText.text = "Kratos";
                break;
            case Specials.ReflectShield:
                HasKratos = false;
                HasReflectShield = true;
                HasSplitShot = false;
                HasSpearBounce = false;
                HasShieldCharge = false;
                HasFastSpear = false;
                HasRotatingSpear = false;
                specialText.text = "Reflect Shield";
                break;
            case Specials.SplitShot:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = true;
                HasSpearBounce = false;
                HasShieldCharge = false;
                HasFastSpear = false;
                HasRotatingSpear = false;
                specialText.text = "Split Shot";
                break;
            case Specials.SpearBounce:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = true;
                HasShieldCharge = false;
                HasFastSpear = false;
                HasRotatingSpear = false;
                specialText.text = "Spear Bounce";
                break;
            case Specials.ShieldCharge:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = false;
                HasShieldCharge = true;
                HasFastSpear = false;
                HasRotatingSpear = false;
                specialText.text = "Shield Charge";
                break;
            case Specials.FastSpear:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = false;
                HasShieldCharge = false;
                HasFastSpear = true;
                HasRotatingSpear = false;
                specialText.text = "Fast Spear";
                break;
            case Specials.RotatingSpear:
                HasKratos = false;
                HasReflectShield = false;
                HasSplitShot = false;
                HasSpearBounce = false;
                HasShieldCharge = false;
                HasFastSpear = false;
                HasRotatingSpear = true;
                specialText.text = "Rotating Spear";
                break;
        }
    }

    public void FastSpearLeaveCd()
    {
        StartCoroutine(SpecialCooldown(Specials.FastSpear));
    }

    private IEnumerator LittleDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Charge = 0;
    }
}
