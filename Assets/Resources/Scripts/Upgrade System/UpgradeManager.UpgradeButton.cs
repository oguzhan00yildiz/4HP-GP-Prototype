using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public partial class UpgradeManager
{
    // Simple class just for easier holding of the upgrade button's components
    // Without having to do GetComponent every time we want to access text and image in the button etc
    private class UpgradeButton
    {
        public SkillUpgrade Upgrade;
        private Button _uiButton;
        private Image _uiImage;
        private TMP_Text _uiTitle;
        private TMP_Text _uiInfoText;
        private TMP_Text _uiFlavorText;
        public bool enabled
        {
            get { return _uiButton.enabled; }
            set { _uiButton.enabled = value; }
        }
        public Button.ButtonClickedEvent onClick
        {
            get { return _uiButton.onClick; }
            set { _uiButton.onClick = value; }
        }
        public bool interactable
        {
            get { return _uiButton.interactable; }
            set { _uiButton.interactable = value; }
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

        public void SetInfoFromUpgrade(SkillUpgrade upgrade)
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

            // Tank-specific info.
            if (upgrade is TankUpgrade)
            {
                var tUpgrade = (TankUpgrade)upgrade;

                Debug.Log("Tank-specific upgrade info display isn't implemented yet!");
            }
            // Archer-specific info.
            else if (upgrade is ArcherUpgrade)
            {
                var aUpgrade = (ArcherUpgrade)upgrade;

                Debug.Log("Archer-specific upgrade info display isn't implemented yet!");
            }

            // General upgrade info.
            else
            {
                var fx = upgrade.Effects;

                for (int i = 0; i < fx.Count; i++)
                {
                    // This particular effect from the list.
                    var effect = fx[i];

                    // Too small of a difference to display.
                    if (Mathf.Approximately(effect.Difference, 0))
                        continue;

                    string displayType = "";

                    // Display the effect type in a more readable format.
                    switch (effect.Type)
                    {

                        case SkillUpgrade.Effect.EffectType.AttackDamage:
                            displayType = "Damage";
                            break;
                        case SkillUpgrade.Effect.EffectType.AttackSpeed:
                            displayType = "Attack Speed";
                            break;
                        case SkillUpgrade.Effect.EffectType.MoveSpeed:
                            displayType = "Movement Speed";
                            break;
                        case SkillUpgrade.Effect.EffectType.MaxHealth:
                            displayType = "Health";
                            break;
                        case SkillUpgrade.Effect.EffectType.CritDamage:
                            displayType = "Critical Damage";
                            break;
                        case SkillUpgrade.Effect.EffectType.CritChance:
                            displayType = "Critical Chance";
                            break;
                        case SkillUpgrade.Effect.EffectType.Armor:
                            displayType = "Armor";
                            break;
                    }

                    if (effect.Difference > 0)
                        builder.AppendLine($"<color=\"green\">+{effect.Difference}% {displayType}");
                    else
                        builder.AppendLine($"<color=\"red\">{effect.Difference}% {displayType}");
                }

                _uiInfoText.text = builder.ToString();
                _uiFlavorText.text = upgrade.FlavorText;
            }
        }
    }
}
