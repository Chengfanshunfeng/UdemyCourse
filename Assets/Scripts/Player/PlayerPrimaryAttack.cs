using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;
    private float lastTimeAttacked;
    private float comboWindow = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        xInput = 0;
        if(comboCounter>2||Time.time>=lastTimeAttacked+comboWindow)
            comboCounter = 0;
        player.anim.SetInteger("ComboCounter", comboCounter);

        #region Choose attack direction
        float attackDir = player.facingDir;
        if(xInput!=0)
            attackDir = xInput;
        #endregion
        //player.anim.speed = 3;
        player.SetVelocity(player.attackMovement[comboCounter].x*attackDir,player.attackMovement[comboCounter].y);
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .15f);
        //player.anim.speed = 1;
        comboCounter++;
        //Time.time是游戏开始到当前时刻的时间
        lastTimeAttacked = Time.time;

    }

    public override void Update()
    {
        base.Update();
        if(stateTimer<0)
            rb.velocity = new Vector2(0,0);
        if(triggerCalled)
            stateMachine.ChangeState(player.idolState);

    }
}
