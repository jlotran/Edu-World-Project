// using System;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Rukha93.ModularAnimeCharacter.Customization
// {
//     [Serializable]
//     public class CustomizationData : MonoBehaviour
//     {
//         public void SaveCustomizationItem(string category, string asset)
//         {
//             PlayerPrefs.SetString($"Customization_{category}", asset);
//             PlayerPrefs.Save();
//         }

//         public string LoadCustomizationItem(string category)
//         {
//             return PlayerPrefs.GetString($"Customization_{category}", "");
//         }

//         public void SaveGender(string gender)
//         {
//             PlayerPrefs.SetString("Customization_Gender", gender);
//             PlayerPrefs.Save();
//         }

//         public string LoadGender()
//         {
//             return PlayerPrefs.GetString("Customization_Gender", "");
//         }
//     }
// }
