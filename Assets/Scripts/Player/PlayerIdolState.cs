using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdolState : PlayerGroundedState
{
    public PlayerIdolState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(0, 0);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        //自己添加的
        // if(player.IsWallDetected()&&(xInput==player.facingDir))
        //     return;
        if(player.IsGroundDetected()&&xInput!=0&&!player.isBusy)
        {
            stateMachine.ChangeState(player.moveState);
        }


    }
}
