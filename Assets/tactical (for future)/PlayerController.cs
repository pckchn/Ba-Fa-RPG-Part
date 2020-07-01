using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

using UnityEngine.UI;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public enum AttackType { Sword, Bow, Glove };
    public AttackType _attackType;
    #region movement variables
    public bool walking = false;
    bool diagonal;
    public enum MovementType { Bishop, Rook };
    public MovementType movType;
    public int MovementRadius;
    public CardPivotScript cardControls;
    protected Walkable[] Tiles;
    List<GameObject> ExistingTilesForMovement = new List<GameObject>();

    List<Walkable> AvailablePaths = new List<Walkable>();
    GameObject currentCubeGameObject;

    [Space]

    public Transform currentCube;
    public Transform clickedCube;
    public Transform indicator;

    [Space]

    public List<Transform> finalPath = new List<Transform>();

    private float blend;
    #endregion

    //BATTLE INTERFACE
    #region Interface Buttons
    private float FingerHoldCounter;
    private bool isHoldingFinger;
    public GameObject ButtonsPivot;
    protected List<GameObject> buttons = new List<GameObject>();
    private bool MenuIsActive;
    public new Camera camera;
    [HideInInspector] public TacticalButton currentTacticalButton;
    [HideInInspector] public CombatButton currentCombatButton;
    private int currentTacticalButtonID;
    private bool MovementRadiusActive = false;
    private bool CombatMenuActive;
    private bool ChoosingAttackTarget;
    private bool MovingAttackTarget;
    //ARROW:
    public GameObject ArrowCurve;
    private Transform ArrowCollector;

    public CardPivotScript cardWheel;
    public enum attackElement { Fire, Water, Grass };
    [System.Serializable]
    public struct AttackInfo
    {
        public float Damage;
        public GameObject ArrowObject;
        public Walkable targetTile;
        public attackElement element;
        public Card card;
    }
    //private int currentAttackIndex;
    private List<AttackInfo> attacks = new List<AttackInfo>();
    private Walkable currentAttackTile;
    #endregion


    public bool MyTurn = true;
    public TurnSystem turnSystem;
    public int HP = 10;

    private void Awake()
    {
        ArrowCollector = transform.Find("ArrowCollector").transform;
        foreach (Transform child in ButtonsPivot.transform)
        {
            buttons.Add(child.gameObject);

            child.gameObject.GetComponent<BoxCollider2D>();
        }
        Tiles = FindObjectsOfType<Walkable>();
        RayCastDown();
        GetMovementRadius();
        switch (movType)
        {
            case MovementType.Bishop:
                diagonal = true;
                return;
            case MovementType.Rook:
                diagonal = false;
                return;
        }
    }
    void Start()
    {

    }

    void Update()
    {
        if (MyTurn)
        {
            #region detecting attack target

            if (MovingAttackTarget)
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit mouseHit;

                if (Physics.Raycast(mouseRay, out mouseHit)/* && !walking*/)
                {
                    if (mouseHit.transform.GetComponent<Walkable>() != null && AvailablePaths.Contains(mouseHit.transform.gameObject.GetComponent<Walkable>()))
                    {
                        /*if (attacks[attacks.Count() - 1].targetTile == null)
                            attacks[attacks.Count() - 1].targetTile.GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);*/
                        if (mouseHit.transform.GetComponent<Walkable>() != currentAttackTile && currentAttackTile != null)
                            currentAttackTile.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                        //currentAttackTile.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                        currentAttackTile = mouseHit.transform.GetComponent<Walkable>();
                        currentAttackTile.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = Color.red;
                        Card cardTemp = cardWheel.currentCard.Find("Card").GetComponent<Card>();

                        //mouseHit.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = Color.red;
                        for (int i = 0; i < cardTemp.arrow.GetComponent<LineRenderer>().positionCount; i++)
                        {
                            cardTemp.arrow.GetComponent<LineRenderer>().SetPosition(i, Vector3.Lerp(/*Vector3.zero*/ currentCube.position, /*currentCube.position -*/ mouseHit.transform.position, 1.0f / cardTemp.arrow.GetComponent<LineRenderer>().positionCount * i)
                                + Vector3.Lerp(Vector3.zero, Vector3.down, Mathf.InverseLerp(0, cardTemp.arrow.GetComponent<LineRenderer>().positionCount / 2,
                                Mathf.Sqrt(Mathf.Abs(cardTemp.arrow.GetComponent<LineRenderer>().positionCount / 2 - i)))) + Vector3.up * 1.5f);
                            //Debug.Log(1.0f/attacks[attacks.Count()-1].ArrowObject.GetComponent<LineRenderer>().positionCount);

                        }
                        //MURDERINTHEFIRSTDEGREE

                    }
                }
                else
                {
                    /*if(attacks[attacks.Count() - 1].targetTile != null)
                        attacks[attacks.Count() - 1].targetTile.GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);*/
                }
            }
            #endregion
            #region movement update
            //GET CURRENT CUBE (UNDER PLAYER)

            RayCastDown();

            if (currentCube.GetComponent<Walkable>().movingGround)
            {
                transform.parent = currentCube.parent;
            }
            else
            {
                transform.parent = null;
            }

            // CLICK ON CUBE

            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit mouseHit;
                if (MovementRadiusActive)
                {
                    if (Physics.Raycast(mouseRay, out mouseHit) && !walking)
                    {
                        if (mouseHit.transform.GetComponent<Walkable>() != null && AvailablePaths.Contains(mouseHit.transform.gameObject.GetComponent<Walkable>()))
                        {
                            clickedCube = mouseHit.transform;
                            //Debug.Log(clickedCube);
                            DOTween.Kill(gameObject.transform);
                            finalPath.Clear();
                            FindPath();

                            blend = transform.position.y - clickedCube.position.y > 0 ? -1 : 1;

                            indicator.position = mouseHit.transform.GetComponent<Walkable>().GetWalkPoint();
                            Sequence s = DOTween.Sequence();
                            s.AppendCallback(() => indicator.GetComponentInChildren<ParticleSystem>().Play());
                            s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
                            s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.black, .3f).SetDelay(.2f));
                            s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
                            toggleMovementRadius();

                        }
                    }
                }
            }
            #endregion
            #region finger hold
            if (isHoldingFinger)
            {
                FingerHoldCounter += Time.deltaTime;
                if (FingerHoldCounter > .2f)
                {
                    isHoldingFinger = false;
                    toggleMovementRadius(true);
                    Debug.Log("Open the menu");
                    ButtonsPivot.SetActive(true);
                    OpenTacticalMenu();
                    //FingerHoldCounter = 0;
                }
            }
            #endregion
        }

    }
    #region movement functions
    void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<Walkable>().possiblePaths)
        {
            if (path.active && path.diagonal == diagonal && path.target.GetComponent<Walkable>().occupied == false)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {

        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        if (current == clickedCube)
        {
            return;
        }
        if (MovementRadiusActive)
        {
            foreach (WalkPath path in current.GetComponent<Walkable>().possiblePaths)
            {
                if (!visitedCubes.Contains(path.target) && path.active && path.diagonal == diagonal && AvailablePaths.Contains(path.target.gameObject.GetComponent<Walkable>()) 
                    && path.target.GetComponent<Walkable>().occupied == false)
                {
                    nextCubes.Add(path.target);
                    path.target.GetComponent<Walkable>().previousBlock = current;
                }
            }
        }


        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }

    }

    void BuildPath()
    {
        Transform cube = clickedCube;
        while (cube != currentCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<Walkable>().previousBlock != null)
                cube = cube.GetComponent<Walkable>().previousBlock;
            else
                return;
        }

        finalPath.Insert(0, clickedCube);

        FollowPath();
    }

    void FollowPath()
    {
        Sequence s = DOTween.Sequence();
        if (!walking)
        {
            walking = true;
            for (int i = finalPath.Count - 1; i > 0; i--)
            {
                float time = finalPath[i].GetComponent<Walkable>().isStair ? 1.5f : 1;

                s.Append(transform.DOMove(finalPath[i].GetComponent<Walkable>().GetWalkPoint(), .2f * time).SetEase(Ease.Linear));

                if (!finalPath[i].GetComponent<Walkable>().dontRotate)
                    s.Join(transform.DOLookAt(finalPath[i].position, .1f, AxisConstraint.Y, Vector3.up));
            }

            if (clickedCube.GetComponent<Walkable>().isButton)
            {
                s.AppendCallback(() => GameManager.instance.RotateRightPivot());
            }

            s.AppendCallback(() => Clear());
        }

    }

    void Clear()
    {
        foreach (Transform t in finalPath)
        {
            t.GetComponent<Walkable>().previousBlock = null;
        }
        finalPath.Clear();
        //walking = false;
        GetMovementRadius();
        toggleMovementRadius();
    }

    public void RayCastDown()
    {

        Ray playerRay = new Ray(transform.GetChild(0).position, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                currentCube = playerHit.transform;
                if (playerHit.collider.gameObject != currentCubeGameObject)
                {
                    if (currentCubeGameObject != null)
                        currentCubeGameObject.GetComponent<Walkable>().occupied = false;

                    currentCubeGameObject = playerHit.collider.gameObject;

                    currentCubeGameObject.GetComponent<Walkable>().occupied = true;
                }
                if (playerHit.transform.GetComponent<Walkable>().isStair)
                {
                    DOVirtual.Float(GetBlend(), blend, .1f, SetBlend);
                }
                else
                {
                    DOVirtual.Float(GetBlend(), 0, .1f, SetBlend);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray ray = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(ray);
    }

    float GetBlend()
    {
        if (GetComponentInChildren<Animator>() != null)
            return GetComponentInChildren<Animator>().GetFloat("Blend");
        return 0;
    }
    void SetBlend(float x)
    {
        if (GetComponentInChildren<Animator>() != null)
            GetComponentInChildren<Animator>().SetFloat("Blend", x);
    }
    #endregion
    #region interface functions
    public void ToggleAttackRadius()
    {
        if (MyTurn)
        {
            if (ChoosingAttackTarget)
            {
                if (Vector3.Distance(turnSystem.enemy.currentCube.position, currentCube.position) == 1)
                {
                    turnSystem.enemy.currentCube.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                    turnSystem.enemy.currentCube.Find("RadiusIndicator").gameObject.SetActive(false);
                }

                foreach (Walkable GO in AvailablePaths)
                {
                    GO.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(95, 141, 218, 215);
                    GO.transform.Find("RadiusIndicator").gameObject.SetActive(false);
                    //Debug.Log("Hello");
                }
            }
            else
            {
                Card currentCardLocal = cardWheel.currentCard.Find("Card").GetComponent<Card>();
                //Debug.Log(currentCardLocal);
                /*AttackInfo AtILocal = new AttackInfo()
                {
                    Damage = currentCardLocal.damage,
                    element = currentCardLocal.element,
                    card = currentCardLocal,
                    ArrowObject = 
                };*/
                currentCardLocal.arrow = Instantiate(ArrowCurve, ArrowCollector.transform);
                currentCardLocal.arrow.transform.localPosition = new Vector3(currentCardLocal.arrow.transform.localPosition.x, 1, currentCardLocal.arrow.transform.localPosition.y);
                currentCardLocal.arrow.SetActive(true);
                currentCardLocal.isUsed = true;
                //attacks.Add(AtILocal);
                foreach (Walkable GO in AvailablePaths)
                {
                    GO.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                    GO.transform.Find("RadiusIndicator").gameObject.SetActive(true);
                }
            }
            ChoosingAttackTarget = !ChoosingAttackTarget;
        }


    }
    void GetMovementRadius()
    {
        if (MyTurn)
        {
            /*foreach (Walkable wk in AvailablePaths)
        {
            wk.gameObject.transform.Find("RadiusIndicator").gameObject.SetActive(false);
        }*/
            if (AvailablePaths.Count != 0)
            {
                toggleMovementRadius(true);
                AvailablePaths.Clear();
            }
            //MovementRadiusActive = false;
            ExistingTilesForMovement.Clear();
            //ExistingTilesForMovement.Clear();
            Vector3 locationtemp;
            locationtemp = currentCubeGameObject.transform.position;
            switch (movType)
            {
                case MovementType.Rook:
                    for (int i = 1; i <= MovementRadius; i++)
                    {
                        if (Tiles.Any(a => a.transform.position.x == currentCube.position.x + i && a.transform.position.z == currentCube.position.z))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.x == currentCube.position.x + i && a.transform.position.z == currentCube.position.z).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.x == currentCube.position.x - i && a.transform.position.z == currentCube.position.z))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.x == currentCube.position.x - i && a.transform.position.z == currentCube.position.z).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.z == currentCube.position.z - i && a.transform.position.x == currentCube.position.x))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.z == currentCube.position.z - i && a.transform.position.x == currentCube.position.x).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.z == currentCube.position.z + i && a.transform.position.x == currentCube.position.x))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.z == currentCube.position.z + i && a.transform.position.x == currentCube.position.x).gameObject);
                        }
                    }
                    break;
                case MovementType.Bishop:
                    for (int i = 1; i <= MovementRadius; i++)
                    {
                        if (Tiles.Any(a => a.transform.position.x == currentCube.position.x + i && a.transform.position.z == currentCube.position.z + i))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.x == currentCube.position.x + i && a.transform.position.z == currentCube.position.z + i).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.x == currentCube.position.x - i && a.transform.position.z == currentCube.position.z - i))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.x == currentCube.position.x - i && a.transform.position.z == currentCube.position.z - i).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.z == currentCube.position.z - i && a.transform.position.x == currentCube.position.x + i))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.z == currentCube.position.z - i && a.transform.position.x == currentCube.position.x + i).gameObject);
                        }
                        if (Tiles.Any(a => a.transform.position.z == currentCube.position.z + i && a.transform.position.x == currentCube.position.x - i))
                        {
                            ExistingTilesForMovement.Add(Array.Find(Tiles, a => a.transform.position.z == currentCube.position.z + i && a.transform.position.x == currentCube.position.x - i).gameObject);
                        }
                    }
                    break;
            }
            //Debug.Log(ExistingTilesForMovement.Count());
            foreach (GameObject GO in ExistingTilesForMovement)
            {

                if ((ExistingTilesForMovement.Any(a => a.GetComponent<Walkable>().possiblePaths.Any(b => b.target == GO.transform) && a != GO)) || 
                    (currentCubeGameObject.GetComponent<Walkable>().possiblePaths.Any(c => c.target == GO.transform)))
                {
                    AvailablePaths.Add(GO.GetComponent<Walkable>());
                }
                //Debug.Log(AvailablePaths.Count());
            }
        }


    }
    private void toggleMovementRadius()
    {
        if (!walking)
        {
            if (MovementRadiusActive)
            {
                MovementRadiusActive = false;
                foreach (Walkable GO in AvailablePaths)
                {
                    GO.transform.Find("RadiusIndicator").gameObject.SetActive(false);
                }

            }
            else
            {
                MovementRadiusActive = true;
                foreach (Walkable GO in AvailablePaths)
                {
                    GO.transform.Find("RadiusIndicator").gameObject.SetActive(true);
                }
            }
        }

    }
    private void toggleMovementRadius(bool Erase)
    {
        if (Erase && MovementRadiusActive)
        {
            MovementRadiusActive = false;
            foreach (Walkable GO in AvailablePaths)
            {
                GO.transform.Find("RadiusIndicator").gameObject.SetActive(false);
            }
        }
    }

    void OpenTacticalMenu()
    {
        ButtonsPivot.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        //ButtonsPivot.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        for (int i = 0; i <= 4; i++)
        {
            buttons[i].GetComponent<SpriteRenderer>().DOFade(1, .3f);
            buttons[i].SetActive(true);
            //buttons[i].GetComponent<RectTransform>().localPosition = buttons[i].GetComponent<TacticalButton>().InterfacePosition;
            //buttons[i].GetComponent<RectTransform>().DOAnchorPos(buttons[i].GetComponent<TacticalButton>().InterfacePosition, .3f);
            buttons[i].transform.DOLocalMove(buttons[i].GetComponent<TacticalButton>().InterfacePosition, .3f);
            //.OnComplete(()=>OpenMenu());
        }
        MenuIsActive = true;
    }
    void CancelButtonAnimation()
    {
        for (int i = 0; i <= 4; i++)
        {
            DOTween.Kill(buttons[i].transform.GetComponent<SpriteRenderer>());
            DOTween.Kill(buttons[i].transform);
            buttons[i].transform.DOLocalMove(Vector3.zero, .1f);
            buttons[i].GetComponent<SpriteRenderer>().DOFade(0, .1f)
                .OnComplete(() =>
                {
                    if (buttons[0].activeSelf)
                    {
                        for (int x = 0; x <= 4; x++)
                        {
                            buttons[x].SetActive(false);
                        }
                    }
                });
        }
        MenuIsActive = false;
    }
    void OpenMenu()
    {
        //Debug.Log("Hello");
    }
    void UseTacticalButton()
    {
        currentTacticalButtonID = buttons.IndexOf(currentTacticalButton.gameObject);
        Debug.Log(currentTacticalButtonID);
        switch (currentTacticalButtonID)
        {
            case 0:
                return;
            case 1:
                return;
            case 4:
                if (!CombatMenuActive)
                    OpenCombatMenu();
                return;
        }
    }
    void OpenCombatMenu()
    {
        CombatMenuActive = true;
        //ButtonsPivot.GetComponent<RectTransform>().position = camera.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        ButtonsPivot.transform.DOLocalRotate(new Vector3(90, ButtonsPivot.transform.rotation.eulerAngles.y, ButtonsPivot.transform.rotation.eulerAngles.z), .3f);
        for (int i = 0; i <= 7; i++)
        {
            buttons[i].GetComponent<SpriteRenderer>().DOFade(.5f, .3f);
            if (buttons[i].GetComponent<BoxCollider2D>() != null)
                buttons[i].GetComponent<BoxCollider2D>().enabled = false;
            buttons[i].SetActive(true);
            //buttons[i].GetComponent<RectTransform>().localPosition = buttons[i].GetComponent<TacticalButton>().InterfacePosition;
            buttons[i].transform.DOLocalMove(buttons[i].GetComponent<CombatButton>().InterfacePosition, .3f);
            //.OnComplete(()=>OpenMenu());
        }
        cardControls.ShowCards();
    }
    #endregion
    #region mouse inputs
    private void OnMouseDown()
    {
        if (MyTurn)
        {
            if (ChoosingAttackTarget)
            {
                MovingAttackTarget = true;
            }
            else
            {
                if (!MenuIsActive && !CombatMenuActive)
                    isHoldingFinger = true;
                else
                {
                    if (!cardWheel.BigCard.activeSelf)
                    {
                        cardWheel.HideCards();
                        ButtonsPivot.transform.DOLocalRotate(new Vector3(35.264f, ButtonsPivot.transform.rotation.eulerAngles.y, ButtonsPivot.transform.rotation.eulerAngles.z), .3f);
                        for (int i = 0; i < ButtonsPivot.transform.childCount; i++)
                        {
                            buttons[i].GetComponent<SpriteRenderer>().DOFade(0, .3f);
                            if (buttons[i].GetComponent<BoxCollider2D>() != null)
                                buttons[i].GetComponent<BoxCollider2D>().enabled = true;
                            buttons[i].SetActive(false);
                            //buttons[i].GetComponent<RectTransform>().localPosition = buttons[i].GetComponent<TacticalButton>().InterfacePosition;
                            buttons[i].transform.DOLocalMove(Vector3.zero, .3f)
                                .OnComplete(() =>
                                {
                                    ButtonsPivot.SetActive(false);
                                    MenuIsActive = false;
                                    CombatMenuActive = false;

                                });
                            //.OnComplete(()=>OpenMenu());
                        }
                    }
                }
            }
        }

    }
    private void OnMouseUp()
    {
        if (MyTurn)
        {
            if (MovingAttackTarget)
            {
                cardWheel.currentCard.Find("Card").GetComponent<Card>().targetTile = currentAttackTile;
                MovingAttackTarget = false;
                //attacks.Find(x->)
            }
            FingerHoldCounter = 0;
            isHoldingFinger = false;
            if (!CombatMenuActive)
            {
                if (!MenuIsActive)
                {
                    if (FingerHoldCounter <= .2f)
                    {
                        Debug.Log("Show radius");
                        toggleMovementRadius();

                    }
                }
                if (currentTacticalButton != null)
                {
                    UseTacticalButton();
                    return;
                }
                else
                {
                    CancelButtonAnimation();
                }


                if (!MenuIsActive)
                {

                }
            }
        }

    }
    #endregion
    public void EndTurn()
    {
        if (ChoosingAttackTarget)
        {
            //ToggleAttackRadius();
            cardWheel.CancelAttack(cardWheel.currentCard.Find("Card").GetComponent<Card>());
            cardWheel.HideCards();
            cardWheel.HideCards();
            ButtonsPivot.transform.DOLocalRotate(new Vector3(35.264f, ButtonsPivot.transform.rotation.eulerAngles.y, ButtonsPivot.transform.rotation.eulerAngles.z), .3f);
            for (int i = 0; i < ButtonsPivot.transform.childCount; i++)
            {
                buttons[i].GetComponent<SpriteRenderer>().DOFade(0, .3f);
                if (buttons[i].GetComponent<BoxCollider2D>() != null)
                    buttons[i].GetComponent<BoxCollider2D>().enabled = true;
                buttons[i].SetActive(false);
                //buttons[i].GetComponent<RectTransform>().localPosition = buttons[i].GetComponent<TacticalButton>().InterfacePosition;
                buttons[i].transform.DOLocalMove(Vector3.zero, .3f)
                    .OnComplete(() =>
                    {
                        ButtonsPivot.SetActive(false);
                        MenuIsActive = false;
                        CombatMenuActive = false;

                    });
                //.OnComplete(()=>OpenMenu());
            }

            if (currentAttackTile.occupied)
            {
                turnSystem.enemy.GetComponent<STATEMACHINE>().TakeDamage(5);
            }
            //ChoosingAttackTarget = false;
        }

        turnSystem.EndTurn(gameObject);
    }
}
