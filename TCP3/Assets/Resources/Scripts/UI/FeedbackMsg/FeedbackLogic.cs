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
    private float duration = 2f;
    private float moveDistance = 200f;

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
        myImage.DOFade(0f, duration);
        myText.DOFade(0f, duration).OnComplete(() => Destroy(gameObject));
    }
}
