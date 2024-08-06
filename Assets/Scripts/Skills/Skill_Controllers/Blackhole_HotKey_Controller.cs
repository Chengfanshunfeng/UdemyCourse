using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;//用于按下键后使透明度变为0
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;//用于显示字母

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackHole;

    public void SetupHotKey(KeyCode _mynewHotKey,Transform _myEnemy,Blackhole_Skill_Controller _myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackhole;

        myHotKey = _mynewHotKey;
        myText.text = _mynewHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);//增加敌人的坐标

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
