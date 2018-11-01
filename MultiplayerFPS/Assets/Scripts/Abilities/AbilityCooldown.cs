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

    [SerializeField]
    private Ability ability;

    private AudioClip abilityClip;

    void Start()
    {
        Initialize(ability);
    }

    void Initialize(Ability selectedAbility)
    {
        ability = selectedAbility;
        abilityCooldown = ability.cooldown;
        //abilityImage.sprite = ability.abilityIcon;
        //darkMask.sprite = ability.abilityIcon;
        ability.Initialize(gameObject);
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
        //darkMask.enabled = false;
        //cooldownText.enabled = false;
    }

    void Cooldown()
    {
        timeLeftCooldown -= Time.deltaTime;
        Mathf.Round(timeLeftCooldown);
        //cooldownText.text = timeLeftCooldown.ToString();
        //darkMask.fillAmount = (timeLeftCooldown / abilityCooldown);
    }

    void AbilityTriggered()
    {
        nextCooldownTime = Time.time + abilityCooldown;
        timeLeftCooldown = abilityCooldown;

        //play our abilityClip sound

        //darkMask.enabled = true;
        //cooldownText.enabled = true;

        ability.ActivateAbility();
    }
}
