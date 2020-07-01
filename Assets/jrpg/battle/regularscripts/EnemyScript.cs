using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float mana;
    float currentMana;
    public int HP;
    public simplePlayerScript player;
    public AttackDefinition[] attacks = new AttackDefinition[4];
    AttackDefinition currentAttack;

    public EnemyHPBar hpbar;
    public simpleTurnSystem turnsystem;
    private void Awake()
    {
        currentMana = mana;
    }
    public void Act()
    {
        if(Random.Range(0,mana) < currentMana)
        {
            //attack
            foreach(AttackDefinition attack in attacks)
            {
                if(currentMana - attack.manaCost >= 0)
                {
                    if (currentAttack == null)
                        currentAttack = attack;
                    else
                    {
                        if (currentAttack.damage < attack.damage)
                            currentAttack = attack;
                    }
                }
            }
        }
        if(currentAttack != null)
        {
            currentMana -= currentAttack.manaCost;
            Attack(currentAttack);
            currentAttack = null;
            EndTurn();
            return;
        }

        //propusk hoda
        Debug.Log("PROPUSKHODA");
        EndTurn();
    }
    void Attack(AttackDefinition attack)
    {
        //Debug.Log(attack);
        player.TakeDamage(attack.damage);
    }
    public void takeDamage(int damage)
    {
        HP -= damage;
        hpbar.takeDamage(damage);
        if (HP <= 0)
            Die();
    }
    void Die()
    {
        turnsystem.TangoDown();
        Destroy(gameObject);
    }
    void EndTurn()
    {
        turnsystem.EndTurn(false);
    }
}
