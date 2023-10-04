using UnityEditor;

[CustomEditor(typeof(StatUpgrade))]
public class SkillUpgradeEditor : Editor
{

    SerializedProperty selectedTypeProp;
    SerializedProperty spriteProp;
    SerializedProperty nameProp;
    SerializedProperty flavorTextProp;
    SerializedProperty affectAtkDmg;
    SerializedProperty atkDmgProp;
    SerializedProperty affectAtkSpd;
    SerializedProperty atkSpdProp;
    SerializedProperty affectMoveSpd;
    SerializedProperty moveSpdProp;

    private void OnEnable()
    {
        // Find "AffectedStat" dropdown (enum) property
        selectedTypeProp = serializedObject.FindProperty("TypeOfUpgrade");
        spriteProp = serializedObject.FindProperty("UpgradeIcon");
        nameProp = serializedObject.FindProperty("UpgradeName");
        flavorTextProp = serializedObject.FindProperty("FlavorText");
        affectAtkDmg = serializedObject.FindProperty("AffectAttackDamage");
        atkDmgProp = serializedObject.FindProperty("AttackDamageFactor");
        affectAtkSpd = serializedObject.FindProperty("AffectAttackSpeed");
        atkSpdProp = serializedObject.FindProperty("AttackSpeedFactor");
        affectMoveSpd = serializedObject.FindProperty("AffectMoveSpeed");
        moveSpdProp = serializedObject.FindProperty("MoveSpeedFactor");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        // Update the actual scriptable object for us to display
        serializedObject.Update();

        EditorGUILayout.PropertyField(selectedTypeProp);

        StatUpgrade.UpgradeType typeSelection = (StatUpgrade.UpgradeType)selectedTypeProp.enumValueIndex;

        EditorGUILayout.Space();

        switch (typeSelection)
        {
            case StatUpgrade.UpgradeType.General:
                EditorGUILayout.LabelField("General Upgrade");
                break;
            case StatUpgrade.UpgradeType.Ranged:
                EditorGUILayout.LabelField("Ranged Upgrade");
                break;
            case StatUpgrade.UpgradeType.Melee:
                EditorGUILayout.LabelField("Melee Upgrade");
                break;
            case StatUpgrade.UpgradeType.Magic:
                EditorGUILayout.LabelField("Magic Upgrade");
                break;
        }

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
        */
    }
}
