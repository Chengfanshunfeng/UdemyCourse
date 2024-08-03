using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField]protected LayerMask whatisPlayer;
    [Header("Stunned info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;
    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;
    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;

    public EnemyStateMachine stateMachine{get; private set;}

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        //Debug.Log(IsPlayerDetected().collider.gameObject.name+"I See");

    }

    public virtual void FreezeTime(bool _timeFrozen)//怪物时间冻结
    {
        if(_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    protected virtual IEnumerator FreezeTimer(float _seconds)//通过协程传递冻结时间让怪物时间冻结
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);
        
        FreezeTime(false);
    }
    #region Counter Attack Window
    public virtual void OpenConterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }

    #endregion

    public virtual bool CanBeStunned()
    {
        if(canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }
    public virtual void AnimationFinishedTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerDetected()=>Physics2D.Raycast(wallCheck.position,Vector2.right*facingDir,50,whatisPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x+attackDistance*facingDir,transform.position.y));
    }
}
