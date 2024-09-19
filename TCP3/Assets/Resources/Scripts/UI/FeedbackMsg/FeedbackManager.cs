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
    }

    public void FeedbackItem(Item item)
    {
        if (item == null) return;

        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.SetText("x"+item.amount.ToString()+" "+item.itemName);
        logic.SetSprite(item.itemSprite);
    }

    public void FeedbackCraft(Craft craft)
    {
        if (craft == null) return;

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

    public IEnumerator FeedbackTextForRenewingResource()
    {
        yield return new WaitForSeconds(.5f);
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.RemoveMyImage();
        logic.SetText("O recurso est� sendo renovado!");
    }

    public IEnumerator FeedbackTextForNoResource()
    {
        yield return new WaitForSeconds(1f);
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.RemoveMyImage();
        logic.SetText("Recursos esgotados!");
    }

    public IEnumerator FeedbackTextForCancelColect()
    {
        yield return new WaitForSeconds(.3f);
        Transform instance = Instantiate(pfFeedbackMsg, transform);
        FeedbackLogic logic = Configure(instance);

        logic.RemoveMyImage();
        logic.SetText("Coleta Cancelada!");
    }

    private FeedbackLogic Configure(Transform instance)
    {
        FeedbackLogic logic = instance.AddComponent<FeedbackLogic>();
        logic.myImage = instance.GetComponentInChildren<Image>();
        logic.myText = instance.GetComponentInChildren<TextMeshProUGUI>();

        return logic;
    }




}
