using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipScreenSpaceUI : MonoBehaviour
{
    public static TooltipScreenSpaceUI Instance { get; private set; }

    [SerializeField] private RectTransform canvasRectTransform;
    private RectTransform myRectTransform;
    private RectTransform background;
    private TextMeshProUGUI textMeshPro;
    [SerializeField] private Vector2 padding;
    private void Awake()
    {
        Instance = this;
        myRectTransform = transform.GetComponent<RectTransform>();
        background = transform.Find("background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("text").GetComponent<TextMeshProUGUI>();

        HideTooltip();
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        //Fix leaving on right side
        if (anchoredPosition.x + background.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - background.rect.width;
        }
        //Fix leaving on top side
        if (anchoredPosition.y + background.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - background.rect.height;
        }

        myRectTransform.anchoredPosition = anchoredPosition;
    }

    private void SetText(string tooltipText)
    { 
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        background.sizeDelta = textSize + padding;
    }

    private void ShowTooltip(string tooltipText)
    { 
        gameObject.SetActive(true);
        SetText(tooltipText);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipText)
    { 
        Instance.ShowTooltip(tooltipText);
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }

}
