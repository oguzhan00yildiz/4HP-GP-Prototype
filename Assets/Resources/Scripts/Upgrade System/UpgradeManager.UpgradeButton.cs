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
                float atkIncrease = upgrade.AttackDamagePercentageIncrease;
                float atkSpdIncrease = upgrade.AttackSpeedPercentageIncrease;
                float movSpdIncrease = upgrade.MoveSpeedPercentageIncrease;
                float critDmgIncrease = upgrade.CritDamagePercentageIncrease;
                float critChanceIncrease = upgrade.CritDmgChancePercentageIncrease;
                float armorIncrease = upgrade.ArmorPercentageIncrease;

                // If more than a 1% increase/decrease
                if (upgrade.AffectDamage && atkIncrease != 0)
                {
                    switch (atkIncrease)
                    {
                        case > 0:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{atkIncrease}% damage");
                            break;
                        case < 0:
                            // Add red-colored percentage ("-50% damage")
                            builder.AppendLine($"<color=\"red\">-{atkIncrease}% damage");
                            break;
                    }
                }

                // Doing the same as above for attack speed and movement speed
                if (upgrade.AffectAttackSpeed && atkSpdIncrease != 0)
                {
                    switch (atkSpdIncrease)
                    {
                        case > 0:
                            builder.AppendLine($"<color=\"green\">+{atkSpdIncrease}% attack speed");
                            break;
                        case < 0:
                            builder.AppendLine($"<color=\"red\">-{atkSpdIncrease}% attack speed");
                            break;
                    }
                }

                if (upgrade.AffectMoveSpeed && movSpdIncrease != 0)
                {
                    switch (movSpdIncrease)
                    {
                        case > 0:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{movSpdIncrease}% movement speed");
                            break;
                        case < 0:
                            builder.AppendLine($"<color=\"red\">-{movSpdIncrease}% movement speed");
                            break;
                    }
                }

                if (upgrade.AffectArmor && armorIncrease != 0)
                {
                    switch (armorIncrease)
                    {
                        case > 0:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{armorIncrease}% armor");
                            break;
                        case < 0:
                            builder.AppendLine($"<color=\"red\">-{armorIncrease}% armor");
                            break;
                    }
                }

                if (upgrade.AffectCritChance && critChanceIncrease != 0)
                {
                    switch (critChanceIncrease)
                    {
                        case > 0:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{critChanceIncrease}% critical chance");
                            break;
                        case < 0:
                            builder.AppendLine($"<color=\"red\">-{critChanceIncrease}% critical chance");
                            break;
                    }
                }

                if (upgrade.AffectCritDmg && critDmgIncrease != 0)
                {
                    switch (critDmgIncrease)
                    {
                        case > 0:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{critDmgIncrease}% critical damage");
                            break;
                        case < 0:
                            builder.AppendLine($"<color=\"red\">-{critDmgIncrease}% critical damage");
                            break;
                    }
                }

                _uiInfoText.text = builder.ToString();
                _uiFlavorText.text = upgrade.FlavorText;
            }
        }
    }
}
