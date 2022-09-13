using UnityEngine;
using UnityEditor;


namespace MPack
{
    public abstract class RereferenceDrawer : PropertyDrawer
    {
        protected GUIContent settingIcon;

        protected SerializedObject serializedObject;
        protected SerializedProperty useVariableProperty;
        protected SerializedProperty variableProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (useVariableProperty == null)
                OnEnable(property);

            return useVariableProperty.boolValue ? 43 : 20;
        }

        protected virtual void OnEnable(SerializedProperty property)
        {
            settingIcon = EditorGUIUtility.IconContent("d_AnimationWrapModeMenu");
            serializedObject = property.serializedObject;
            useVariableProperty = property.FindPropertyRelative("UseVariable");
            variableProperty = property.FindPropertyRelative("Variable");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (useVariableProperty == null)
                OnEnable(property);

            position.y += 1;

            Rect labelRect = position;
            labelRect.width = labelRect.width * 0.4f - 20;
            labelRect.height = 18;

            Rect settingIconRect = labelRect;
            settingIconRect.width = 20;
            settingIconRect.x += labelRect.width;

            Rect rest = position;
            rest.width *= 0.6f;
            rest.x += labelRect.width + 20 + 5;

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.LabelField(settingIconRect, settingIcon);

            serializedObject.Update();

            DrawValue(rest);

            HandleContextnMenu(settingIconRect);
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void DrawValue(Rect rest);

        #region Context Menu
        protected virtual void HandleContextnMenu(Rect position)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && position.Contains(e.mousePosition))
            {
                GenericMenu context = new GenericMenu();

                bool useVarible = useVariableProperty.boolValue;

                context.AddItem(new GUIContent("Variable"), useVarible, SwitchToVariable);
                context.AddItem(new GUIContent("Constant"), !useVarible, SwitchToConstant);

                context.AddSeparator("");
                if (useVarible)
                {
                    context.AddItem(new GUIContent("New"), false, CreateAsset);
                }
                else
                {
                    context.AddDisabledItem(new GUIContent("New"), false);
                }

                context.ShowAsContext();
            }
        }

        protected virtual void SwitchToVariable()
        {
            useVariableProperty.boolValue = true;
            useVariableProperty.serializedObject.ApplyModifiedProperties();
        }
        protected virtual void SwitchToConstant()
        {
            useVariableProperty.boolValue = false;
            useVariableProperty.serializedObject.ApplyModifiedProperties();
        }
        protected abstract void CreateAsset();
        #endregion
    }

}