using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    //enum currentTurn {player, bots};
    public STATEMACHINE enemy;
    public PlayerController player;
    // Start is called before the first frame update

    public void EndTurn(GameObject GO)
    {
        if(GO.GetComponent<PlayerController>() != null)
        {
            player.MyTurn = false;
            player.walking = false;
            enemy.MyTurn = true;
            enemy.Decide();
        }
        else
        {
            player.MyTurn = true;
            enemy.MyTurn = false;
        }
    }
}
