using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CodeMonkey.Utils;
public class FollowMouse : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private LayerMask layer;
    private RectTransform myRectTransform;
    // Start is called before the first frame update
    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        FollowMouseLogic();
    }
    private void FollowMouseLogic()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        myRectTransform.anchoredPosition = anchoredPosition;
    }
}
