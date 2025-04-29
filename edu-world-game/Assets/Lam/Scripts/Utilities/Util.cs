using System.Collections.Generic;
using Rukha93.ModularAnimeCharacter.Customization;
using UnityEngine;
namespace Lam
{
    public static class Util
    {
        public static Vector3 GetrandomSpawnPoint()
        {
            return new Vector3(UnityEngine.Random.Range(-5, 5), 3, UnityEngine.Random.Range(-5, 5));
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringResult = new char[length];
            System.Random random = new System.Random();

            for (int i = 0; i < stringResult.Length; i++)
            {
                stringResult[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringResult);
        }

        public static string[] nameRandom = new string[]
        {
            "Pham Trong Hoa",
            "Phan Duc Lam",
            "Nguyen Hai Viet",
            "Nguyen Doan Quan Dang",
            "Thuy Viet Quoc",
            "Vo Van Chi Thuan",
        };
        public static string RandomName()
        {
            return nameRandom[UnityEngine.Random.Range(0, nameRandom.Length)];
        }

        public static void ClearChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        public static void LoadSavedPath(out string gender, out List<string> listCat, out List<string> listPath)
        {
            gender = PlayerPrefs.GetString("Customization_gender", Edu_World_Game.GenderSelection.IsMale ? "m" : "f");
            listCat = new List<string>();
            listPath = new List<string>();

            foreach (string category in CustomizationDemo.m_Categories)
            {
                string savedPath;
                if (category == "skin")
                {
                    savedPath = CustomizationUtils.GetAssetPath(gender, "skin");
                }
                else
                {
                    savedPath = PlayerPrefs.GetString($"Equipment_{category}", "");
                }

                listCat.Add(category);
                listPath.Add(savedPath);
            }
        }
    }
}
