using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPivotScript : MonoBehaviour
{
    List<Transform> Children = new List<Transform>();
    public Transform imageTr;
    public List<Image> images = new List<Image>();
    [HideInInspector]public Transform currentCard;
    RectTransform selfTransform;
    Transform _canvas;
    public bool activeMenu;
    public GameObject BigCard;
    private Image bigCardImage;
    private Text BigCardText;
    public bool ShowBigCard;
    [HideInInspector]public bool attackTargetChoose;
    public PlayerController player;
    // Start is called before the first frame update
    private void Awake()
    {
        bigCardImage = BigCard.GetComponent<Image>();
        BigCardText = BigCard.transform.Find("Text").GetComponent<Text>();

        selfTransform = GetComponent<RectTransform>();
        _canvas = transform.parent;
        foreach (Transform child in transform)
        {
            Children.Add(child);
        }
        foreach (Transform child in imageTr)
        {
            images.Add(child.GetComponent<Image>());
        }
    }
    void Start()
    {
        //ShowCards();
 
    }

    // Update is called once per frame
    void Update()
    {
        if (activeMenu)
        {
            /*Debug.Log(Children.Find(x => Mathf.Abs(x.rotation.eulerAngles.z) > selfTransform.localRotation.eulerAngles.z - 12
                && Mathf.Abs(x.rotation.eulerAngles.z) < selfTransform.localRotation.eulerAngles.z + 12));*/
            //Children.Find(x => x.position)
            //Debug.Log(Vector3.Dot(Vector2.up, Children[1].up));
            //Debug.Log(Children[1]);
            if (currentCard != null /*&& BigCard.activeSelf*/)
                currentCard.GetChild(0).transform.localPosition = Vector3.Lerp(new Vector3(0, 268.8f, 0), new Vector3(0,337,0), Mathf.InverseLerp(.97f, 1, Vector3.Dot(Vector2.up, currentCard.up)));
                //bigCardImage.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, Mathf.InverseLerp(.97f, 1, Vector3.Dot(Vector2.up, currentCard.up))));
            if (currentCard != Children.Find(x=> Vector3.Dot(Vector2.up, x.up) > .97f))
            {

                if (currentCard != null)
                {
                    //bigCardImage.DOFade(0, .1f);
                    currentCard.Find("Card").GetComponent<Card>().activeCard = false;
                }
                //currentCard.Find("BigCard").gameObject.SetActive(false);

                currentCard = Children.Find(x => Vector3.Dot(Vector2.up, x.up) > .97f);
                currentCard.Find("Card").GetComponent<Card>().activeCard = true;
                bigCardImage.sprite = currentCard.Find("Card").GetComponent<Card>().cardImage;
                BigCardText.text = $"RADIUS: {currentCard.Find("Card").GetComponent<Card>().radius}\n DAMAGE: {currentCard.Find("Card").GetComponent<Card>().damage}";
                //currentCard.Find("BigCard").gameObject.SetActive(true);

            }
            if (!attackTargetChoose)
            {
                if (Input.GetMouseButton(0))
                {

                    DOTween.Kill(selfTransform);
                    selfTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, /*Mathf.Clamp(*/transform.eulerAngles.z - Input.GetAxis("Mouse X") * 150 * Time.deltaTime/*, 0, 180)*/));
                }
                else
                {
                    if (selfTransform.localRotation.eulerAngles.z % 24 != 0)
                        selfTransform.DOLocalRotate(new Vector3(0, 0, round(Mathf.RoundToInt(transform.eulerAngles.z))), .1f);
                }
            }
            

        }
        
    }
    public void ToggleBigCard()
    {
        float duration = .1f;
        DOTween.Kill(BigCard.GetComponent<RectTransform>());
        if (BigCard.activeSelf)
        {
            BigCard.GetComponent<RectTransform>().DOScaleY(.1f, duration)
                .OnComplete(() => BigCard.SetActive(false));
            BigCard.GetComponent<Image>().DOFade(.1f, duration);
        }
        else
        {
            BigCard.SetActive(true);
            BigCard.GetComponent<RectTransform>().DOScaleY(2, duration);
            BigCard.GetComponent<Image>().DOFade(1, duration);
        }
    }
    public async void ShowCards()
    {
        //selfTransform.DOAnchorPosY(-390, .5f);
        foreach(RectTransform child in Children)
        {
            child.SetParent(_canvas);
            if(Children.IndexOf(child) < 7)
            {
                await selfTransform.DOLocalRotate(new Vector3(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.z - 24), .08f);
            }
            else
            {
                selfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.z - 24);
            }
            /*selfTransform.rotation = Quaternion.Euler( new Vector3(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.y - 24));*/
        }
        foreach(RectTransform child in Children)
        {
            child.SetParent(selfTransform);
        }
        activeMenu = true;
    }

    // function to round the number 
    static int round(int n)
    {
        // Smaller multiple 
        int a = (n / 24) * 24;

        // Larger multiple 
        int b = a + 24;

        // Return of closest of two 
        return (n - a > b - n) ? b : a;
    }
    public void CancelAttack(Card card)
    {
        if (card.arrow != null)
            GameObject.Destroy(card.arrow);
        if (card.targetTile != null)
            card.targetTile.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);

        player.ToggleAttackRadius();
        attackTargetChoose = false;
        
    }

    public void HideCards()
    {
        transform.DOLocalRotate(new Vector3(0,0,72), .1f);
        foreach (RectTransform child in Children)
        {
            child.DOLocalRotate(Vector3.zero, .1f);
        }
        activeMenu = false;
        /*foreach (RectTransform child in Children)
        {
            child.SetParent(_canvas);
            if (Children.IndexOf(child) < 7)
            {
                await selfTransform.DOLocalRotate(new Vector3(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.z - 24), .08f);
            }
            else
            {
                selfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.z - 24);
            }
            /*selfTransform.rotation = Quaternion.Euler( new Vector3(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y,
                selfTransform.rotation.eulerAngles.y - 24));
        }*
        foreach (RectTransform child in Children)
        {
            child.SetParent(selfTransform);
        }
        activeMenu = true;*/
    }

}
