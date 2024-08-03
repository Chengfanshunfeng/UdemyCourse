using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f);
    }
    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
        if(Input.GetKeyUp(KeyCode.Mouse1))
            player.stateMachine.ChangeState(player.idolState);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(player.transform.position.x > mousePosition.x&&player.facingDir == 1)//玩家的x坐标比鼠标x坐标大，此时需要检查转向，再设定一个玩家朝向
            player.Flip();
        else if(player.transform.position.x < mousePosition.x&&player.facingDir == -1)
            player.Flip();
    }
}
