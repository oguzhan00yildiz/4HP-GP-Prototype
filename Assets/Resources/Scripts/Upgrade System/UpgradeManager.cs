using Global;
using PlayerLogic;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class UpgradeManager : MonoBehaviour
{
    public List<StatUpgrade> AvailableUpgrades { get; private set; }

    private int _numButtons;
    private List<UpgradeButton> _upgradeButtons; // (3) buttons for selecting upgrades.
    private GameObject _upgradePanel;

    private GameObject _upgradeButtonTemplate;

    public void Initialize()
    {
        var canvas = GameManager.CanvasManager.Canvas;

        _upgradePanel = canvas.transform.Find("UpgradeScreen").gameObject;

        _upgradeButtons = new List<UpgradeButton>(_numButtons);

        _numButtons = 3;

        _upgradeButtonTemplate =
            _upgradePanel.transform.Find("UpgradeButtonContainer/UpgradeButtonTemplate").gameObject;

        AvailableUpgrades = new List<StatUpgrade>();

        Transform buttonContainer = _upgradeButtonTemplate.transform.parent;

        for (int i = 0; i < _numButtons; i++)
        {
            // Instantiate button from template as child of container
            // (has horizontal layout group automation)
            var buttonObj = Instantiate(_upgradeButtonTemplate, buttonContainer);
            buttonObj.SetActive(true);

            // Creating new upgrade button class (sets itself up)
            var buttonClass = new UpgradeButton(buttonObj);

            // Adding to the list of buttons
            _upgradeButtons.Add(buttonClass);
        }

        // Add listener for each button's onClick event
        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            int buttonIndex = i;
            _upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(buttonIndex));
        }

        // Load contents from each folder into a list

        var generalUpgrades =
            Resources.LoadAll<StatUpgrade>("Scripts/Upgrade System/ScriptableObjects/General");

        var archerUpgrades =
            Resources.LoadAll<ArcherUpgrade>("Scripts/Upgrade System/ScriptableObjects/Archer");
        var tankUpgrades =
            Resources.LoadAll<TankUpgrade>("Scripts/Upgrade System/ScriptableObjects/Tank");

        int capacity = generalUpgrades.Length + archerUpgrades.Length + tankUpgrades.Length;
        List<StatUpgrade> upgrades = new List<StatUpgrade>(capacity);

        // Add each array to the list
        
        if (generalUpgrades.Length == 0 && GameManager.DebugMode)
        {
            Debug.LogWarning(
                "No general upgrades found" +
                " at Scripts/Upgrade System/ScriptableObjects/General");
        }
        else
            upgrades.AddRange(generalUpgrades);

        if (archerUpgrades.Length == 0 && GameManager.DebugMode)
        {
            Debug.LogWarning(
                "No archer upgrades found" +
                " at Scripts/Upgrade System/ScriptableObjects/Archer");
        }
        else
            upgrades.AddRange(archerUpgrades);

        if (tankUpgrades.Length == 0 && GameManager.DebugMode)
        {
            Debug.LogWarning(
                "No tank upgrades found" +
                " at Scripts/Upgrade System/ScriptableObjects/Tank");
        }
        else
            upgrades.AddRange(tankUpgrades);

        // If the folder in the above path is empty, stop
        if (upgrades.Count == 0 && GameManager.DebugMode)
        {
            Debug.LogError("Scripts/Upgrade System/ScriptableObjects is empty! No upgrades available");
            TogglePanel(false);
            return;
        }

        foreach (var upgrade in upgrades)
        {
            // Further check what kind of upgrade it is, and only add it to the list
            // if the character in use can "use" the upgrade
            var character = GameManager.Player.Character;
            switch (upgrade)
            {
                // If it's null, the scriptableObject found was not of type StatUpgrade,
                // continue
                case null:
                    Debug.LogError("HOLD ON! THIS SHOULD NEVER HAPPEN. TELL VILLE");
                    continue;
                // If found a tank upgrade and playing as one,
                // or if found an archer upgrade and playing as one,
                // add to the list
                case TankUpgrade when character == IPlayer.PlayerCharacter.Tank:
                case ArcherUpgrade when character == IPlayer.PlayerCharacter.Archer:
                    AvailableUpgrades.Add(upgrade);
                    break;
                // If found a general upgrade, add to the list
                case StatUpgrade:
                    AvailableUpgrades.Add(upgrade);
                    break;
            }
        }

        TogglePanel(false);
    }

    // Pop: get and remove from list
    private StatUpgrade[] PopRandomSuitableUpgrades(int count)
    {
        int index = 0;
        StatUpgrade[] randomUpgrades = new StatUpgrade[count];

        if (AvailableUpgrades.Count == 0)
            // ReSharper disable once UseArrayEmptyMethod
            return new StatUpgrade[0];

        if (count > AvailableUpgrades.Count)
            count = AvailableUpgrades.Count;

        while (count > 0)
        {
            // Get random index
            int randIndex = Random.Range(0, AvailableUpgrades.Count);

            // Get reference to random upgrade from list
            var randUpgrade = AvailableUpgrades[randIndex];

            // Assign to array and remove from list, if minimum wave has been reached.
            if (GameManager.EnemyManager.currentWave >= randUpgrade.MinimumWave)
            {
                randomUpgrades[index] = AvailableUpgrades[randIndex];
                AvailableUpgrades.RemoveAt(randIndex);
            }

            // Add check for type of player

            index++;
            count--;
        }

        return randomUpgrades;
    }

    private void RefreshAndShowPanel()
    {
        // Set each button up to have the correct info associated with the upgrade
        // First, pop random upgrades from the list, as many as we have buttons.
        var randUpgrades = PopRandomSuitableUpgrades(_upgradeButtons.Count);

        // If no more available upgrades, don't show panel.
        if (randUpgrades.Length == 0)
        {
            if(GameManager.DebugMode)
                Debug.LogWarning("No more available upgrades! Not showing upgrade panel.");

            GameManager.Instance.PlayerSetReady();
            return;
        }

        // Then set each button's info according to the upgrade chosen for it,
        // and allow interaction.
        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if(i >= randUpgrades.Length)
                break;

            if (randUpgrades[i] == null)
            {
                _upgradeButtons[i].SetActive(false);
                continue;
            }

            _upgradeButtons[i].SetInfoFromUpgrade(randUpgrades[i]);
            _upgradeButtons[i].interactable = true;
            _upgradeButtons[i].SetActive(true);
        }

        // Then show the panel.
        _upgradePanel.SetActive(true);
    }

    public void TogglePanel(bool value)
    {
        _upgradePanel.SetActive(value);
    }

    public void SelectUpgrade(int buttonIndex)
    {
        var upgrade = _upgradeButtons[buttonIndex].Upgrade;
        GameManager.Player.ReceiveUpgrade(upgrade);
        TogglePanel(false);
        GameManager.Instance.PlayerSetReady();
    }

    public void ShowUpgradePanel()
    {
        RefreshAndShowPanel();
    }
}
