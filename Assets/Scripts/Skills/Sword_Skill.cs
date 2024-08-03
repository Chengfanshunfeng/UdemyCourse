using System;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]//弹跳飞刀信息
    [SerializeField] private int bounceAmount;//弹跳次数
    [SerializeField] private float bounceGravity;//弹跳飞刀重力
    [SerializeField] private float bounceSpeed;

    [Header("Peirce info")]//穿透飞刀信息
    [SerializeField] private int pierceAmount;//穿透数量
    [SerializeField] private float pierceGravity;//穿透飞刀重力

    [Header("Spin info")]//旋转飞刀信息
    [SerializeField] private float hitCooldown = 0.35f;//攻击频率
    [SerializeField] private float maxTravelDistance = 7;//旋转飞刀最大旋转距离
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetweenDots;
    [SerializeField] private GameObject dotPrefeb;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;
    protected override void Start()
    {
        base.Start();

        GenereateDots();

        SetupGravity();
    }

    private void SetupGravity()
    {
        if(swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if(swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if(swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AnimDirection().normalized.x*launchForce.x,AnimDirection().normalized.y*launchForce.y);
        
        if(Input.GetKey(KeyCode.Mouse1))
        {
            for(int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i*spaceBeetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab,player.transform.position,transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if(swordType == SwordType.Bounce)//为飞刀选择弹跳属性时，修改飞刀属性
            newSwordScript.SetupBounce(true,bounceAmount,bounceSpeed);
        else if(swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if(swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true,maxTravelDistance,spinDuration,hitCooldown);



        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration,returnSpeed);//该方法中包含了超时销毁

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim region
    public Vector2 AnimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//鼠标位置
        Vector2 direction = mousePosition - playerPosition;
        return direction;//得到了鼠标位置减去玩家位置的向量
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()//白点一直都有，只是进入瞄准状态才激活
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            //将预制体生成后，再将其加入数组中，最后将一开始生成的预制体设置为非激活
            dots[i] = Instantiate(dotPrefeb,player.transform.position,Quaternion.identity,dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AnimDirection().normalized.x * launchForce.x ,
            AnimDirection().normalized.y * launchForce.y)*t +.5f*(Physics2D.gravity*swordGravity)*(t*t);

        return position;
    }

    #endregion
}
