using System.Collections.Generic;
using Rukha93.ModularAnimeCharacter.Customization;
using UnityEngine;

namespace LamFusion
{
    public class CharacterEquip : MonoBehaviour
    {
        public Animator animator;
        public Dictionary<string, EquipedItem> m_Equiped = new Dictionary<string, EquipedItem>();
        public SkinnedMeshRenderer m_ReferenceMesh;
    }
}
