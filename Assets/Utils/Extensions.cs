using UnityEngine;

namespace Assets.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Toggles a game object's active status.
        /// </summary>
        /// <param name="gameObjectToToggle">The game object to change.</param>
        /// <param name="show">Whether or not to set is game object as active.</param>
        public static void ToggleObject(GameObject gameObjectToToggle, bool show)
        {
            gameObjectToToggle.SetActive(show);
        }
    }
}
