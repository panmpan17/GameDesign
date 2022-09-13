using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    public class EditorUtilities : MonoBehaviour
    {
        private static GUIStyle buttonPressedStyle;

        public static bool ToggleButton(bool toggled, GUIContent btnContent) {
            if (buttonPressedStyle == null) {
                buttonPressedStyle = new GUIStyle("Button");
                buttonPressedStyle.padding = new RectOffset(0, 0, 5, 5);
                buttonPressedStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            return GUILayout.Toggle(toggled, btnContent, buttonPressedStyle);
        }

        /// <summary>
        /// Tanslate 0..255 to 0..1
        /// </summary>
        /// <param name="r">Red [0..255]</param>
        /// <param name="g">Green [0..255]</param>
        /// <param name="b">Blue [0..255]</param>
        /// <param name="a">Alpha [0..255]</param>
        /// <returns></returns>
        public static Color From256Color(float r, float g, float b, float a=255) {
            return new Color(
                r / 255 > 255 ? 1: r / 255,
                g / 255 > 255 ? 1: g / 255,
                b / 255 > 255 ? 1: b / 255,
                a / 255 > 255 ? 1 : a / 255
            );
        }
    }
}