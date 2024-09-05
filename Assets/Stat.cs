using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()//基础值加列表中的值计算最终值
    {
        int finalValue = baseValue;

        foreach (int modifer in modifiers)//运用遍历依次添加列表当中的buff
            finalValue += modifer;
            
        return finalValue;
    }

    public void SetDefaultValue(int _value)//设置基础值
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)//加buff到列表
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)//减buff到列表
    {
        modifiers.RemoveAt(_modifier);
    }
}
