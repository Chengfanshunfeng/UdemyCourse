using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;//热键预制体
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public bool canGrow;

    public int amountOfAttacks = 4;

    private List<Transform> targets = new List<Transform>();

    private void Update()
    {
        if(canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
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

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("没有热键了");
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);//生成热键（能显示）

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];//从keyCodeList列表中随机选择一个KeyCode
        keyCodeList.Remove(choosenKey);//从keyCodeList中移除选中的KeyCode，确保不会重复使用

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();//获取热键控制器脚本

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);//重新给刚刚生成的热键赋值
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
