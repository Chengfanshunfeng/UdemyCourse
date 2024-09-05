using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player=>GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position,player.attackCheckRadius);
        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>()!=null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(_target);//获取到怪物面板，把怪物面板传给玩家面板脚本执行造成伤害

            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
