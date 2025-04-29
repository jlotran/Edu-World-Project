using UnityEngine;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    public static class CustomizationUtils
    {
        public static string GetAssetPath(string bodyType, string asset)
        {
            return $"{bodyType}/{asset}".ToLower();
        }

        public static string GetAssetPath(string bodyType, string category, string asset)
        {
            return $"{bodyType}/{category}/{asset}".ToLower();
        }

        public static void SaveColorToPrefs(string materialName, string property, Color color)
        {
            string key = $"Color_{materialName}_{property}";
            string colorString = $"{color.r},{color.g},{color.b},{color.a}";
            PlayerPrefs.SetString(key, colorString);
        }

        public static Color LoadColorFromPrefs(string materialName, string property, Color defaultColor)
        {
            string key = $"Color_{materialName}_{property}";
            if (PlayerPrefs.HasKey(key))
            {
                string colorString = PlayerPrefs.GetString(key);
                string[] components = colorString.Split(',');
                if (components.Length == 4)
                {
                    return new Color(
                        float.Parse(components[0]),
                        float.Parse(components[1]),
                        float.Parse(components[2]),
                        float.Parse(components[3])
                    );
                }
            }
            return defaultColor;
        }
    }
}
