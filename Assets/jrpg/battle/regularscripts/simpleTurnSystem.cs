using UnityEngine;
using DG.Tweening;
using System.Collections;

public class simpleTurnSystem : MonoBehaviour
{
    public EnemyScript[] enemies;
    public Transform pivot;
    public simplePlayerScript player;
    int currentEnemy;
    public float duration = 10;
    // Start is called before the first frame update
    void Start()
    {
        player.currentEnemy = enemies[0];
        //enemies[currentEnemy].takeDamage(100);
    }
    public void EndTurn(bool ME)
    {
        if (ME)
        {
            StartCoroutine(EnemyAct());
            //enemies[currentEnemy].Act();
        }
        else
        {
            StartCoroutine(ActMyself());
            //player.myTurn();
        }
    }
    IEnumerator EnemyAct()
    {
        yield return new WaitForSeconds(1);
        enemies[currentEnemy].Act();

    }
    IEnumerator ActMyself()
    {
        yield return new WaitForSeconds(1);
        player.myTurn();
    }
    public void TangoDown()
    {
        if(currentEnemy + 1 <= enemies.Length - 1)
        {
            currentEnemy++;
            player.currentEnemy = enemies[currentEnemy];
            pivot.DOLocalMoveX(pivot.position.x - 18.06264f, duration)
                .OnComplete(()=>RestoreEnemyHP());
            
        }
        else
        {
            WIN();
        }
        
    }
    void RestoreEnemyHP()
    {
        player.currentEnemy.hpbar.FullHP(8);
        player.currentEnemy.hpbar.newEnemy(player.currentEnemy);
    }
    void WIN()
    {
        Debug.Log("YOU WON");
    }
}
