using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate=true;
    private bool isReturning;//飞刀是否正在回来


    private float freezeTimeDuration;
    private float returnSpeed = 12f;

    [Header("Pierce info")]
    private float pierceAmount;//穿刺次数

    [Header("Bounce info")]
    private float bounceSpeed;//飞刀弹跳速度
    private bool isBouncing;//是否可以弹跳
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;//作为检索弹跳列表的索引

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;//旋转是否停止
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        cd = GetComponent<CircleCollider2D>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void DestoryMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale,Player _player,float _freezeTimeDuration,float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if(pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);//标准化X轴的速度值，使用 Mathf.Clamp 函数限制了从 rb.velocity.x 得到的速度值。Mathf.Clamp 函数确保返回的值不会小于 -1 也不会大于 1。
        
        Invoke("DestoryMe", 7);
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBounces,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();//为了让飞刀弹跳，所以初始化列表，当列表有东西才会发生弹跳
    }

    public void SetupPierce(float _pierceAmount)
    {
        pierceAmount = _pierceAmount;//初始化穿刺次数
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration,float _hitCooldown)
    {
        isSpinning = _isSpinning;//旋转设置为true
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()//飞刀回来时触发
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//冻结刚体在所有轴上的位置和旋转
        //rb.isKinematic = false;//isKinematic是一个布尔值属性，用于控制Rigidbody是否应该受物理引擎的约束。当isKinematic设置为true时，Rigidbody将忽略所有物理交互，包括重力、碰撞以及来自关节的力
        transform.parent = null;//断开与父节点的联系，本身依然存在
        isReturning = true;
    }

    private void Update()
    {
        //rb.velocity表示物体的刚体组件（Rigidbody）的瞬时速度向量
        //transform.right返回的是一个单位向量，其默认在未旋转的情况下指向X轴的正方向（即(1, 0, 0)）
        //这样的话等于将一个不为单位向量的值（rb.velocity）给单位向量（transform.right）赋值
        //transform.right = Vector3.Normalize(rb.velocity);可以规范化为单位向量再赋值
        if (canRotate)
            transform.right = rb.velocity;
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)//旋转逻辑
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x + spinDirection, transform.position.y),1.5f * Time.deltaTime);//到达终点后进行一小段距离的移动

                if (spinTimer <= 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }
            }

            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0)
            {
                hitTimer = hitCooldown;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        SwordSkillDamage(hit.GetComponent<Enemy>());
                }
            }
        }
    }

    private void StopWhenSpinning()//旋转状态值的修改
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)//飞刀弹跳
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)//如果弹跳后达到了对应位置
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());//造成伤害与冻结怪物

                targetIndex++;//列表的key更新为下一个，因为key在不断更新，所以不会执行多次
                bounceAmount--;//弹跳次数减少

                if (bounceAmount <= 0)
                {
                    isBouncing = false;//弹跳结束
                    isReturning = true;//飞刀状态为回来
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;//重置key值
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)//飞刀回来时
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        SetupTargetForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)//造成伤害与冻结怪物
    {
        enemy.DamageEffect();//造成伤害
        enemy.StartCoroutine("FreezeTimer", freezeTimeDuration);
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)//飞刀弹跳的判定
        {
            if (isBouncing && enemyTarget.Count <= 0)//如果可以弹跳并且列表为空
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);//加入的是碰撞体组件的transform
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    //StuckInto 函数负责处理剑在碰撞后的状态，包括停止其运动、禁用碰撞检测、更新动画状态以及将其附着到碰撞物体上。
    //如果剑正在弹跳并且还有目标，函数会立即返回，避免执行这些操作，以便剑能够继续其弹跳行为。
    {
        if(pierceAmount > 0&&collision.GetComponent<Enemy>()!=null)//如果还有穿透次数就穿透敌怪，穿透次数减一
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)//旋转飞刀穿透敌怪
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;//回来时物体不再旋转，使transform.right不再根据rb.velocity旋转
        cd.enabled = false;//防止进一步的碰撞检测。禁用碰撞器，禁用剑的 CircleCollider2D，

        rb.isKinematic = true;//为了让剑回来，使剑不再受重力和碰撞
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//冻结rb组件本身的所有自由度，让剑不移动或旋转

        if (isBouncing&&enemyTarget.Count > 0)
            return;//弹跳状态时不执行下面的方法

        anim.SetBool("Rotation", false);
        //将飞刀的父对象设置为与发生碰撞的游戏对象相同的父对象
        transform.parent = collision.transform;
    }
}
