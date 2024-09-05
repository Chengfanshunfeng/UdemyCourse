using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage,CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    void Update()
    {
        if (!targetStats)
            return;

        if (triggered) 
            return;
            
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);//一个GameObject下的脚本公用一个transform.position
        transform.right = transform.position-targetStats.transform.position;//在动画物体中修改旋转的值，然后在父物体上旋转的值并未发生改变。这里再用该句将物体始终横向朝向目标

        if (Vector2.Distance(transform.position, targetStats.transform.position) <.1f)
        {
            anim.transform.localPosition = new Vector3(0,.5f);//爆炸前使动画组件稍稍上移使观感更好
            anim.transform.localRotation = Quaternion.identity;//重置动画组件的旋转
            transform.localRotation = Quaternion.identity;//重置本物体的旋转
            transform.localScale = new Vector3(3,3);//缩放本物体

            Invoke("DamageAndSelfDestory",.2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }

    }
    private void DamageAndSelfDestory()//爆炸时的方法
    {
            targetStats.ApplyShock(true);
            targetStats.TakeDamage(damage);
            Destroy(gameObject,.4f);

    }
}
