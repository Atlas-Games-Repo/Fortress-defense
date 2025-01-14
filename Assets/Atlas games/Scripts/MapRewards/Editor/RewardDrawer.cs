using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Reward))]
public class RewardDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty rewardLevel = property.FindPropertyRelative("rewardLevel");
        SerializedProperty type = property.FindPropertyRelative("type");
        SerializedProperty shopItemName = property.FindPropertyRelative("shopItemName");
        SerializedProperty amount = property.FindPropertyRelative("amount");
        SerializedProperty icon = property.FindPropertyRelative("icon");

        RewardList rewardList = (RewardList)AssetDatabase.LoadAssetAtPath("Assets/Atlas games/Scripts/MapRewards/Scriptables/Rewards.asset", typeof(RewardList));

        Rect rewardLevelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect typeRect = new Rect(position.x, position.y + 20, position.width, EditorGUIUtility.singleLineHeight);
        Rect amountRect = new Rect(position.x, position.y + 40, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(rewardLevelRect, rewardLevel);
        EditorGUI.PropertyField(typeRect, type);
        EditorGUI.PropertyField(amountRect, amount);

        RewardType rewardType = (RewardType)type.enumValueIndex;

        if (rewardType == RewardType.ShopItem)
        {
            Rect shopItemNameRect = new Rect(position.x, position.y + 60, position.width, EditorGUIUtility.singleLineHeight);
            Rect iconRect = new Rect(position.x, position.y + 80, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(shopItemNameRect, shopItemName);
            EditorGUI.PropertyField(iconRect, icon);
        }
        else if (rewardType == RewardType.Coin)
        {
            icon.objectReferenceValue = rewardList.coinIcon;
        }
        else if (rewardType == RewardType.Exp)
        {
            icon.objectReferenceValue = rewardList.expIcon;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty type = property.FindPropertyRelative("type");

        if (type.enumValueIndex == (int)RewardType.ShopItem)
        {
            return EditorGUIUtility.singleLineHeight * 5.5f; 
        }

        return EditorGUIUtility.singleLineHeight * 3.2f; 
    }
}
