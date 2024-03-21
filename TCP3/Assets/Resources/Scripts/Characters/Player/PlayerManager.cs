using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [Header("Status Base")]
    public StatusBase statusBase;
    public XPTracker xpTracker;

    [Header("Eventos")]

    public UnityEvent onLevelUp;
    public UnityEvent onRunStart;

    // Valores apos o calculo dos stats
    #region CURRENT STATS

    //Constituição
    private float currentRegenerationHP = 0.2f;
    private float currentRegenerationVigor = 0.2f;
    private float currentReductionFome = 0f;
    private float currentReductionDamage = 0f;

    // Força     
    private float currentMeleeDamage = 10f;

    //Agilidade   
    private float currentReductionRecargaRanged = 0.5f;
    private float currentReductionCustoVigor = 0.5f;
    private float currentMoveSpeed = 2f;
    private float currentSprintSpeed = 5.335f;
    private float currentJumpHeight = 1.2f;

    // Precisao   
    private float currentRangedDamage = 20f;

    // Sorte      
    private float currentLoot = 0f;


    #endregion

    // ------- 
    // Controle
    private CharacterController controlador;
    private float UltimaPosicaoEmY, DistanciaDeQueda;
    public float AlturaQueda = 6, DanoPorMetro = 5;
    public Image BarraVida, BarraEstamina, BarraFome;
    public float VidaCheia = 100, EstaminaCheia = 100, FomeCheia = 100, velocidadeEstamina = 250;
    public float VidaAtual, EstaminaAtual, FomeAtual;
    private bool semEstamina = false;
    private float cronometroFome;
    private float multEuler;


    private void Start()
    {
        controlador = GetComponent<CharacterController>();
        InitHUD();

        VidaAtual = VidaCheia;
        EstaminaAtual = EstaminaCheia;
        FomeAtual = FomeCheia;

        multEuler = ((1 / EstaminaCheia) * EstaminaAtual) * ((1 / FomeCheia) * FomeAtual);
    }


    void Update()
    {
        SistemaDeQueda();
        SistemaDeVida();
        SistemaDeEstamina();
        SistemaDeFome();
        AplicarBarras();
    }

    private void InitHUD()
    {
        var hud = GameObject.Find("PlayerHUD");
        BarraVida = hud.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        BarraEstamina = hud.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        BarraFome = hud.transform.GetChild(2).GetChild(1).GetComponent<Image>();   
    }

    public void LevelUpgrade()
    {
        onLevelUp.Invoke();
    }

    public void RunStart()
    {
        onRunStart.Invoke();
    }

    #region GetterSkills
    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }

    public float GetCurrentSprintSpeed()
    {
        return currentSprintSpeed;
    }

    public float GetCurrentJumpHeight()
    {
        return currentJumpHeight;
    }
    #endregion

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
        else if (VidaAtual < VidaCheia && VidaAtual > 0)
        {
            RegenHP();
        }
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

    public void UseStamina()
    {
        EstaminaAtual -= Time.deltaTime * (velocidadeEstamina / 15) * Mathf.Pow(2.718f, multEuler);
    }
    public void RegenHP()
    {
        VidaAtual += Time.deltaTime *( currentRegenerationHP / 15 )* Mathf.Pow(2.718f, multEuler);
    }

}
