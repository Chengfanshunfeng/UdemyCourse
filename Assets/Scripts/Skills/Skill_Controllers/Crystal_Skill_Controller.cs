using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;//水晶存在时间

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed=5;//爆炸时的变大速度

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;
    public void SetupCrystal(float _crystalDuration,bool _canExplode,bool _canMove,float _moveSpeed,Transform _closestTarget)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,radius,whatIsEnemy);
        
        if(colliders.Length > 0)
            closestTarget = colliders[Random.Range(0,colliders.Length)].transform;//在colliders中随机选择一个transform
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;//减少销毁时间

        if(crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canMove)//水晶如果可以移动的话移动到最近目标处
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position,closestTarget.position)<1)
            {
                FinishCrystal();
                canMove=false;
            }
        }

        if(canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,cd.radius);
        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Enemy>()!=null)
               hit.GetComponent<Enemy>().DamageEffect();
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    private void SelfDestroy()=>Destroy(gameObject);
}
