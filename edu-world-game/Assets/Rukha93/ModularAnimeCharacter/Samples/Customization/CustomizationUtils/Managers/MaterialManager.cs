using UnityEngine;
using System.Collections.Generic;
using Rukha93.ModularAnimeCharacter.Customization.UI;

namespace Rukha93.ModularAnimeCharacter.Customization 
{
    public class MaterialManager 
    {
        private Dictionary<Material, MaterialPropertyBlock> materialProperties = new Dictionary<Material, MaterialPropertyBlock>();
        
        public MaterialPropertyBlock GetOrCreatePropertyBlock(Renderer renderer, int materialIndex)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block, materialIndex);
            return block;
        }

        public void SyncMaterialChange(Material sharedMaterial, MaterialPropertyBlock newProperties, LamFusion.CharacterEquip character)
        {
            foreach (var equip in character.m_Equiped.Values)
            {
                foreach (var renderer in equip.renderers)
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        if (renderer.sharedMaterials[i] != sharedMaterial)
                            continue;
                        renderer.SetPropertyBlock(newProperties, i);
                    }
                }
            }
        }

        public void SyncNewItemMaterials(Renderer renderer)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                if (materialProperties.ContainsKey(renderer.sharedMaterials[i]) == false)
                    continue;
                renderer.SetPropertyBlock(materialProperties[renderer.sharedMaterials[i]], i);
            }
        }

        public void ApplyColor(Renderer renderer, int materialIndex, string property, Color color, LamFusion.CharacterEquip character, bool saveToPrefs = true)
        {
            MaterialPropertyBlock block = GetOrCreatePropertyBlock(renderer, materialIndex);
            block.SetColor(property, color);
            renderer.SetPropertyBlock(block, materialIndex);

            var sharedMaterial = renderer.sharedMaterials[materialIndex];
            materialProperties[sharedMaterial] = block;

            SyncMaterialChange(sharedMaterial, block, character);
            
            if (saveToPrefs && CustomizationDemo.localCustomizeCharacter.isInteracUI)
            {
                CustomizationUtils.SaveColorToPrefs(sharedMaterial.name, property, color);
            }
        }
    }
}
