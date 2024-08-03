using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration=0.2f;

    public bool isBusy{get;private set;}
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce ;
    public float swordReturnImpact;//接刀回来的冲击力

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dasDir{get;private set;}


    
    public SkillManager skill{get;private set;}
    public GameObject sword{get;private set;}

    #region States
    public PlayerStateMachine stateMachine{get;private set;}

    public PlayerIdolState idolState{get;private set;}
    public PlayerMoveState moveState{get;private set;}
    public PlayerJumpState jumpState{get;private set;}
    public PlayerAirState airState{get;private set;}
    public PlayerWallSlideState wallSlide{get;private set;}
    public PlayerWallJumpState wallJump{get;private set;}
    public PlayerDashState dashState{get;private set;}

    public PlayerPrimaryAttackState primaryAttack{get;private set;}
    public PlayerCounterAttackState counterAttack{get;private set;}

    public PlayerAimSwordState aimSword{get;private set;}
    public PlayerCatchSwordState catchSword{get;private set;}
    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        //stateMachine是状态机对象，用于管理不同状态之间的切换
        idolState = new PlayerIdolState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this,stateMachine,"Move");
        jumpState = new PlayerJumpState(this,stateMachine,"Jump");
        airState  = new PlayerAirState(this,stateMachine,"Jump");
        dashState = new PlayerDashState(this,stateMachine,"Dash");
        wallSlide = new PlayerWallSlideState(this,stateMachine,"WallSlide");
        wallJump = new PlayerWallJumpState(this,stateMachine,"Jump");

        primaryAttack = new PlayerPrimaryAttackState(this,stateMachine,"Attack");
        counterAttack = new PlayerCounterAttackState(this,stateMachine,"CounterAttack");

        aimSword = new PlayerAimSwordState(this,stateMachine,"AimSword");
        catchSword = new PlayerCatchSwordState(this,stateMachine,"CatchSword");
    }
    protected override void Start()
    {
        base.Start();
        skill=SkillManager.instance;
        stateMachine.Initialize(idolState);
        
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }
    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    public void CheckForDashInput()
    {
        if(IsWallDetected())
            return;
        //dashUsageTimer -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.LeftShift)/*&& dashUsageTimer<0*/&&SkillManager.instance.dash.CanUseSkill())
        {
            //dashUsageTimer = dashCooldown;
            dasDir = Input.GetAxisRaw("Horizontal");
            //按着右键时dasDir是1，左键按着为-1，不按左右直接冲刺为0
            if(dasDir==0)
                dasDir = facingDir;
            stateMachine.ChangeState(dashState);

        
        }
        
            
    }


    


}
