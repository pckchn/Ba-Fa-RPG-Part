using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class STATEMACHINE : MonoBehaviour
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

    //private float blend;
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







    public bool MyTurn = false;
    public PlayerController player;
    private Walkable chosenPoint;
    public TurnSystem turnSystem;
    public float HP = 10;

    Animator anim;





    private void Awake()
    {
        //ArrowCollector = transform.Find("ArrowCollector").transform;
        /*foreach (Transform child in ButtonsPivot.transform)
        {
            buttons.Add(child.gameObject);

            child.gameObject.GetComponent<BoxCollider2D>();
        }*/
        Tiles = FindObjectsOfType<Walkable>();
        RayCastDown();
        //GetMovementRadius();
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
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            Debug.Log("hi bitch");
        }
    }

    void Update()
    {
        #region detecting attack target

        if (MovingAttackTarget)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit) && !walking)
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
                        cardTemp.arrow.GetComponent<LineRenderer>().SetPosition(i, Vector3.Lerp(Vector3.zero, currentCube.position - mouseHit.transform.position, 1.0f / cardTemp.arrow.GetComponent<LineRenderer>().positionCount * i)
                            + Vector3.Lerp(Vector3.zero, Vector3.down, Mathf.InverseLerp(0, cardTemp.arrow.GetComponent<LineRenderer>().positionCount / 2,
                            Mathf.Sqrt(Mathf.Abs(cardTemp.arrow.GetComponent<LineRenderer>().positionCount / 2 - i)))));
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


        #endregion
    }
    #region movement functions
    void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<Walkable>().possiblePaths)
        {
            if (path.active && path.diagonal == diagonal)
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
                if (!visitedCubes.Contains(path.target) && path.active && path.diagonal == diagonal && AvailablePaths.Contains(path.target.gameObject.GetComponent<Walkable>()))
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
            //toggleMovementRadius();
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
        walking = false;
        //AvailablePaths.Clear();
        //GetMovementRadius();
        toggleMovementRadius(true);
        turnSystem.EndTurn(gameObject);
    }

    public void RayCastDown()
    {

        Ray playerRay = new Ray(transform/*.GetChild(0)*/.position, -transform.up);
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
                    //DOVirtual.Float(GetBlend(), blend, .1f, SetBlend);
                }
                else
                {
                    //DOVirtual.Float(GetBlend(), 0, .1f, SetBlend);
                }
            }
        }
        //Debug.Log(currentCube);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Ray ray = new Ray(transform/*.GetChild(0)*/.position, -transform.up);
        Gizmos.DrawRay(ray);
    }

    /*float GetBlend()
    {
        if (GetComponentInChildren<Animator>() != null)
            return GetComponentInChildren<Animator>().GetFloat("Blend");
        return 0;
    }*/
    /*void SetBlend(float x)
    {
        if (GetComponentInChildren<Animator>() != null)
            GetComponentInChildren<Animator>().SetFloat("Blend", x);
    }*/
    #endregion
    #region interface functions
    public void ToggleAttackRadius()
    {

        if (ChoosingAttackTarget) 
        {
            if (Vector3.Distance(player.currentCube.position, currentCube.position) == 1)
            {
                player.currentCube.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                player.currentCube.Find("RadiusIndicator").gameObject.SetActive(false);
            }

            foreach (Walkable GO in AvailablePaths)
            {
                GO.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(95, 141, 218, 215);
                GO.transform.Find("RadiusIndicator").gameObject.SetActive(false);
            }
        }
        else
        {
            
            //Card currentCardLocal = cardWheel.currentCard.Find("Card").GetComponent<Card>();
            //Debug.Log(currentCardLocal);
            /*AttackInfo AtILocal = new AttackInfo()
            {
                Damage = currentCardLocal.damage,
                element = currentCardLocal.element,
                card = currentCardLocal,
                ArrowObject = 
            };*/
            //currentCardLocal.arrow = Instantiate(ArrowCurve, ArrowCollector.transform);
            //currentCardLocal.arrow.transform.localPosition = new Vector3(currentCardLocal.arrow.transform.localPosition.x, 1, currentCardLocal.arrow.transform.localPosition.y);
            //currentCardLocal.arrow.SetActive(true);
            //currentCardLocal.isUsed = true;
            ////attacks.Add(AtILocal);
            foreach (Walkable GO in AvailablePaths)
            {
                GO.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = new Color32(228, 0, 70, 50);
                GO.transform.Find("RadiusIndicator").gameObject.SetActive(true);
            }
        }
        ChoosingAttackTarget = !ChoosingAttackTarget;
    }
    void GetMovementRadius()
    {
        //Debug.Log("Hello");
        /*foreach (Walkable wk in AvailablePaths)
        {
            wk.gameObject.transform.Find("RadiusIndicator").gameObject.SetActive(false);
        }*/
        if (AvailablePaths.Count != 0)
        {
            //toggleMovementRadius(true);
            AvailablePaths.Clear();
        }
        //MovementRadiusActive = false;
        ExistingTilesForMovement.Clear();
        //ExistingTilesForMovement.Clear();
        Vector3 locationtemp;
        locationtemp = currentCubeGameObject.transform.position;
        //Debug.Log(Tiles.Length);
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

            if ((ExistingTilesForMovement.Any(a => a.GetComponent<Walkable>().possiblePaths.Any(b => b.target == GO.transform) && a != GO)) || (currentCubeGameObject.GetComponent<Walkable>().possiblePaths.Any(c => c.target == GO.transform)))
            {
                //Debug.Log()
                if (GO.GetComponent<Walkable>().occupied == false)
                {
                    AvailablePaths.Add(GO.GetComponent<Walkable>());
                    //GO.transform.Find("RadiusIndicator").gameObject.SetActive(true);
                }
            }
            //Debug.Log(AvailablePaths.Count());
        }

    }
    private void toggleMovementRadius()
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

    #endregion
    public void Decide()
    {
        if (FindNearestPoint())
        {
            ToggleAttackRadius();
            chosenPoint.gameObject.transform.Find("RadiusIndicator").GetComponent<SpriteRenderer>().color = Color.red;
            transform.LookAt(player.transform.position);
            GetComponent<Animator>().SetTrigger("ATTACK");
            player.HP -= 1;
            Invoke("Attack", 1);
            //Debug.Log("ATTACK");
        }
        else
        {
            toggleMovementRadius();

            //AvailablePaths.Clear();
            FindPath();
        }

    }
    private bool FindNearestPoint()
    {
        if (Vector3.Distance(player.currentCube.position, currentCube.position) == 1)
            return true;
        GetMovementRadius();
        chosenPoint = null;
        clickedCube = null;
        //true if player is within radius
        //Debug.Log(AvailablePaths.Count);
        foreach (Walkable Tile in AvailablePaths)
        {
            if (chosenPoint == null)
                chosenPoint = Tile;
            //Debug.Log(chosenPoint);
            if(Vector3.Distance(Tile.transform.position, player.currentCube.transform.position)<Vector3.Distance(chosenPoint.transform.position, player.currentCube.position))
            {
                chosenPoint = Tile;
                if (player.currentCube == Tile)
                    return true;
            }
        }
        //Debug.Log(chosenPoint);
        clickedCube = chosenPoint.transform;
        Debug.Log(clickedCube);
        DOTween.Kill(gameObject.transform);
        finalPath.Clear();
        //FindPath();

        //blend = transform.position.y - clickedCube.position.y > 0 ? -1 : 1;

        indicator.position = chosenPoint.GetWalkPoint();
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => indicator.GetComponentInChildren<ParticleSystem>().Play());
        s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
        s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.black, .3f).SetDelay(.2f));
        s.Append(indicator.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
        //toggleMovementRadius();

        return false;
    }
    void Attack()
    {
        ToggleAttackRadius();
        turnSystem.EndTurn(gameObject);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("OnDeath");
        Destroy(gameObject, 3f);
    }
}

