using UnityEngine;
using System.Collections.Generic;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    public class EquipedItem
    {
        public string path;
        public List<GameObject> instantiatedObjects;
        public CustomizationItemAsset assetReference;
        public Renderer[] renderers;
    }
}
