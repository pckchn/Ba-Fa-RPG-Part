using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHPBar : HPBAR
{
    private HPBarLoc hpbarloc;
    public Camera cam;
    private Image hpstripe;
    public float fadeDuration;
    public simpleTurnSystem turnSystem;
    // Start is called before the first frame update
    private void Awake()
    {
        currentHP = MaxHP;
        hpstripe = hpCurrent.GetComponent<Image>();
        newEnemy(turnSystem.enemies[0]);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*transform.position = new Vector2(cam.WorldToScreenPoint(hpbarloc.location).x, 
            cam.WorldToScreenPoint(hpbarloc.location).y + hpbarloc.Offset2d);*/
    }
    public void ToggleBar()
    {
        if (hpstripe.gameObject.activeSelf)
        {
            hpstripe.DOFade(0, fadeDuration)
                .OnComplete(()=> hpstripe.gameObject.SetActive(false));
        }
        else
        {
            hpstripe.gameObject.SetActive(true);
            hpstripe.DOFade(1, fadeDuration);
        }
    }
    public void newEnemy(EnemyScript enemy)
    {
        hpbarloc = enemy.gameObject.GetComponentInChildren<HPBarLoc>();
    }
}
