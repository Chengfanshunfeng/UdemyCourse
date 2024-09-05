using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWondow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();


        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)//没有水晶时设置水晶
        {
            CreateCrystal();
        }
        else//有水晶时
        {
            if(canMoveToEnemy)//水晶在移动至怪物身边时不能进行换位
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {//换位，然后销毁水晶
                SkillManager.instance.clone.CreateClone(currentCrystal.transform,Vector3.zero); //克隆玩家分身进行攻击
                Destroy(currentCrystal);
            }
            else
            {//水晶爆炸
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();//销毁水晶的效果
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));//设置销毁时间

    }

    public void currentCrystalChooseRandomTarget()=> currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();
    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStacks)
        {
            if(crystalLeft.Count > 0)//储存空间有水晶时
            {
                if(crystalLeft.Count == amountOfStacks)//存满水晶时
                    Invoke("ResetAbility", useTimeWondow);//水晶是满的时候第一次按下，开始重置

                cooldown = 0;//在CanUseSkill中受控制，
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed,FindClosestEnemy(newCrystal.transform));
                
                if(crystalLeft.Count <= 0)//水晶储备不够时
                {
                    cooldown = multiStackCooldown;//给冷却赋值
                    RefilCrystal();
                }
            return true;
            }

        }

        return false;
    }

    private void RefilCrystal()//补充水晶
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for(int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
}
