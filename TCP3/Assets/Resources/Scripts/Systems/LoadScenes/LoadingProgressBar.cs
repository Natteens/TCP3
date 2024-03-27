using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingProgressBar : MonoBehaviour
{
    private Image image;
    private TextMeshProUGUI text;

    private void Awake()
    { 
        image = transform.GetComponent<Image>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        image.fillAmount = Loader.GetLoadingProgress();
        text.text = (Loader.GetLoadingProgress() * 100).ToString() + "%";
    } 
}
