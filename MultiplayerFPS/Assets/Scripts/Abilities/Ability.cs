using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{

    public string name = "Ability";
    public Sprite abilityIcon;
    public float cooldown = 0f;

    public abstract void Initialize(GameObject obj);
    public abstract void ActivateAbility();
}
