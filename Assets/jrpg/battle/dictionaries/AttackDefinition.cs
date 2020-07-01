using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Attack", menuName = "Attack")]
public class AttackDefinition : ScriptableObject
{
    public string Name;
    public float manaCost;
    public int damage;
    public float critChance;
    public bool comboFriendly;

    public Sprite card;
    public Sprite icon;
}
