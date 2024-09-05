using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;
    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;//在flashDuration期间改变物体材质
        Color currentColor = sr.color;//为了能让骷髅短暂的变为白色
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;//重置材质
    }

    private void RedColorBlink()
    {
        if(sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;

    }
    private void CancelColorChange()
    {
        CancelInvoke();//终止所有的调用
        sr.color = Color.white;
    }

    public void IgniteFxFor(float _seconds)//燃烧颜色转换
    {
        InvokeRepeating("IgniteColorFx", 0, .3f);//启动颜色变换效果
        Invoke("CancelColorChange", _seconds);//_seconds秒后取消颜色变换
        //InvokeRepeating和Invoke区别在于，前者会反复调用
    }

    public void ChillFxFor(float _seconds)//冰冻颜色转换
    {
        InvokeRepeating("ChillColorFx", 0, .3f);//启动颜色变换效果
        Invoke("CancelColorChange", _seconds);//_seconds秒后取消颜色变换
    }

    public void ShockFxFor(float _seconds)//麻痹颜色转换
    {
        InvokeRepeating("ShockColorFx", 0, .3f);//启动颜色变换效果
        Invoke("CancelColorChange", _seconds);//_seconds秒后取消颜色变换
    }

    private void IgniteColorFx()//使物体的颜色在两种预设颜色之间交替变换
    {
        if(sr.color!=igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ChillColorFx()//冻结颜色
    {
        if(sr.color!=chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }
    
    private void ShockColorFx()//麻痹效果的颜色转换
    {
        if(sr.color!=shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }
}
