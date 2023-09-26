using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using PlayerLogic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button[] upgradeButtons; // 3 buttons for selecting upgrades.
    [SerializeField] private GameObject upgradePanel;

    private UpgradeType selectedUpgrade = UpgradeType.None;

    private enum UpgradeType
    {
        None,
        ShootingAbility,
        Ability2,
        Ability3,
        Ability4,
        Ability5,
        Ability6,
        // Other abilities can be added here
    }

    private void Start()
    {
      
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int buttonIndex = i; 
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(buttonIndex));
        }

        // When a wave is completed, randomly select new upgrades and assign them to buttons.
        WaveCompleted();
    }

    private void Update()
    {
        if (selectedUpgrade==UpgradeType.ShootingAbility)
        {
            PlayerAttackHandler.instance.Shoot();
        }
    }

    public void WaveCompleted()
    {
        // Randomly select 3 upgrades.
        UpgradeType[] randomUpgrades = GetRandomUpgrades(3);

        
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = randomUpgrades[i].ToString();
            }
            upgradeButtons[i].interactable = true;
        }

        if (randomUpgrades.Length > 0)
        {
            infoText.text = "Please select an upgrade:";
        }
        else
        {
            infoText.text = "All upgrades have been used!";
        }
    }

    public void SelectUpgrade(int buttonIndex)
    {
        string upgradeName = upgradeButtons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        selectedUpgrade = (UpgradeType)System.Enum.Parse(typeof(UpgradeType), upgradeName);
        upgradePanel.SetActive(false);
        WaveCompleted();
    }

    private UpgradeType[] GetRandomUpgrades(int count)
    {
        // Create a list containing all upgrades.
        List<UpgradeType> allUpgrades = new List<UpgradeType>();
        foreach (UpgradeType upgrade in System.Enum.GetValues(typeof(UpgradeType)))
        {
            if (upgrade != UpgradeType.None)
            {
                allUpgrades.Add(upgrade);
            }
        }

        // Select a random number of upgrades as specified by 'count'.
        List<UpgradeType> randomUpgrades = new List<UpgradeType>();
        while (randomUpgrades.Count < count && allUpgrades.Count > 0)
        {
            int randomIndex = Random.Range(0, allUpgrades.Count);
            randomUpgrades.Add(allUpgrades[randomIndex]);
            allUpgrades.RemoveAt(randomIndex);
        }

        return randomUpgrades.ToArray();
    }
}
