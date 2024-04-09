using Sirenix.OdinInspector;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DummyTrainer : MonoBehaviour, Damageable
{
    [BoxGroup("Setup Dummy Trainer")]public int maxHealth = 100;
    [BoxGroup("Setup Dummy Trainer")]public int currentHealth;
    [BoxGroup("Setup Dummy Trainer")]public float bounceDuration = 0.5f;
    [BoxGroup("Setup Dummy Trainer")]public float bounceProgress = 0.0f; 
    [BoxGroup("Setup Dummy Trainer")]public Image healthBar;
    [BoxGroup("Setup Dummy Trainer")]public GameObject target;

    private float healthRecoveryRate = 1.0f;
    private float healthRecoveryCounter = 0.0f;
    private bool isBouncing = false;
    private bool isDead = false;
    private Vector3 originalPosition;

    private void Start()
    {
        currentHealth = maxHealth;
        originalPosition = target.transform.position;
        UpdateHealthBar();
    }

    private void Update()
    {
        HPregeneration();
        ImpactBounce();
        UpdateHealthBar();
    }

    private void HPregeneration()
    {
        if (!isDead && currentHealth < maxHealth)
        {
            healthRecoveryCounter += Time.deltaTime;
            if (healthRecoveryCounter >= healthRecoveryRate)
            {
                currentHealth++;
                healthRecoveryCounter = 0.0f;
            }
        }
    }

    private void ImpactBounce()
    {
        if (isBouncing)
        {
            bounceProgress += Time.deltaTime / bounceDuration;
            target.transform.position = Vector3.Lerp(originalPosition + Vector3.back, originalPosition, bounceProgress);

            if (bounceProgress >= 1.0f)
            {
                isBouncing = false;
                bounceProgress = 0.0f;
            }
        }
    }

    public void ApplyDamage(int amount)
    {
        if (!isDead)
        {
            currentHealth -= amount;
            isBouncing = true;
            if (currentHealth <= 0)
            {
                StartCoroutine(Die()); 
            }
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        currentHealth = 0; 

        yield return new WaitForSeconds(1);

        isDead = false;
        currentHealth = maxHealth;
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;

    }
}
