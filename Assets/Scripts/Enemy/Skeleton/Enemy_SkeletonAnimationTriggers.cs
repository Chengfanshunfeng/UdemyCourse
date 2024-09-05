using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SkeletonAnimationTriggers : MonoBehaviour
{
    private Enemy_Skeleton enemy=> GetComponentInParent<Enemy_Skeleton>();
    private void AnimationTrigger()
    {
        enemy.AnimationFinishedTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);//获取到玩家面板，把玩家面板传给怪物面板脚本执行造成伤害
            }

        }
    }
    private void OpenCounterWindow() => enemy.OpenConterAttackWindow();

    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
