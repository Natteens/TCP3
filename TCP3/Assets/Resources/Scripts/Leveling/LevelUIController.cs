using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : NetworkBehaviour
{
    public LevelManager manager;
    private Image xp;
    private TextMeshProUGUI nextLevelText, level, pointsToUp, cons, dest, sobre, sorte, details;
    private StatusComponent myStatus;

    // Valores de preview para os pontos alocados
    private int consPoints, destPoints, sobrePoints, sortePoints;

    public Button confirm;
    public Button increaseConsButton, decreaseConsButton;
    public Button increaseDestButton, decreaseDestButton;
    public Button increaseSobreButton, decreaseSobreButton;
    public Button increaseSorteButton, decreaseSorteButton;

    private void Start()
    {
        if (!IsOwner) return;

        manager.OnXpChanged += UpdateUI;

        xp = GameManager.Instance.xpBar;
        nextLevelText = GameManager.Instance.nextLevelText;
        level = GameManager.Instance.level;
        pointsToUp = GameManager.Instance.pointsToUp;
        cons = GameManager.Instance.cons;
        dest = GameManager.Instance.dest;
        sobre = GameManager.Instance.sobre;
        sorte = GameManager.Instance.sorte;
        details = GameManager.Instance.details;
        myStatus = GetComponent<StatusComponent>();

        confirm = GameManager.Instance.confirm;

        increaseConsButton = GameManager.Instance.increaseConsButton;
        decreaseConsButton = GameManager.Instance.decreaseConsButton;

        increaseDestButton = GameManager.Instance.increaseDestButton;
        decreaseDestButton = GameManager.Instance.decreaseDestButton;

        increaseSobreButton = GameManager.Instance.increaseSobreButton;
        decreaseSobreButton = GameManager.Instance.decreaseSobreButton;

        increaseSorteButton = GameManager.Instance.increaseSorteButton;
        decreaseSorteButton = GameManager.Instance.decreaseSorteButton;


        // Inicializa os pontos de preview
        consPoints = 0;
        destPoints = 0;
        sobrePoints = 0;
        sortePoints = 0;

        confirm.gameObject.SetActive(false);

        // Adiciona os listeners para os botões
        confirm.onClick.AddListener(() => ConfirmChanges());

        increaseConsButton.onClick.AddListener(() => IncreaseStat(StatusCategory.Constitution));
        decreaseConsButton.onClick.AddListener(() => DecreaseStat(StatusCategory.Constitution));

        increaseDestButton.onClick.AddListener(() => IncreaseStat(StatusCategory.Dexterity));
        decreaseDestButton.onClick.AddListener(() => DecreaseStat(StatusCategory.Dexterity));

        increaseSobreButton.onClick.AddListener(() => IncreaseStat(StatusCategory.Survival));
        decreaseSobreButton.onClick.AddListener(() => DecreaseStat(StatusCategory.Survival));

        increaseSorteButton.onClick.AddListener(() => IncreaseStat(StatusCategory.Luck));
        decreaseSorteButton.onClick.AddListener(() => DecreaseStat(StatusCategory.Luck));

        UpdateUI();
    }

    public void UpdateUI()
    {
        xp.fillAmount = manager.CurrentXp / manager.NextLevelXp;
        nextLevelText.text = $"{manager.CurrentXp} / {manager.NextLevelXp}";
        pointsToUp.text = manager.SkillPoints.ToString();

        cons.text = consPoints > 0 ? $"<color=green>{consPoints}</color>" : $"{consPoints}";
        dest.text = destPoints > 0 ? $"<color=green>{destPoints}</color>" : $"{destPoints}";
        sobre.text = sobrePoints > 0 ? $"<color=green>{sobrePoints}</color>" : $"{sobrePoints}";
        sorte.text = sortePoints > 0 ? $"<color=green>{sortePoints}</color>" : $"{sortePoints}";


        level.text = $"Nível {manager.Level}";

        details.text = $"Vida: {myStatus.GetStatus(StatusType.Health)} \n" +
               $"Defesa: {myStatus.GetStatus(StatusType.Defense)} \n" +
               $"Velocidade de Recarga: {myStatus.GetStatus(StatusType.CooldownReload)} \n" +
               $"Velocidade de Coleta: {myStatus.GetStatus(StatusType.GatheringSpeed)} \n" +
               $"Saciedade: {myStatus.GetStatus(StatusType.Satiaty)} \n" +
               $"Chance de Crítico: {myStatus.GetStatus(StatusType.CritChance)} \n" +
               $"Chance de Saque: {myStatus.GetStatus(StatusType.LootChance)} \n" +
               $"Velocidade de Movimento: {myStatus.GetStatus(StatusType.MoveSpeed)}";
    }

    public void IncreaseStat(StatusCategory category)
    {
        
        if (manager.SkillPoints <= 0) return;

        switch (category)
        {
            case StatusCategory.Constitution:
                 consPoints++;
                break;
            case StatusCategory.Survival:
                sobrePoints++;
                break;
            case StatusCategory.Dexterity:
                destPoints++;
                break;
            case StatusCategory.Luck:
                sortePoints++;
                break;
        }

        manager.SkillPoints--;
        UpdateUI();
        confirm.gameObject.SetActive(true);
    }

    public void DecreaseStat(StatusCategory category)
    {
        if (manager.SkillPoints <= 0) return;

        switch (category)
        {
            case StatusCategory.Constitution:
                if (consPoints > 0) consPoints--;
                break;
            case StatusCategory.Survival:
                if (sobrePoints > 0) sobrePoints--;
                break;
            case StatusCategory.Dexterity:
                if (destPoints > 0) destPoints--;
                break;
            case StatusCategory.Luck:
                if (sortePoints > 0) sortePoints--;
                break;
        }

        manager.SkillPoints++;
        UpdateUI();
        confirm.gameObject.SetActive(true);
    }

    public void ConfirmChanges()
    {
        // Aplica as mudanças aos status reais
        manager.ApplyStatusChanges(consPoints, destPoints, sobrePoints, sortePoints);

        // Atualiza a UI para refletir as alterações
        UpdateUI();
        confirm.gameObject.SetActive(false);
    }
}
