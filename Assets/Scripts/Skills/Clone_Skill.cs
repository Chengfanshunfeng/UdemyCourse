using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool creatCloneOnDashStart;//克隆在冲刺开始时
    [SerializeField] private bool creatCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;//克隆在反击时

    [Header("Clone can duplicate")]
    [SerializeField] private bool canDuplicateClone;//幻影攻击能否创造幻影
    [SerializeField] private float chanceToDuplicate;//反击时克隆能否创造幻影
    [Header("Crystal instead of clone")]
    public bool crystalInseadOfClone;

    public void CreateClone(Transform _clonePosition,Vector3 _offset)//只有黑洞技能发出的水晶是随机的
    {
        if(crystalInseadOfClone)//制造水晶代替克隆
        {
            SkillManager.instance.crystal.CreateCrystal();
            //SkillManager.instance.crystal.currentCrystalChooseRandomTarget();//在另一个脚本中修改了通过CreateCrystal()修改的closestTarget
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone,chanceToDuplicate);
    }
    
    public void CreateCloneOnDashStart()
    {
        if(creatCloneOnDashStart)
            CreateClone(player.transform,Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if(creatCloneOnDashOver)
            CreateClone(player.transform,Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if(canCreateCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform,new Vector3(2*player.facingDir,0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform,Vector3 _offset)
    {
        yield return new WaitForSeconds(0.5f);
            CreateClone(_transform,_offset);
    }
}
