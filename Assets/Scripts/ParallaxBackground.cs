using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float xPosition;
    private float length;
    void Start()
    {
        //会寻找当前场景下名为Main Camera的组件，若有多个则返回第一个寻找到的，子父节点不影响寻找
        cam = GameObject.Find("Main Camera");
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        //直接打transform说明这是脚本搭载对象的transform
        xPosition = transform.position.x;
    }
    void Update()
    {
        float distanceMoved = cam.transform.position.x *(1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if(distanceMoved > xPosition + length)
            xPosition = xPosition + length;

        else if(distanceMoved < xPosition - length)
            xPosition = xPosition - length;
    }
}
