using UnityEditor;

[CustomEditor(typeof(ArcherUpgrade))]
public class ArcherUpgradeEditor : Editor
{
    private SerializedProperty _giveMultiShotUpgrade;
    private SerializedProperty _multiShotDisplay;
    private SerializedProperty _giveBurstShotUpgrade;
    private SerializedProperty _burstShotDisplay;

    private SerializedProperty _upgradeName;
    private SerializedProperty _upgradeSprite;
    private SerializedProperty _minimumWave;
    private SerializedProperty _flavorText;

    void OnEnable()
    {
        // Archer-specific properties
        _giveMultiShotUpgrade = serializedObject.FindProperty("GiveMultiShotUpgrade");
        _multiShotDisplay = serializedObject.FindProperty("GivenMultiShotUpgrade");
        _giveBurstShotUpgrade = serializedObject.FindProperty("GiveBurstShotUpgrade");
        _burstShotDisplay = serializedObject.FindProperty("GivenBurstShotUpgrade");

        // General properties
        _upgradeName = serializedObject.FindProperty("UpgradeName");
        _upgradeSprite = serializedObject.FindProperty("UpgradeIcon");
        _minimumWave = serializedObject.FindProperty("MinimumWave");
        _flavorText = serializedObject.FindProperty("FlavorText");
    }

    public override void OnInspectorGUI()
    {
        // Serialize these archer-specific properties
        bool displayMultiShot = _giveMultiShotUpgrade.boolValue;
        bool displayBurstShot = _giveBurstShotUpgrade.boolValue;

        if (displayMultiShot && displayBurstShot)
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.PropertyField(_giveBurstShotUpgrade);

        if (displayBurstShot)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_burstShotDisplay);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        EditorGUILayout.PropertyField(_giveMultiShotUpgrade);

        if (displayMultiShot)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_multiShotDisplay);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        // Serialize general fields
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_upgradeSprite);
        EditorGUILayout.PropertyField(_upgradeName);
        EditorGUILayout.PropertyField(_minimumWave);
        EditorGUILayout.PropertyField(_flavorText);

        serializedObject.ApplyModifiedProperties();
    }
}
