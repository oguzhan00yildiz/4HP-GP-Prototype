using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using PlayerLogic;
using System;
using Random = UnityEngine.Random;
using Global;

public partial class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;
    public Player.PlayerCharacter CharacterInUse;
    public List<SkillUpgrade> AvailableUpgrades { get; private set; }

    [SerializeField] private int _numButtons;
    [SerializeField] private List<UpgradeButton> _upgradeButtons; // (3) buttons for selecting upgrades.
    [SerializeField] private GameObject _upgradePanel;

    private GameObject _upgradeButtonTemplate;

    private void OnEnable()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();

        _upgradePanel = canvas.transform.Find("UpgradeScreen").gameObject;

        _upgradeButtons = new List<UpgradeButton>(_numButtons);

        _upgradeButtonTemplate =
            _upgradePanel.transform.Find("UpgradeButtonContainer/UpgradeButtonTemplate").gameObject;

        AvailableUpgrades = new List<SkillUpgrade>();

        Transform buttonContainer = _upgradeButtonTemplate.transform.parent;

        for(int i = 0; i < _numButtons; i++)
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

        // Load the contents of this folder at this path into an array Object[]
        var scriptableObjects = Resources.LoadAll("Scripts/Upgrade System/ScriptableObjects");

        // If the folder in the above path is empty, stop
        if(scriptableObjects.Length == 0)
        {
            Debug.LogError("Scripts/Upgrade System/ScriptableObjects is empty! No upgrades available");
            TogglePanel(false);
            return;
        }
        
        for(int i = 0; i < scriptableObjects.Length; i++)
        {
            SkillUpgrade foundUpgrade = null;

            // Try casting the Object at [i] to SkillUpgrade, which should work if it's of the correct type
            // (Essentially, check if it's the right type)
            try
            {
                foundUpgrade = (SkillUpgrade)scriptableObjects[i];
            }

            // Catch the exception: can not cast to SkillUpgrade
            // (so this Object is not compatible or is not of the type SkillUpgrade)
            catch(InvalidCastException)
            {
                // Show error: wrong type of file found in scriptableobjects folder
                Debug.LogError("Upgrade folder contains invalid objects! "
                                    + "Should only have SkillUpgrade ScriptableObjects ");

                // Skip to the next object in the upgrades array
                continue;
            }

            // Further check what kind of upgrade it is, and only add it to the list
            // if the character in use can "use" the upgrade
            switch (foundUpgrade)
            {
                // If found a tank upgrade and playing as one,
                // or if found an archer upgrade and playing as one,
                // add to the list
                case TankUpgrade when CharacterInUse == Player.PlayerCharacter.Tank:
                case ArcherUpgrade when CharacterInUse == Player.PlayerCharacter.Archer:
                    AvailableUpgrades.Add(foundUpgrade);
                    break;

                // If upgrade isn't of the right type for us, skip to the next upgrade in array
                case TankUpgrade when CharacterInUse != Player.PlayerCharacter.Tank:
                case ArcherUpgrade when CharacterInUse != Player.PlayerCharacter.Archer:
                    continue;

                // Should be a general upgrade, so add to the list.
                default:
                    AvailableUpgrades.Add(foundUpgrade);
                    continue;
            }
        }

        TogglePanel(false);
    }

    // Pop: get and remove from list
    public SkillUpgrade[] PopRandomSuitableUpgrades(int count)
    {
        int index = 0;
        SkillUpgrade[] randomUpgrades = new SkillUpgrade[count];

        if(AvailableUpgrades.Count == 0)
            return new SkillUpgrade[0];

        if (count > AvailableUpgrades.Count)
            count = AvailableUpgrades.Count;

        while(count > 0)
        {
            // Get random index
            int randIndex = Random.Range(0, AvailableUpgrades.Count);

            // Get reference to random upgrade from list
            var randUpgrade = AvailableUpgrades[randIndex];

            // Assign to array and remove from list, if minimum wave has been reached.
            if (Enemies.SpawnerManager.Instance.currentWave >= randUpgrade.MinimumWave)
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

    void RefreshAndShowPanel()
    {
        // Set each button up to have the correct info associated with the upgrade
        // First, pop random upgrades from the list, as many as we have buttons.
        var randUpgrades = PopRandomSuitableUpgrades(_upgradeButtons.Count);

        // If no more available upgrades, don't show panel.
        if(randUpgrades.Length == 0)
        {
            Debug.LogWarning("No more available upgrades! Not showing upgrade panel.");
            GameManager.Instance.PlayerSetReady();
            return;
        }

        // Then set each button's info according to the upgrade chosen for it,
        // and allow interaction.
        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
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
        Player.instance.AddUpgrade(upgrade);
        TogglePanel(false);
        GameManager.Instance.PlayerSetReady();
    }

    public void ShowUpgradePanel()
    {
        RefreshAndShowPanel();
    }
}
