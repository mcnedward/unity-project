using UnityEngine;

namespace Assets.Utils
{
    public static class Extensions
    {
        public static void ToggleObjectRenderers(GameObject gameObjectToToggle, bool show)
        {
            var renderers = gameObjectToToggle.GetComponentsInChildren<Renderer>();
            foreach (var enchantRenderer in renderers)
                enchantRenderer.enabled = show;
        }
    }
}
