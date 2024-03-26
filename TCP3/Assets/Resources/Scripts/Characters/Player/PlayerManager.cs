using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
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
        }     
    }

    // Status
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



 
}
