using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;
    private float timeLaft;

    Image healthSlider;
    Transform UIbar;

    //面向摄像机
    Transform cam;
    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.updateHealthBarOnAttack += UpdateHealthBar;
    }
    private void OnEnable()
    {
        cam = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode==RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth<=0)
        {
            Destroy(UIbar.gameObject);
        }
        UIbar.gameObject.SetActive(true);
        timeLaft = visibleTime;
        
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar!=null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;
            if (timeLaft<=0&&!alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);

            }
            else
            {
                timeLaft -= Time.deltaTime;
            }
        }
    }
}
