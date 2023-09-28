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
                float atkF = upgrade.AttackDamageFactor;
                float atkSpdF = upgrade.AttackSpeedFactor;
                float movSpdF = upgrade.MoveSpeedFactor;

                int atkFPercentage, atkSpdPercentage, movSpdPercentage;

                // Converting 0 to 1 multipliers to 0 to 100 percentages
                atkFPercentage = 100 - Mathf.Abs(((int)(100 * atkF)) - 100);
                atkSpdPercentage = 100 - Mathf.Abs(((int)(100 * atkSpdF)) - 100);
                movSpdPercentage = 100 - Mathf.Abs(((int)(100 * movSpdF)) - 100);

                // If more than a 1% increase/decrease
                if (upgrade.AffectDamage)
                {
                    switch (atkF)
                    {
                        case > 1.01f:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{100 - atkFPercentage}% damage");
                            break;
                        case < 0.99f:
                            // Add red-colored percentage ("-50% damage")
                            builder.AppendLine($"<color=\"red\">-{100 - atkFPercentage}% damage");
                            break;
                    }
                }

                // Doing the same as above for attack speed and movement speed
                if (upgrade.AffectAttackSpeed)
                {
                    switch (atkSpdF)
                    {
                        case > 1.01f:
                            builder.AppendLine($"<color=\"green\">+{100 - atkSpdPercentage}% attack speed");
                            break;
                        case < 0.99f:
                            builder.AppendLine($"<color=\"red\">-{100 - atkSpdPercentage}% attack speed");
                            break;
                    }
                }

                if (upgrade.AffectMoveSpeed)
                {
                    switch (movSpdF)
                    {
                        case > 1.01f:
                            // Add green-colored percentage ("+50% damage")
                            builder.AppendLine($"<color=\"green\">+{100 - movSpdPercentage}% movement speed");
                            break;
                        case < 0.99f:
                            builder.AppendLine($"<color=\"red\">-{100 - movSpdPercentage}% movement speed");
                            break;
                    }
                }

                _uiInfoText.text = builder.ToString();
                _uiFlavorText.text = upgrade.FlavorText;
            }
        }
    }
}
