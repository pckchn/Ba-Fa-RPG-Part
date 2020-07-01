using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBAR : MonoBehaviour
{
    public Transform hpCurrent, pointer;
    public Image hpBite;
    public float MaxHP = 10;
    [HideInInspector] public float currentHP;
    // Start is called before the first frame update
    private void Awake()
    {
        currentHP = MaxHP;
    }
    /*private void Start()
    {
        //debug
        takeDamage(5);
    }*/
    public void FullHP(int newMaxHP)
    {
        MaxHP = newMaxHP;
        currentHP = MaxHP;
        hpCurrent.localScale = new Vector3(currentHP / MaxHP /*- .05f*/, 1, 1);
    }
    public void takeDamage(int Damage)
    {
        var duration = .3f;
        currentHP -= Damage;
        Debug.Log(currentHP / MaxHP);
        hpCurrent.localScale = new Vector3(currentHP /MaxHP /*- .05f*/, 1 , 1);
        hpBite.transform.localScale = new Vector3(Damage / MaxHP, 1, 1);
        hpBite.transform.parent.position = pointer.position;
        hpBite.color = new Color(hpBite.color.r, hpBite.color.g, hpBite.color.b, 1);
        hpBite.gameObject.SetActive(true);
        hpBite.DOFade(0, duration)
            .OnComplete(()=> hpBite.gameObject.SetActive(false));
        hpBite.transform.parent.DOLocalMoveY(hpBite.transform.localPosition.y - 100, duration);
        //hpBite.parent.localPosition = new Vector3(hpBite.localPosition.x - (38f * (10 - currentHP)), hpBite.parent.localPosition.y, hpBite.parent.localPosition.z);
    }
}
