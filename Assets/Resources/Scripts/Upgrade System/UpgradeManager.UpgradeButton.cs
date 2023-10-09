using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UpgradeManager
{
    // Simple class just for easier holding of the upgrade button's components
    // Without having to do GetComponent every time we want to access text and image in the button etc
    private class UpgradeButton
    {
        public StatUpgrade Upgrade;
        private Button _uiButton;
        private Image _uiImage;
        private TMP_Text _uiTitle;
        private TMP_Text _uiInfoText;
        private TMP_Text _uiFlavorText;
        public bool enabled
        {
            get => _uiButton.enabled;
            set => _uiButton.enabled = value;
        }
        public Button.ButtonClickedEvent onClick
        {
            get => _uiButton.onClick;
            set => _uiButton.onClick = value;
        }
        public bool interactable
        {
            get => _uiButton.interactable;
            set => _uiButton.interactable = value;
        }

        public UpgradeButton(GameObject buttonObj)
        {
            _uiButton = buttonObj.GetComponent<Button>();
            _uiImage = buttonObj.transform.Find("UpgradeImage").GetComponent<Image>();
            _uiTitle = buttonObj.transform.Find("UpgradeTitle").GetComponent<TMP_Text>();
            _uiInfoText = buttonObj.transform.Find("UpgradeInfo").GetComponent<TMP_Text>();
            _uiFlavorText = buttonObj.transform.Find("UpgradeFlavorText").GetComponent<TMP_Text>();
        }

        public void Destroy()
        {
            Object.Destroy(_uiButton.gameObject);
        }

        public void SetActive(bool value)
        {
            _uiButton.gameObject.SetActive(value);
        }

        public void SetInfoFromUpgrade([NotNull] StatUpgrade upgrade)
        {
            // Store upgrade in this button.
            Upgrade = upgrade;

            // Get a string builder object.
            var builder = new StringBuilder();

            // Set sprite and title.
            _uiImage.sprite = upgrade.UpgradeIcon;
            _uiTitle.text = upgrade.UpgradeName;

            // Format information to display in the info,
            // based on the upgrade's stat changes.
            
            foreach (var statChange in upgrade.StatChanges)
            {
                // No difference to display.
                if (statChange.Difference == 0)
                    continue;

                string statName = "";

                // Display the effect type in a more readable format.
                switch (statChange.AffectedStat)
                {
                    case StatUpgrade.Stat.AttackDamage:
                        statName = upgrade switch
                        {
                            TankUpgrade => "Melee Damage",
                            ArcherUpgrade => "Arrow Damage",
                            not null => "Damage"
                        };
                        break;
                    case StatUpgrade.Stat.AttackSpeed:
                        statName = upgrade switch
                        {
                            TankUpgrade => "Melee Speed",
                            ArcherUpgrade => "Firing Speed",
                            not null => "Attack Speed"
                        };
                        break;
                    case StatUpgrade.Stat.MeleeRange:
                        statName = "Melee Range";
                        break;
                    case StatUpgrade.Stat.MoveSpeed:
                        statName = "Movement Speed";
                        break;
                    case StatUpgrade.Stat.MaxHealth:
                        statName = "Health";
                        break;
                    case StatUpgrade.Stat.CritDamage:
                        statName = "Critical Damage";
                        break;
                    case StatUpgrade.Stat.CritChance:
                        statName = "Critical Chance";
                        break;
                    case StatUpgrade.Stat.Armor:
                        statName = "Armor";
                        break;
                    case StatUpgrade.Stat.Knockback:
                        statName = "Knockback";
                        break;
                }

                builder.AppendLine(statChange.Difference > 0
                    ? $"<color=\"green\">+{statChange.Difference}% {statName}"
                    : $"<color=\"red\">{statChange.Difference}% {statName}");
            }

            _uiInfoText.text = builder.ToString();
            _uiFlavorText.text = upgrade.FlavorText;
        }
    }
}
