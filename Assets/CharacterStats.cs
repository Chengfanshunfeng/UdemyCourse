using System.Security.Principal;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;//每一点提升力量
    public Stat agility;//每一点提升敏捷
    public Stat intelligence;//每一点提升法术伤害和法术抗性
    public Stat vitality;//每一点提升生命值

    [Header("Offensive stats")]
    public Stat damage;//对敌人造成的伤害
    public Stat critChance;//暴击率
    public Stat critPower;//暴击伤害

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;//护甲
    public Stat evasion;//闪避值
    public Stat magicResistance;//魔法抗性

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;//是否被点燃
    public bool isChilled;//是否被冰冻
    public bool isShocked;//是否被闪电

    [SerializeField] private float ailmentsDuration = 4f;
    private float ignitedTimer;//点燃持续时间
    private float chilledTimer;
    private float shockedTimer;

    private float ailmentTimer;//症状持续时间，可以用来控制总体状态，暂时不用

    private float igniteDamageCoodlown = .3f;//一段燃烧伤害执行后的冷却
    private float igniteDamageTimer;
    private int igniteDamage;//点燃伤害
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth;

    public System.Action OnHealthChanged;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);//设置基础暴击倍率
        currentHealth = GetMaxHealthValue();//设置生命值

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;//三种状态持续逐渐消失
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)//燃烧时间到了取消状态
            isIgnited = false;
        if (chilledTimer < 0)//冰冻时间到了取消状态
            isChilled = false;
        if (shockedTimer < 0)//惊恐时间到了取消状态
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnited)//单次燃烧伤害到了并还处于燃烧状态
        {
            //currentHealth -= igniteDamage;//执行燃烧伤害的削减，帧伤
            DecreaseHealth(igniteDamage);

            Debug.Log(igniteDamage);
            if (currentHealth < 0)
                Die();

            igniteDamageTimer = igniteDamageCoodlown;//单次伤害时间到了
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)//造成的伤害在此处实行buff的加减
    {
        if (TargetCanAvoidAttack(_targetStats))//判断是否闪避
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();//初始受到的伤害

        if (CanCrit())//判断是否暴击
        {
            totalDamage = CalculateCriticalDamage(totalDamage);//计算暴击伤害
            Debug.Log("Critical Hit!" + totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);//受到的伤害减去护甲值
        //_targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();//魔法伤害总和为三种魔法加上智力值

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)//如果没有伤害就无法退出下面的while循环
            return;

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;//根据三个魔法伤害的大小确定附加状态
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)//当三种魔法伤害一样时想附加状态可使用此方法
        {
            if (Random.value < 0.33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 0.5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 1f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }
        if (canApplyIgnite)//如果可以附加燃烧则设置燃烧伤害为火焰伤害的20%
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)//计算受到是的实际法术伤害
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);//将魔法伤害减去魔法抗性和智力的三倍
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)//附加状态方法，如果没有状态则附加
    {
        // if(isIgnited||isChilled||isShocked)
        //     return;

        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;//电流反弹，额外效果

        if (_ignite && canApplyIgnite)//进入燃烧状态，设置燃烧时间
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);//触发两秒改变颜色
        }
        if (_chill && canApplyChill)//进入冰冻状态，设置冰冻时间
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock && canApplyShock)//进入惊吓状态，设置惊吓时间
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if(isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closeseDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);
                if (distanceToEnemy < closeseDistance)
                {
                    closeseDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);

            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;//将计算好的燃烧伤害赋值

    public void SetupShockDamage(int _damage) => shockDamage = _damage;//将计算好的惊吓伤害赋值

    public virtual void TakeDamage(int _damage)//实行经过buff加减后受到的伤害
    {
        DecreaseHealth(_damage);

        Debug.Log(_damage);

        if (currentHealth < 0)
            Die();
    }

    protected virtual void DecreaseHealth(int _damage)//一个血量减少函数，用于触发委托
    {
        currentHealth -= _damage;

        if (OnHealthChanged != null)
            OnHealthChanged();
    }

    protected virtual void Die()
    {

    }
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)//受到的伤害减去护甲
    {
        if (_targetStats.isChilled)//受到伤害时如果处于冰冻状态
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);//护甲效果将为原来80%四舍五入取整
        else
            totalDamage -= _targetStats.armor.GetValue();//受到的伤害减去护甲
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);//防止伤害为负数
        return totalDamage;
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)//判断是否闪避
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)//角色处于惊吓状态时闪避概率增加
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    private bool CanCrit()//判断是否暴击
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)//计算暴击伤害
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;//计算暴击倍率

        float critDamage = _damage * totalCritPower;//计算暴击后的伤害

        return Mathf.RoundToInt(critDamage);//将伤害类型转换为整数类型
    }

    public int GetMaxHealthValue()//获得最大生命值，通过基础最大加上体质点数乘5
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
}
