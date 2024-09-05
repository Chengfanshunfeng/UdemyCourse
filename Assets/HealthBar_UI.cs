using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private CharacterStats mystats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        mystats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        mystats.OnHealthChanged += UpdateHealthUI;
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = mystats.GetMaxHealthValue();
        slider.value = mystats.currentHealth;
    }

    private void FlipUI()=>myTransform.Rotate(0f, 180f, 0f);//为了使角色翻转时血条ui也翻转

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        mystats.OnHealthChanged -= UpdateHealthUI;
    }
}
