using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeadState : EnemyState//在EnemyState中调用
{
    private Enemy_Skeleton enemy;

    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;//构造函数中多添加了一项用来给enemy赋值
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetBool(enemy.lastAnimBoolName, true);//
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;//关闭胶囊碰撞体

        stateTimer = .1f;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer>0)
            rb.velocity = new Vector2(0, 10);
    }
}
