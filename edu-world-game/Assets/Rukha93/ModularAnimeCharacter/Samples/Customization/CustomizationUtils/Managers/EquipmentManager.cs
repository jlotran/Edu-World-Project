using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    public class EquipmentManager
    {
        private readonly MaterialManager materialManager;
        private readonly Dictionary<string, string> temporaryEquipmentStorage;
        private readonly Dictionary<BodyPartType, BodyPartTag> bodyParts;
        
        public EquipmentManager(MaterialManager materialManager, Dictionary<BodyPartType, BodyPartTag> bodyParts)
        {
            this.materialManager = materialManager;
            this.bodyParts = bodyParts;
            this.temporaryEquipmentStorage = new Dictionary<string, string>();
        }

        public void Equip(string category, string path, CustomizationItemAsset item, LamFusion.CharacterEquip character)
        {
            if (character == null || item == null)
                return;

            if (character.m_ReferenceMesh == null)
            {
                Debug.LogError("Reference mesh is not initialized. Make sure to initialize the character properly.");
                return;
            }

            HandleConflictingEquipment(category, character);
            UnequipCategory(category, character, false);
            
            var equip = CreateEquipItem(path, item);
            character.m_Equiped[category] = equip;
            
            InstantiateEquipmentMeshes(item, equip, character);
            InstantiateEquipmentObjects(item, equip, character);
            
            UpdateBodyRenderers(character);
            InitRenderersForItem(equip);
            
            SaveEquipment(category, path);
        }

        private void HandleConflictingEquipment(string category, LamFusion.CharacterEquip character)
        {
            if (category == "outfit")
            {
                UnequipCategory("top", character, false);
                UnequipCategory("bottom", character, false);
            }
            else if (category == "top" || category == "bottom")
            {
                UnequipCategory("outfit", character, false);
            }
        }

        private EquipedItem CreateEquipItem(string path, CustomizationItemAsset item)
        {
            return new EquipedItem
            {
                path = path,
                assetReference = item,
                instantiatedObjects = new List<GameObject>()
            };
        }

        private void InstantiateEquipmentMeshes(CustomizationItemAsset item, EquipedItem equip, LamFusion.CharacterEquip character)
        {
            if (item == null || item.meshes == null || equip == null || character == null || character.m_ReferenceMesh == null)
            {
                Debug.LogWarning("Cannot instantiate equipment meshes - missing required components");
                return;
            }

            foreach (var mesh in item.meshes)
            {
                if (mesh == null || mesh.sharedMesh == null)
                {
                    Debug.LogWarning("Skipping null mesh in equipment");
                    continue;
                }

                var go = new GameObject(mesh.name);
                go.gameObject.layer = LayerMask.NameToLayer("PlayerCity");
                go.transform.SetParent(character.transform, false);
                equip.instantiatedObjects.Add(go);

                var skinnedMesh = go.AddComponent<SkinnedMeshRenderer>();
                skinnedMesh.rootBone = character.m_ReferenceMesh.rootBone;
                skinnedMesh.bones = character.m_ReferenceMesh.bones;
                skinnedMesh.localBounds = character.m_ReferenceMesh.localBounds;
                skinnedMesh.sharedMesh = mesh.sharedMesh;
                skinnedMesh.sharedMaterials = mesh.sharedMaterials ?? new Material[0];
            }
        }

        private void InstantiateEquipmentObjects(CustomizationItemAsset item, EquipedItem equip, LamFusion.CharacterEquip character)
        {
            foreach (var obj in item.objects)
            {
                var go = Object.Instantiate(obj.prefab, character.animator.GetBoneTransform(obj.targetBone));
                go.gameObject.layer = LayerMask.NameToLayer("PlayerCity");
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var child = go.transform.GetChild(i);
                    child.gameObject.layer = LayerMask.NameToLayer("PlayerCity");
                }
                equip.instantiatedObjects.Add(go);
            }
        }

        public void UnequipCategory(string category, LamFusion.CharacterEquip character, bool updateRenderers = true)
        {
            if (!character.m_Equiped.ContainsKey(category)) return;

            var item = character.m_Equiped[category];
            character.m_Equiped.Remove(category);

            foreach (var go in item.instantiatedObjects)
                Object.Destroy(go);

            if (updateRenderers)
                UpdateBodyRenderers(character);

            SaveEquipment(category, "");
        }

        public void UpdateBodyRenderers(LamFusion.CharacterEquip character)
        {
            var disabledParts = GetDisabledBodyParts(character);
            UpdateBodyPartVisibility(disabledParts);
            UpdateCharacterPosition(character);
        }

        private List<BodyPartType> GetDisabledBodyParts(LamFusion.CharacterEquip character)
        {
            var disabled = new List<BodyPartType>();
            foreach (var equip in character.m_Equiped.Values)
            {
                if (equip.assetReference == null) continue;
                foreach (var part in equip.assetReference.bodyParts)
                {
                    if (!disabled.Contains(part))
                        disabled.Add(part);
                }
            }
            return disabled;
        }

        private void UpdateBodyPartVisibility(List<BodyPartType> disabledParts)
        {
            foreach (var part in bodyParts)
                part.Value.gameObject.SetActive(!disabledParts.Contains(part.Key));
        }

        private void UpdateCharacterPosition(LamFusion.CharacterEquip character)
        {
            var localPos = character.transform.localPosition;
            localPos.y = character.m_Equiped.ContainsKey("shoes") ? 0.02f : 0;
            character.transform.localPosition = localPos;
        }

        private void InitRenderersForItem(EquipedItem item)
        {
            var renderers = new List<Renderer>();
            foreach (var obj in item.instantiatedObjects)
                renderers.AddRange(obj.GetComponentsInChildren<Renderer>());

            item.renderers = renderers.ToArray();

            foreach (var renderer in item.renderers)
                materialManager.SyncNewItemMaterials(renderer);
        }

        private void SaveEquipment(string category, string path)
        {
            if (!string.IsNullOrEmpty(path))
                temporaryEquipmentStorage[category] = path;
            else
                temporaryEquipmentStorage.Remove(category);
        }
                public Dictionary<string, string> GetStoredEquipment()
        {
            return new Dictionary<string, string>(temporaryEquipmentStorage);
        }
    }
}
