using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player=PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;

    }
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer<=0)
        {
            UseSkill();
            cooldownTimer = cooldown;//释放完技能后设置冷却
            return true;
        }
        Debug.Log("Skill is on cooldown");
        return false;
    }
    public virtual void UseSkill()
    { 
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)//寻找最近的敌人
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position,25);

        float closeseDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if(distanceToEnemy < closeseDistance)
                {
                    closeseDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }
}
