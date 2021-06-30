using System;
using UnityEditor;
using UnityEngine;

namespace XNodeEditor {
    public class TextInputPopup : EditorWindow {
        private string inputControlName = "";

        public static TextInputPopup current { get; private set; }
        public string input;
        public Action onAfterClose;
        private bool firstFrame = true;

        /// <summary> Show a rename popup for an asset at mouse position. Will trigger reimport of the asset on apply.
        public static TextInputPopup Show(
            string textContnet,
            string title = "Text",
            string inputControlName = "Input",
            float width = 200) {
            TextInputPopup window = EditorWindow.GetWindow<TextInputPopup>(true, title, true);
            if (current != null) current.Close();
            current = window;
            window.inputControlName = inputControlName;
            window.input = textContnet;
            window.minSize = new Vector2(100, 44);
            window.position = new Rect(0, 0, width, 44);
            window.UpdatePositionToMouse();
            return window;
        }

        private void UpdatePositionToMouse() {
            if (Event.current == null) return;
            Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect pos = position;
            pos.x = mousePoint.x - position.width * 0.5f;
            pos.y = mousePoint.y - 10;
            position = pos;
        }

        private void OnLostFocus() {
            // Make the popup close on lose focus
            Close();
        }

        private void OnGUI() {
            if (firstFrame) {
                UpdatePositionToMouse();
                firstFrame = false;
            }
            GUI.SetNextControlName(inputControlName);
            input = EditorGUILayout.TextField(input);
            EditorGUI.FocusTextInControl(inputControlName);
            Event e = Event.current;
            // If input is empty, revert name to default instead
            if (input != null && input.Trim() != "") {
                if (GUILayout.Button("Confirm") || (e.isKey && e.keyCode == KeyCode.Return)) {
                    Close();
                    onAfterClose?.Invoke();
                }
            }

            if (e.isKey && e.keyCode == KeyCode.Escape) {
                Close();
            }
        }

        private void OnDestroy() {
            EditorGUIUtility.editingTextField = false;
        }
    }
}