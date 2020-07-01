using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerController;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int radius;
    public float damage;
    public Sprite icon;
    public Sprite cardImage;
    public attackElement element;

    [HideInInspector] public bool isUsed;
    [HideInInspector] public int attackIndex;

    [HideInInspector] public bool activeCard = false;
    private CardPivotScript parent;
    public GameObject arrow;
    public Walkable targetTile;

    public simplePlayerScript player;
    public AttackDefinition attack;
    //[HideInInspector] public bool showBigCard = false;
    public void GetCardValues()
    {

    }
    private void Awake()
    {
        parent = transform.parent.parent.GetComponent<CardPivotScript>();
        GetComponent<Image>().sprite = attack.card;

    }
    public void SetLocalPositions(Vector2 parentPos, Vector2 selfPos, Vector2 bigCardPos)
    {
        transform.parent.GetComponent<RectTransform>().localPosition = parentPos;
        GetComponent<RectTransform>().localPosition = selfPos;
        transform.parent.Find("BigCard").GetComponent<RectTransform>().localPosition = bigCardPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed)
        {
            parent.GetComponent<CardPivotScript>().CancelAttack(this);
            GetComponent<Image>().sprite = icon;
            isUsed = false;

        }
        else
        {
            if (activeCard && player.atk(attack))
            {
                //parent.ToggleBigCard();


                //throw new System.NotImplementedException();
                //Debug.Log(showBigCard);

                //player.atk(attack);
                gameObject.SetActive(false);
            }
        }
    }
}