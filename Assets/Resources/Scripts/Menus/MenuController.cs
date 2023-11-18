using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menus
{
    public class MenuController : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonAction
        {
            // Simple struct to store a button and an action,
            // serializable so it can be shown in the inspector

            public Button button;
            public UnityEvent action;
        }

        public List<ButtonAction> ButtonActions = new();

        private void SeekButtonsRecursive(Transform origin)
        {
            // Recursive function; calls itself for every child of the origin.
            // If the origin has no children, it will just seek buttons in itself.
            if (origin == null)
                return;

            // If the origin has children, seek them first
            if (origin.childCount > 0)
            {
                foreach (Transform child in origin)
                {
                    SeekButtonsRecursive(child);
                }
            }

            // Get every button in the origin
            var buttons = origin.GetComponents<Button>();

            foreach (var button in buttons)
            {
                // Buttons that are turned off are ignored
                if (button.interactable == false)
                    continue;

                // Add the click sound (which does not exist yet) to the button
                button.onClick.AddListener(ButtonClickSound);

                // Add the correct action to the button based on
                // the ButtonActions list with the LINQ Find() function
                // where the button in the list is the same as the button we are handling here
                ButtonAction buttonAction = ButtonActions.Find(action => action.button == button);

                if (buttonAction.button == null || buttonAction.action == null)
                {
                    Debug.LogWarning($"No action found for {GetHierarchyPath(origin)}", button.gameObject);
                    continue;
                }

                // Subscribe the action's invocation to the button's
                // onClick event (i.e. action.Invoke() is called when the button is clicked)
                button.onClick.AddListener(() => buttonAction.action.Invoke());
            }
        }

        // Gets the full hierarchy path of a transform.
        private static string GetHierarchyPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        private void Start()
        {
            // This seeks buttons in the entire hierarchy from top to bottom
            // and adds the correct actions to them based on the actions
            // defined in the inspector
            SeekButtonsRecursive(transform);
        }

        public static void ButtonClickSound()
        {
            // TODO: Click sound
        }
    }
}
