using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour {

    public KeyCode abilityKey;

    public float abilityCooldown;
    public float nextCooldownTime;
    public float timeLeftCooldown;

    public Image abilityImage;
    public Image darkMask;
    public Text cooldownText;

    public Ability ability;

    private AudioClip abilityClip;

    void Setup()
    {
        Initialize(ability);
    }

    void Initialize(Ability selectedAbility)
    {
        ability = selectedAbility;
        abilityCooldown = ability.cooldown;
        ability.Initialize(gameObject);
    }

    public void SetupUI()
    {
        abilityImage.sprite = ability.abilityIcon;
        darkMask.sprite = ability.abilityIcon;
    }

    void Update()
    {
        if (Time.time >= nextCooldownTime)
        {
            ReadyAbility();
            if (Input.GetButtonDown(abilityKey.ToString()))
            {
                AbilityTriggered();
            }
        }
        else
        {
            Cooldown();
        }
    }

    void ReadyAbility()
    {
        if (darkMask != null)
        {
            darkMask.enabled = false;
        }
        if (cooldownText != null)
        {
            cooldownText.enabled = false;
        }
    }

    void Cooldown()
    {
        timeLeftCooldown -= Time.deltaTime;

        double cooldownRounded = Mathf.Floor(timeLeftCooldown * 10.0f) / 10.0f;
        
        if (cooldownText != null)
        {
            cooldownText.text = cooldownRounded.ToString();
        }
        if (darkMask != null)
        {
            darkMask.fillAmount = (timeLeftCooldown / abilityCooldown);
        }
    }

    void AbilityTriggered()
    {
        nextCooldownTime = Time.time + abilityCooldown;
        timeLeftCooldown = abilityCooldown;

        //play our abilityClip sound

        if (darkMask != null)
        {
            darkMask.enabled = true;
        }
        if (cooldownText != null)
        {
            cooldownText.enabled = true;
        }

        ability.ActivateAbility();
    }
}
