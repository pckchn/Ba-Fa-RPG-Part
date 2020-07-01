using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticalButton : MonoBehaviour
{

    // Start is called before the first frame update
    public Vector3 InterfacePosition;
    public GameObject player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseEnter()
    {
        player.GetComponent<PlayerController>().currentTacticalButton = this;
    }
    private void OnMouseExit()
    {
        if (player.GetComponent<PlayerController>().currentTacticalButton == this)
            player.GetComponent<PlayerController>().currentTacticalButton = null;
    }
}
