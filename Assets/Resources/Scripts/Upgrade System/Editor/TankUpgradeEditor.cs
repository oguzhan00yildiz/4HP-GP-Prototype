using UnityEditor;

[CustomEditor(typeof(TankUpgrade))]
public class TankUpgradeEditor : Editor
{
    // Tank-specific properties
    private SerializedProperty _giveShieldUpgrade;
    private SerializedProperty _shieldDisplay;
    private SerializedProperty _giveSpearUpgrade;
    private SerializedProperty _spearDisplay;

    // General properties
    private SerializedProperty _upgradeName;
    private SerializedProperty _upgradeSprite;
    private SerializedProperty _minimumWave;
    private SerializedProperty _flavorText;

    private void OnEnable()
    {
        // Find tank-specific properties
        _giveShieldUpgrade = serializedObject.FindProperty("GiveShieldUpgrade");
        _shieldDisplay = serializedObject.FindProperty("GivenShieldUpgrade");
        _giveSpearUpgrade = serializedObject.FindProperty("GiveSpearUpgrade");
        _spearDisplay = serializedObject.FindProperty("GivenSpearUpgrade");

        // Find general properties
        _upgradeName = serializedObject.FindProperty("UpgradeName");
        _upgradeSprite = serializedObject.FindProperty("UpgradeIcon");
        _minimumWave = serializedObject.FindProperty("MinimumWave");
        _flavorText = serializedObject.FindProperty("FlavorText");
    }

    public override void OnInspectorGUI()
    {
        // Display tank-specific properties
        bool displayShield = _giveShieldUpgrade.boolValue;
        bool displaySpear = _giveSpearUpgrade.boolValue;

        if (displayShield && displaySpear)
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.PropertyField(_giveShieldUpgrade);
        if (displayShield)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_shieldDisplay);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_giveSpearUpgrade);
        if (displaySpear)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_spearDisplay);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();

        // Display general properties
        EditorGUILayout.PropertyField(_upgradeName);
        EditorGUILayout.PropertyField(_upgradeSprite);
        EditorGUILayout.PropertyField(_minimumWave);
        EditorGUILayout.PropertyField(_flavorText);

        serializedObject.ApplyModifiedProperties();
    }
}
