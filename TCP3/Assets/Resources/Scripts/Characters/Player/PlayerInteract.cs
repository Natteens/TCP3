using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    [SerializeField] private PlayerInputs myInputs;
    public delegate void ItemCollectedHandler(string msg);

    [SerializeField] private UXUIManager uiManager;

    public event ItemCollectedHandler ItemCollected;

    // Start is called before the first frame update
    void Start()
    {
        if(myInputs != null)
        myInputs.InteractAction.performed += _ => TryInteract();

        if (uiManager != null)
        ItemCollected += uiManager.OnItemCollected;

    }

    private void OnDisable()
    {
        if (myInputs != null)
        myInputs.InteractAction.performed -= _ => TryInteract();
    }

    private void TryInteract()
    {
        Ray ray = GameUtils.CenterOfScreenRay();

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        { 
            Interactable obj = hit.transform.gameObject.GetComponent<Interactable>();

            if (obj != null)
            {
                string name = obj.OnInteract();

                if (ItemCollected != null)
                {
                    ItemCollected(name);
                }
            }
        }
        
    }
}
