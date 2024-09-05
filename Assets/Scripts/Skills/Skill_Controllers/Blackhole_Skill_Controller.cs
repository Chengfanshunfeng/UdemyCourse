using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;//热键预制体
    [SerializeField] private List<KeyCode> keyCodeList;//按键列表用List<KeyCode>

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;//是否可以执行克隆攻击
    private bool playerCanDisapear=true;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState { get;private set; }

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown,float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;

        if(SkillManager.instance.clone.crystalInseadOfClone)//黑洞技能如果是释放水晶则玩家不会消失
            playerCanDisapear = false;
    }
    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if(blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;//设置为无限后在设置黑洞方法后才会小于零

            if(targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)//黑洞变大
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);//使用Lerp使黑洞的变化呈线性状态
        }

        if (canShrink)//黑洞变小
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }

    }

    private void ReleaseCloneAttack()//释放克隆攻击
    {
        if(targets.Count <= 0)//没有敌人不释放攻击
            return;

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        if(playerCanDisapear)//玩家消失处理
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if (Random.Range(0, 100) > 50)//引用随机数设置偏移
                xOffset = 2;
            else
                xOffset = -2;

            if(SkillManager.instance.clone.crystalInseadOfClone)//创造水晶攻击
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.currentCrystalChooseRandomTarget();//在另一个脚本中修改了通过CreateCrystal()修改的closestTarget
            }
            else//创造分身攻击
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility",1f);//在一秒后执行
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;//黑洞变小
        cloneAttackReleased = false;

    }

    private void DestroyHotKeys()//热键销毁
    {
        if(createdHotKey.Count <= 0)
            return;
        for(int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!=null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);//冻结敌人

            CreateHotKey(collision);//生成热键
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!=null)
            collision.GetComponent<Enemy>().FreezeTime(false);//解冻敌人
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("没有热键了");
            return;
        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);//生成热键（能显示）
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];//从keyCodeList列表中随机选择一个KeyCode
        keyCodeList.Remove(choosenKey);//从keyCodeList中移除选中的KeyCode，确保不会重复使用

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();//获取热键控制器脚本

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);//重新给刚刚生成的热键赋值
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
