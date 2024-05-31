using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;
using Mono.CSharp.yyParser;
using UnityEngine.InputSystem;
using Mono.CSharp;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Managers manager;

    [Header("Status Base")]
    [Space(10)]
    public StatusBase StatusBase;
    public XPTracker xpTracker;


    [Header("Eventos")]
    [Space(10)]
    public UnityEvent onRunStart;
    public UnityEvent onRegenLife;
    public UnityEvent onRegenStamina;

    // Controle
    #region Controls
    private float UltimaPosicaoEmY, DistanciaDeQueda;
    public float AlturaQueda = 6, DanoPorMetro = 5;
    public Image BarraVida, BarraEstamina, BarraFome;
    public float FomeCheia => StatusBase.playerAbilities.FomeMax.GetValue(StatusBase.status);
    public float VidaCheia => StatusBase.playerAbilities.VidaMax.GetValue(StatusBase.status);
    public float EstaminaCheia => StatusBase.playerAbilities.VidaMax.GetValue(StatusBase.status);
    public float WalkSpeed => StatusBase.playerAbilities.MoveSpeed.GetValue(StatusBase.status);
    public float RunSpeed => StatusBase.playerAbilities.RunSpeed.GetValue(StatusBase.status);
    public float JumpHeight => StatusBase.playerAbilities.JumpHeight.GetValue(StatusBase.status);
    public float VidaAtual, EstaminaAtual, FomeAtual;
    public float velocidadeEstamina = 250;
    private CharacterController controlador;
    private bool semEstamina = false;
    private float cronometroFome;
    private float multEuler;

    public float lfVFX = 1f;
    #endregion

    [SerializeField] private GameObject vfxLand;
    [SerializeField] Transform spawnVFX;
    [SerializeField] private PlayerInputs myInputs;

    //Inventario
    [SerializeField] private List<BaseItem> inventory;
    [SerializeField] private List<GameObject> slots;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private GameObject inventoryGO;

    //DEBUG
    [SerializeField] private bool inventoryGObool = false;

    public List<BaseItem> Inventory {  get { return inventory; } }

    private void Start()
    {
        controlador = GetComponent<CharacterController>();

        VidaAtual = VidaCheia;
        EstaminaAtual = EstaminaCheia;
        FomeAtual = FomeCheia;

        multEuler = ((1 / EstaminaCheia) * EstaminaAtual) * ((1 / FomeCheia) * FomeAtual);
    }

    void Update()
    {
        if (IsOwner)
        {
            SistemaDeQueda();
            SistemaDeVida();
            SistemaDeEstamina();
            SistemaDeFome();
            AplicarBarras();

            //DEBUG
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryGObool = !inventoryGObool;
            }

            inventoryGO.SetActive(inventoryGObool);

            switch (inventoryGObool)
            {
                case true:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case false:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
            }
            //DEBUG
        }
    }

    #region Item Methods
    public void AddItem(BaseItem item, int quantity)
    {
        if (item == null) return;

        //Não tenho item
        if (!inventory.Contains(item))
        {
            GameObject _slot = CreateSlot();
            ItemHolder _slotItemHolder = _slot.GetComponent<ItemHolder>();
            _slotItemHolder.UpdateItem(item, quantity);
            _slotItemHolder.SetManager(manager);
            slots.Add(_slot);

            for (int i = 0; i < quantity; i++)
            {
                inventory.Add(item);
            }

            return;
        }

        //Ja Tenho item
        foreach (GameObject slot in slots)
        {
            if (slot == null) return;

            ItemHolder _slot = slot.GetComponent<ItemHolder>();
            BaseItem _item = _slot.item;
            int newQuantity = quantity + _slot.quantity;

            if (_item == item)
            { 
              _slot.UpdateItem(item, newQuantity);
            }
        }
            
    }

    public void RemoveItem(BaseItem item, int quantity)
    {
        if (item == null) return;

        List<GameObject> slotsToRemove = new List<GameObject>(); // Lista temporária para armazenar slots a serem removidos
        int quantityToRemove = quantity; // Quantidade de itens a serem removidos

        for (int i = 0; i < quantityToRemove; i++)
        {
            if (inventory.Contains(item))
            {
                foreach (GameObject slot in slots)
                {
                    if (slot == null)
                    {
                        Debug.Log("Slot nulo!");
                        return;
                    }

                    ItemHolder _slot = slot.GetComponent<ItemHolder>();
                    BaseItem _item = _slot.item;

                    if (_item == item)
                    {
                        int newQuantity = _slot.quantity - quantity;
                        _slot.UpdateItem(item, newQuantity);

                        if (newQuantity <= 0)
                        {
                            slotsToRemove.Add(slot);
                            inventory.Remove(item);
                            break; // Saia do loop atual para evitar modificação durante a iteração
                        }      
                    }
                }
            }
        }

        // Remover slots após a iteração
        foreach (GameObject slotToRemove in slotsToRemove)
        {
            slots.Remove(slotToRemove);
            Destroy(slotToRemove);
        }
    }


    public GameObject CreateSlot()
    {
        GameObject _slot = Instantiate(slotPrefab, inventoryContent);
        return _slot;
    }

    public int ReturnItemQuantity(BaseItem item) 
    {
        foreach  (BaseItem _i in inventory)
        {
            //Find specific item
            if (_i == item)
            {
                //Search specific item quantity
                foreach (GameObject slot in slots)
                {
                    ItemHolder _slot = slot.GetComponent<ItemHolder>();

                    if (_slot.item == _i) return _slot.quantity;
                }
            }
        }

        return -1;
    }

    public int GetItemCount(BaseItem _item) 
    {
        int quantidadeNoInventario = inventory.FindAll(item => item == _item).Count;

        if (quantidadeNoInventario > 0)
        {
            return ReturnItemQuantity(_item);
        }

        return 0;
    }
    #endregion

    #region Stats Methods
    void SistemaDeQueda()
    {
        if (UltimaPosicaoEmY > transform.position.y && controlador.velocity.y < 0)
        {
            DistanciaDeQueda += UltimaPosicaoEmY - transform.position.y;
        }
        UltimaPosicaoEmY = transform.position.y;
        if (DistanciaDeQueda >= AlturaQueda && controlador.isGrounded)
        {
            VidaAtual = VidaAtual - DanoPorMetro * DistanciaDeQueda;
            DistanciaDeQueda = 0;
            UltimaPosicaoEmY = 0;
            VFXManager.Instance.PlayVFX(vfxLand, spawnVFX.position, spawnVFX.rotation, lfVFX);
        }
        if (DistanciaDeQueda < AlturaQueda && controlador.isGrounded)
        {
            DistanciaDeQueda = 0;
            UltimaPosicaoEmY = 0;
        }
    }
    
    void SistemaDeFome()
    {
        FomeAtual -= Time.deltaTime;
        if (FomeAtual >= FomeCheia)
        {
            FomeAtual = FomeCheia;
        }
        if (FomeAtual <= 0)
        {
            FomeAtual = 0;
            cronometroFome += Time.deltaTime;
            if (cronometroFome >= 3)
            {
                VidaAtual -= (VidaCheia * 0.25f);
                EstaminaAtual -= (EstaminaCheia * 0.1f);
                cronometroFome = 0;
            }
        }
        else
        {
            cronometroFome = 0;
        }
    }

    void SistemaDeEstamina()
    {
        if (EstaminaAtual >= EstaminaCheia)
        {
            EstaminaAtual = EstaminaCheia;
        }
        else
        {
            EstaminaAtual += Time.deltaTime * (velocidadeEstamina / 40) * Mathf.Pow(2.718f, multEuler);
        }
        if (EstaminaAtual <= 0)
        {
            EstaminaAtual = 0;
            semEstamina = true;
        }
        if (semEstamina && EstaminaAtual >= (EstaminaCheia * 0.15f))
        {
            semEstamina = false;
        }
    }

    void SistemaDeVida()
    {
        if (VidaAtual >= VidaCheia)
        {
            VidaAtual = VidaCheia;
        }
        //else if (VidaAtual < VidaCheia && VidaAtual > 0)
        //{
        //    RegenHP();
        //}
        else if (VidaAtual <= 0)
        {
            VidaAtual = 0;
            Morreu();
        }
    }

    void AplicarBarras()
    {
        BarraVida.fillAmount = ((1 / VidaCheia) * VidaAtual);
        BarraEstamina.fillAmount = ((1 / EstaminaCheia) * EstaminaAtual);
        BarraFome.fillAmount = ((1 / FomeCheia) * FomeAtual);
    }

    void Morreu()
    {
        Debug.Log("Morreu por falta de comida");
    }

    public void RunStart()
    {
        onRunStart.Invoke();
    }

    public void UseStamina()
    {
        EstaminaAtual -= Time.deltaTime * (velocidadeEstamina / 15) * Mathf.Pow(2.718f, multEuler);
    }
    
    public void Regen()
    {
        onRegenLife.Invoke();
    }
    #endregion
}
