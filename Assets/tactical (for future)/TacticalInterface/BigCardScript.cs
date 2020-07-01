using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BigCardScript : MonoBehaviour, IPointerClickHandler
{
    private CardPivotScript cardPivot;
    public PlayerController player;
    private void Awake()
    {
        cardPivot = transform.parent.Find("CardsPivotParent").GetComponent<CardPivotScript>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        Debug.Log(cardPivot);
        Debug.Log(cardPivot.currentCard.Find("Card").GetComponent<Card>());
        //cardPivot.currentCard.gameObject.GetComponent<Card>().GetCardValues();
        cardPivot.attackTargetChoose = true;
        cardPivot.currentCard.Find("Card").GetComponent<Image>().sprite = null;
        GetComponent<RectTransform>().DOScaleY(.1f, .1f);
        GetComponent<Image>().DOFade(.1f, .1f)
            .OnComplete(() => {

                gameObject.SetActive(false);
                player.ToggleAttackRadius();
                
            });

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
