using Boo.Lang;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class simplePlayerScript : MonoBehaviour
{

    public HPBAR hpbar;

    private List <AttackDefinition> chosenAttacks = new List<AttackDefinition>();
    public int HP;
    public Text[] btnstxt;
    public float manapull;
    float currentMana;
    public GameObject layer1, layer2;
    public EnemyScript currentEnemy;
    public simpleTurnSystem turnsystem;

    public CardPivotScript cardWheel;
    public Sprite WhiteCircle;

    public Animator _animator;
    public Transform Menu;
    List<Image> buttons = new List<Image>();

    public Camera _cam;
    private void Awake()
    {
        foreach(Transform child in Menu)
        {
            buttons.Add(child.GetComponent<Image>());
        }
        currentMana = manapull;
        /*for(int i = 0; i<4; i++)
        {
            btnstxt[i].text = attacks[i].Name;
        }*/
    }
    private void Start()
    {
        myTurn();
    }
    void endTurn()
    {
        //layer1.SetActive(false);
        layer2.SetActive(false);
        HideMenu();
        turnsystem.EndTurn(true);
        cardWheel.imageTr.gameObject.SetActive(false);
        cardWheel.HideCards();
        cardWheel.transform.localPosition = new Vector3(cardWheel.transform.localPosition.x, cardWheel.transform.localPosition.y + 500, cardWheel.transform.localPosition.z);
        cardWheel.imageTr.localPosition = new Vector3(cardWheel.imageTr.localPosition.x, cardWheel.imageTr.localPosition.y + 450, cardWheel.imageTr.localPosition.z);
    }
    public void myTurn()
    {
        layer1.SetActive(true);
        Invoke("showMenu", .5f);
    }
    void doDamage(EnemyScript enemy, int damage)
    {
        enemy.takeDamage(damage);
    }
    public void backBtn()
    {
        layer1.SetActive(true);
        layer2.SetActive(false);
    }
    public void attackButton()
    {
        layer1.SetActive(false);
        cardWheel.ShowCards();
        cardWheel.imageTr.gameObject.SetActive(true);
        //layer2.SetActive(true);
        
    }
    public bool atk(AttackDefinition attack)
    {
        if(currentMana - attack.manaCost >= 0)
        {
            AttackDefinition currentAttack = attack;
            //doDamage(currentEnemy, currentAttack.damage);
            currentMana -= currentAttack.manaCost;
            //endTurn();
            chosenAttacks.Add(attack);
            cardWheel.images[chosenAttacks.Count - 1].sprite = attack.icon;
            cardWheel.images[chosenAttacks.Count - 1].transform.localScale = Vector3.one * 3;
            //attacks.Remove(attack);
            //Debug.Log(currentMana);
            if (chosenAttacks.Count == 4)
                layer2.SetActive(true);
            return true;
        }
        return false;
    }
    public void Execute()
    {
        StartCoroutine(ExecuteAttack());
        layer2.SetActive(false);
        cardWheel.transform.DOLocalMoveY(cardWheel.transform.localPosition.y-500, .2f);
        cardWheel.imageTr.DOLocalMoveY(-450, .2f);
    }
    public IEnumerator ExecuteAttack()
    {
        foreach(AttackDefinition atk in chosenAttacks)
        {
            currentEnemy.takeDamage(atk.damage);
            _animator.SetTrigger("attack");
            _cam.DOShakePosition(.3f,1);
            yield return new WaitForSeconds(.8f);

            Debug.Log(atk);
        }
        foreach(Image image in cardWheel.images)
        {
            image.sprite = WhiteCircle;
            image.transform.localScale = Vector3.one;
        }
        chosenAttacks.Clear();
        endTurn();
    }
    public void TakeDamage(int damage)
    {
        HP -= damage;
        hpbar.takeDamage(damage);
        if (HP <= 0)
            DIE();
    }
    public void DIE()
    {
        Debug.Log("DEAD");
    }
    void showMenu()
    {
        //cardWheel.gameObject.SetActive(false);
        Menu.DOLocalRotate(Vector3.zero, .2f);
        foreach(Image image in buttons)
        {
            image.DOFade(1, .2f);
        }
    }
    void HideMenu()
    {
        Menu.DOLocalRotate(new Vector3(0, 0, 119.267f), .2f);
        foreach (Image image in buttons)
        {
            image.DOFade(1, .2f);
        }
    }
}
