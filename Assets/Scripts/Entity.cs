using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Component
    public Animator anim{get;private set;}
    public Rigidbody2D rb{get;private set;}
    public EntityFX fx{get;private set;}
    public SpriteRenderer sr{get;private set;}
    public CharacterStats stats{get;private set;}
    public CapsuleCollider2D cd{get;private set;}
    #endregion
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuraction;
    protected bool isKnocked;
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    //[SerializeField]表示该变量表示虽然该变量为私有但也可以在编辑器外更改数值
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int facingDir{get;private set;}=1;
    protected bool facingRight = true;

    public System.Action onFlipped;//声明一个翻转事件

    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        sr=GetComponentInChildren<SpriteRenderer>();//GetComponentInChildren 会先检查当前 GameObject 是否有该组件，如果有则直接返回，否则才会去子物体中查找。
        anim=GetComponentInChildren<Animator>();
        rb=GetComponent<Rigidbody2D>();
        fx=GetComponent<EntityFX>();//GetComponent只在当前 GameObject 上查找指定类型的组件。
        stats=GetComponent<CharacterStats>();
        cd=GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageEffect()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
        //Debug.Log(gameObject.name + "was damaged!");
    }
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y );
        //Debug.Log("Knocked");
        yield return new WaitForSeconds(knockbackDuraction);
        isKnocked = false;
    }
    public void SetZeroVelocity()
    {
        if(isKnocked)
            return;
        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity,float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity,_yVelocity);
        FlipController(_xVelocity);
    }
    #region Collision
    public virtual bool IsGroundDetected()=>Physics2D.Raycast(groundCheck.position,Vector2.down,groundCheckDistance,whatIsGround);
    public virtual bool IsWallDetected()=>Physics2D.Raycast(wallCheck.position,Vector2.right*facingDir,wallCheckDistance,whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position,new Vector3(groundCheck.position.x,groundCheck.position.y-groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position,new Vector3(wallCheck.position.x+wallCheckDistance,wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position,attackCheckRadius);
    }
    #endregion
    #region Flip
        public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0f,180f,0f);

        if(onFlipped!=null)//检查委托是否为空，如果非空则执行方法
            onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if(_x>0&&!facingRight)
            Flip();
        else if(_x<0&&facingRight)
            Flip();
    }
    #endregion

    public void MakeTransparent(bool _transparent)
    {
        if(_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    public virtual void Die()
    {

    }
}
