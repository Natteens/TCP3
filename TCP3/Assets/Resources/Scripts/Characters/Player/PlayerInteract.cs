using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    [SerializeField] private PlayerInputs myInputs;
    public delegate void ItemCollectedHandler(string msg);

    [SerializeField] private Managers manager;

    public event ItemCollectedHandler ItemCollected;

    // Start is called before the first frame update
    void Start()
    {
        if(myInputs != null)
        myInputs.InteractAction.performed += _ => TryInteract();

        if (manager.m_ui != null)
        ItemCollected += manager.m_ui.OnItemCollected;

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

                if (obj.Giver() == true) 
                { 
                    manager.m_player.AddItem(obj.AddItem(), obj.ItemQuantity() );
                    manager.m_craft.UpdateRequirements();

                }

                if (ItemCollected != null)
                {

                    ItemCollected(name);
                }
            }
        }
        
    }
}
