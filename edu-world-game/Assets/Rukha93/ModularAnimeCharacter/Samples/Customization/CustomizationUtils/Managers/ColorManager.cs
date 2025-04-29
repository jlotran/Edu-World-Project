using UnityEngine;
using System.Collections.Generic;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    public class ColorManager
    {
        private readonly MaterialManager materialManager;

        public ColorManager(MaterialManager materialManager)
        {
            this.materialManager = materialManager;
        }

        public void LoadSavedColors(LamFusion.CharacterEquip characterEquip)
        {
            if (characterEquip.m_Equiped == null || characterEquip.m_Equiped.Count == 0) return;

            foreach (var equip in characterEquip.m_Equiped.Values)
            {
                if (equip?.renderers == null) continue;
                ApplySavedColorsToRenderers(equip.renderers, characterEquip);
            }
        }

        private void ApplySavedColorsToRenderers(Renderer[] renderers, LamFusion.CharacterEquip characterEquip)
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    var material = renderer.sharedMaterials[i];
                    if (material == null) continue;

                    if (material.HasProperty("_MaskRemap"))
                    {
                        ApplyMaskRemapColors(renderer, i, material, characterEquip);
                    }
                    else if (material.HasProperty("_Color"))
                    {
                        ApplyBaseColor(renderer, i, material, characterEquip);
                    }
                }
            }
        }

        private void ApplyMaskRemapColors(Renderer renderer, int materialIndex, Material material, LamFusion.CharacterEquip characterEquip)
        {
            string[] properties = new string[]
            {
                "_Color_A_1", "_Color_A_2",
                "_Color_B_1", "_Color_B_2",
                "_Color_C_1", "_Color_C_2"
            };

            foreach (var prop in properties)
            {
                string key = $"Color_{material.name}_{prop}";
                if (PlayerPrefs.HasKey(key))
                {
                    Color defaultColor = material.GetColor(prop);
                    Color savedColor = CustomizationUtils.LoadColorFromPrefs(material.name, prop, defaultColor);
                    materialManager.ApplyColor(renderer, materialIndex, prop, savedColor, characterEquip);
                }
            }
        }

        private void ApplyBaseColor(Renderer renderer, int materialIndex, Material material, LamFusion.CharacterEquip characterEquip)
        {
            string key = $"Color_{material.name}_Color";
            if (PlayerPrefs.HasKey(key))
            {
                Color defaultColor = material.GetColor("_Color");
                Color savedColor = CustomizationUtils.LoadColorFromPrefs(material.name, "_Color", defaultColor);
                materialManager.ApplyColor(renderer, materialIndex, "_Color", savedColor, characterEquip);
            }
        }
    }
}
