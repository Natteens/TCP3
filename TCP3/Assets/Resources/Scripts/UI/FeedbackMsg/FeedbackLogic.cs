using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FeedbackLogic : MonoBehaviour
{
    public TextMeshProUGUI myText;
    public Image myImage;
    private float duration = 5f;
    private float moveDistance = 100f;

    private void Start()
    {
        UpLogic();
    }

    public void SetText(string txt)
    { 
        myText.text = txt;
    }

    public void SetSprite(Sprite spr)
    {
        myImage.sprite = spr;
    }

    public void RemoveMyImage()
    { 
        myImage.enabled = false;
    }

    public void UpLogic()
    {
        GetComponent<RectTransform>().DOAnchorPosY(myText.rectTransform.anchoredPosition.y + moveDistance, duration);
        myText.DOFade(0f, duration).OnComplete(() => myText.gameObject.SetActive(false));
    }
}
