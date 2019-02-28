using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
            Debug.Log("STATUS INDICATOR: No health bar object!");
        if (healthText == null)
            Debug.Log("STATUS INDICATOR: No health text object!");
    }

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;
        healthBar.localScale = new Vector3(_value, healthBar.localScale.y, healthBar.localScale.z);
        healthText.text = _cur + "/" + _max + " HP";

        Image image = healthBar.GetComponent<Image>();
        if (_cur <= 0.25 * _max)
            image.color = new Color(255, 0, 0);  //Health Bar color = red if health = 25% of max
        else if (_cur <= 0.5 * _max)
            image.color = new Color(229, 207, 0); //Health Bar color = yellow if health = 50% of max﻿
    }
}
