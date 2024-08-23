using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }
    [SerializeField] private Transform pfFeedbackMsg;


    private void Awake()
    {
        Instance = this;
        FeedbackText("CabeloDoSerpa");
    }

    public void FeedbackItem(Item item)
    {
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.SetText("x"+item.amount.ToString()+" "+item.itemName);
        logic.SetSprite(item.itemSprite);
    }

    public void FeedbackCraft(Craft craft)
    {
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.SetText("Nova Receita: " + craft.outputItem.itemName);
        logic.SetSprite(craft.outputItem.itemSprite);
    }

    public void FeedbackText(string txt)
    {
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.RemoveMyImage();
        logic.SetText(txt);
    }

    private FeedbackLogic Configure(Transform instance)
    {
        FeedbackLogic logic = instance.AddComponent<FeedbackLogic>();
        logic.myImage = instance.GetComponentInChildren<Image>();
        logic.myText = instance.GetComponentInChildren<TextMeshProUGUI>();

        return logic;
    }




}
