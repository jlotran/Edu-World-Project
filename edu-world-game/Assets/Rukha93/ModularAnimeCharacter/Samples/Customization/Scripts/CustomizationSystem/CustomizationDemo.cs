using Rukha93.ModularAnimeCharacter.Customization.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lam;
using Lam.FUSION;
using Edu_World;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    #region Classes

    public class CustomizationDemo : MonoBehaviour
    {
        #region Variables
        // Static Variables
        public static CustomizationDemo localCustomizeCharacter;
        public static List<string> m_Categories = new List<string>
        {
            "skin",
            "head",
            "hairstyle",
            // "acc_head",
            "top",
            "bottom",
            "shoes",
            // "outfit"
        };

        // Serialized Variables
        [SerializeField] private BaseUICustomization m_BaseUI; // Changed from UICustomizationDemo to BaseUICustomization
        public BaseUICustomization m_UI => m_BaseUI; // Changed accessor
        [SerializeField] private LamFusion.CharacterEquip characterEquip;

        // Protected Variables  
        protected IAssetLoader m_AssetLoader => FakeLoader.instance;
        protected Dictionary<string, List<string>> m_CustomizationOptions; //<categoryId, assetPath[]>
        protected Dictionary<BodyPartType, BodyPartTag> m_BodyParts;
        protected Dictionary<Material, MaterialPropertyBlock> m_MaterialProperties;
        protected Coroutine m_LoadingCoroutine;
        protected Dictionary<string, CustomizationItemAsset> m_LoadedAssets = new Dictionary<string, CustomizationItemAsset>();

        // Private Variables
        public Dictionary<string, Color> temporaryColorStorage = new Dictionary<string, Color>();
        public Dictionary<string, string> temporaryEquipmentReload = new Dictionary<string, string>();
        public bool isSpawnStart;
        public bool isInteracUI => m_UI != null && localCustomizeCharacter == this;
        private MaterialManager materialManager;
        private ColorManager colorManager;
        private EquipmentManager equipmentManager;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            materialManager = new MaterialManager();
            colorManager = new ColorManager(materialManager);
            //init variables
            m_BodyParts = new Dictionary<BodyPartType, BodyPartTag>();
            m_MaterialProperties = new Dictionary<Material, MaterialPropertyBlock>();
            equipmentManager = new EquipmentManager(materialManager, m_BodyParts);
            LoadEquipmentReloadFromPrefs();
        }

        private void Start()
        {
            // Copy values from temporaryEquipmentStorage to temporaryEquipmentReload
            if (isSpawnStart)
            {
                localCustomizeCharacter = this;
                SpawnPlayer(null);
            }

            if (isInteracUI)
            {
                m_UI.OnClickCategory += OnSelectCategory;
                m_UI.OnChangeItem += OnSwapItem;
                m_UI.OnChangeColor += OnChangeColor;
                m_UI.SetCategories(m_Categories.ToArray());
                for (int i = 0; i < m_Categories.Count; i++)
                    m_UI.SetCategoryValue(i, "");
            }

        }

        private void OnDestroy()
        {
            m_LoadedAssets.Clear();
        }
        #endregion

        #region Initialization Methods
        public void SetLocalPlayer()
        {
            localCustomizeCharacter = this;
        }

        public void SpawnPlayer(System.Action<string, List<string>, List<string>> action)
        {
            // Load data from asset
            if (action == null)
            {
                m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody());
            }
            else
            {
                List<string> cats;
                List<string> paths;
                string gender = default;
                Util.LoadSavedPath(out gender, out cats, out paths);
                action(gender, cats, paths);
            }
        }

        private void InitBody(string path, GameObject prefab, LamFusion.CharacterEquip parent)
        {
            //instantiate the skin prefab and store the animator
            Animator character = Instantiate(prefab, this.transform).GetComponent<Animator>();

            //get a random skin mesh to be used as reference
            var meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
            parent.m_ReferenceMesh = meshes[meshes.Length / 2];

            //initialize all tagged skin parts
            //they be used to disable meshes that are hidden by clothes
            var bodyparts = character.GetComponentsInChildren<BodyPartTag>();
            foreach (var part in bodyparts)
                m_BodyParts[part.type] = part;

            var equip = new EquipedItem()
            {
                path = path,
                assetReference = null,
                instantiatedObjects = new List<GameObject>() { character.gameObject }
            };
            InitRenderersForItem(equip);
            parent.m_Equiped["skin"] = equip;

            parent.animator = character;
            character.transform.SetParent(parent.transform, false);

            //update ui
            if (isInteracUI)
            {
                m_UI.SetCategoryValue(m_Categories.IndexOf("skin"), path);
                if (m_UI.IsCustomizationOpen && m_UI.CurrentCategory == "skin")
                    m_UI.SetCustomizationMaterials(equip.renderers);
            }
        }

        private IEnumerator InitData(string bodyType)
        {
            List<Coroutine> coroutines = new List<Coroutine>();

            m_CustomizationOptions = new Dictionary<string, List<string>>();
            for (int i = 0; i < m_Categories.Count; i++)
            {
                int index = i;
                string path = m_Categories[i].Equals("skin") ? "skin" : CustomizationUtils.GetAssetPath(bodyType, m_Categories[i]);
                coroutines.Add(StartCoroutine(m_AssetLoader.LoadAssetList(path, res =>
                {
                    m_CustomizationOptions[m_Categories[index]] = new List<string>(res);
                })));
            }

            for (int i = 0; i < m_Categories.Count; i++)
            {
                yield return coroutines[i];

                //add an empty item for all categories that can be empty
                if (m_Categories[i].Equals("skin"))
                    continue;
                if (m_Categories[i].Equals("head"))
                    continue;
                m_CustomizationOptions[m_Categories[i]].Insert(0, "");
            }
        }

        protected IEnumerator Co_LoadAndInitBody()
        {
            List<string> cats;
            List<string> paths;

            Util.LoadSavedPath(out string gender, out cats, out paths);
            yield return StartCoroutine(EquipPath(gender, cats, paths));

            LoadSavedColors();

            m_LoadingCoroutine = null;
        }
        #endregion

        #region Equipment Management
        // Equip category by path
        public IEnumerator EquipPath(string gender, List<string> cats, List<string> paths)
        {
            StartCoroutine(InitData(gender));

            if (cats.Count != paths.Count)
            {
                yield break;
            }

            // Wait for customization options to be loaded
            yield return new WaitUntil(() => m_CustomizationOptions != null && m_CustomizationOptions.Count > 0);



            for (int i = 0; i < cats.Count; i++)
            {
                string category = cats[i];
                string path = paths[i];
                if (category == "skin")
                {
                    yield return m_AssetLoader.LoadAsset<GameObject>(path, res => InitBody(path, res, this.characterEquip));
                    continue;
                }
                // Set default item if path is empty
                if (string.IsNullOrEmpty(path) && m_CustomizationOptions.ContainsKey(category) && m_CustomizationOptions[category].Count > 0)
                {
                    // Use index 0 for head category, index 1 for others
                    path = m_CustomizationOptions[category][category == "head" ? 0 : 1];
                }

                if (!string.IsNullOrEmpty(path))
                {
                    yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(path, res => Equip(category, path, res, this.characterEquip));
                }
            }
        }

        // Equip worker
        public void Equip(string cat, string path, CustomizationItemAsset item, LamFusion.CharacterEquip character)
        {
            equipmentManager.Equip(cat, path, item, character);

            //update ui
            if (isInteracUI)
            {
                m_UI.SetCategoryValue(m_Categories.IndexOf(cat), path);
                if (m_UI.IsCustomizationOpen && m_UI.CurrentCategory == cat)
                    m_UI.SetCustomizationMaterials(character.m_Equiped[cat].renderers);
            }

            //send message to the character
            character.SendMessage("OnChangeEquip", new object[] { cat, character.m_Equiped[cat].instantiatedObjects }, SendMessageOptions.DontRequireReceiver);
        }

        protected IEnumerator Co_LoadAndEquip(string cat, string path)
        {
            yield return m_AssetLoader.LoadAsset<CustomizationItemAsset>(path, res => 
            {
                Equip(cat, path, res, this.characterEquip);
                
                // Khi equip head, áp dụng màu của skin cho face material
                if (cat == "head" && this.characterEquip.m_Equiped.ContainsKey("skin"))
                {
                    var skinRenderers = this.characterEquip.m_Equiped["skin"].renderers;
                    var headRenderers = this.characterEquip.m_Equiped["head"].renderers;

                    // Tìm skin material để lấy màu
                    foreach (var skinRenderer in skinRenderers)
                    {
                        for (int i = 0; i < skinRenderer.sharedMaterials.Length; i++)
                        {
                            var skinMat = skinRenderer.sharedMaterials[i];
                            MaterialPropertyBlock skinBlock = new MaterialPropertyBlock();
                            skinRenderer.GetPropertyBlock(skinBlock, i);

                            // Áp dụng màu cho head material
                            foreach (var headRenderer in headRenderers)
                            {
                                for (int j = 0; j < headRenderer.sharedMaterials.Length; j++)
                                {
                                    var headMat = headRenderer.sharedMaterials[j];
                                    if (headMat.name.Contains("mat_base_M_face") || headMat.name.Contains("mat_base_F_face"))
                                    {
                                        MaterialPropertyBlock headBlock = new MaterialPropertyBlock();
                                        headRenderer.GetPropertyBlock(headBlock, j);

                                        // Copy các màu từ skin sang head
                                        if (skinBlock.HasColor("_Color_A_1"))
                                            headBlock.SetColor("_Color_A_1", skinBlock.GetColor("_Color_A_1"));
                                        if (skinBlock.HasColor("_Color_A_2"))
                                            headBlock.SetColor("_Color_A_2", skinBlock.GetColor("_Color_A_2"));

                                        headRenderer.SetPropertyBlock(headBlock, j);
                                        m_MaterialProperties[headMat] = headBlock;
                                        materialManager.SyncMaterialChange(headMat, headBlock, this.characterEquip);
                                    }
                                }
                            }
                        }
                    }
                }
            });
            m_LoadingCoroutine = null;
        }

        public void Unequip(string category, LamFusion.CharacterEquip character, bool updateRenderers = true)
        {
            equipmentManager.UnequipCategory(category, character, updateRenderers);

            //update UI
            if (isInteracUI)
            {
                m_UI.SetCategoryValue(m_Categories.IndexOf(category), "");
                if (m_UI.IsCustomizationOpen)
                    m_UI.SetCustomizationMaterials(null);
            }
        }

        public void UpdateBodyRenderers(LamFusion.CharacterEquip character)
        {
            equipmentManager.UpdateBodyRenderers(character);
        }
        #endregion

        private void InitRenderersForItem(EquipedItem item)
        {
            List<Renderer> renderers = new List<Renderer>();
            List<MaterialPropertyBlock> props = new List<MaterialPropertyBlock>();

            //get all materials in the instantiated items
            foreach (var obj in item.instantiatedObjects)
                renderers.AddRange(obj.GetComponentsInChildren<Renderer>());

            item.renderers = renderers.ToArray();

            //update the material properties for the new item
            foreach (var renderer in item.renderers)
                materialManager.SyncNewItemMaterials(renderer);
        }

        #endregion

        #region Save/Load Methods

        public void LoadEquipmentReloadFromPrefs()
        {
            temporaryEquipmentReload.Clear(); // Xóa dữ liệu cũ để tránh lỗi trùng lặp

            foreach (string category in m_Categories)
            {
                string savedPath = PlayerPrefs.GetString($"Equipment_{category}", "");

                if (!string.IsNullOrEmpty(savedPath))
                {
                    temporaryEquipmentReload[category] = savedPath;
                }
            }

        }

        public void SaveAllToPrefs()
        {
            // Save equipment
            var storedEquipment = equipmentManager.GetStoredEquipment();

            List<string> cats = new List<string>();
            List<string> paths = new List<string>();

            foreach (var kvp in storedEquipment)
            {
                cats.Add(kvp.Key);
                paths.Add(kvp.Value);
                PlayerPrefs.SetString($"Equipment_{kvp.Key}", kvp.Value);
            }

            // Synchronous equiment to Fusion
            Lam.FUSION.Player player = Player.localPlayer;

            if (player)
            {
                string gender = PlayerPrefs.GetString("Customization_gender", Edu_World_Game.GenderSelection.IsMale ? "m" : "f");
                player.SaveDataEquip(gender, cats, paths);
            }

            // Save colors
            foreach (var kvp in temporaryColorStorage)
            {
                string colorString = $"{kvp.Value.r},{kvp.Value.g},{kvp.Value.b},{kvp.Value.a}";
                PlayerPrefs.SetString(kvp.Key, colorString);
            }

            PlayerPrefs.Save();
        }

        private void LoadSavedColors()
        {
            colorManager.LoadSavedColors(this.characterEquip);
        }
        #endregion

        #region UI Management
        // public void InitializeUI()
        // {
        //     if (isInteracUI)
        //     {
        //         m_UI.OnClickCategory += OnSelectCategory;
        //         m_UI.OnChangeItem += OnSwapItem;
        //         m_UI.OnChangeColor += OnChangeColor;

        //         // Set categories like in Start()
        //         m_UI.SetCategories(m_Categories.ToArray());
        //         for (int i = 0; i < m_Categories.Count; i++)
        //             m_UI.SetCategoryValue(i, "");

        //         // Wait for assets to be loaded before selecting hairstyle
        //         StartCoroutine(SelectHairstyleAfterLoad());
        //     }
        // }

        public IEnumerator SelectAfterLoad(string category)
        {
            // Wait until m_CustomizationOptions contains the hairstyle category
            yield return new WaitUntil(() => m_CustomizationOptions.ContainsKey(category));

            // Wait one more frame to ensure all assets are properly initialized
            yield return null;

            if (m_Categories.Contains(category))
            {
                OnSelectCategory(category);
            }
        }

        public virtual void OnSelectCategory(string cat)
        {
            if (isInteracUI)
            {
                if (string.Equals(m_UI.CurrentCategory, cat))
                {
                    m_UI.ShowCustomization(false);
                    return;
                }

                // Load assets for the category if they haven't been loaded yet
                if (!m_LoadedAssets.ContainsKey(cat))
                {
                    foreach (string itemPath in m_CustomizationOptions[cat])
                    {
                        if (!string.IsNullOrEmpty(itemPath))
                        {
                            StartCoroutine(m_AssetLoader.LoadAsset<CustomizationItemAsset>(itemPath,
                                (CustomizationItemAsset asset) =>
                                {
                                    if (asset != null)
                                    {
                                        m_LoadedAssets[itemPath] = asset;
                                        // Refresh UI if this is the current category
                                        if (m_UI.CurrentCategory == cat)
                                        {
                                            m_UI.SetCustomizationOptions(cat,
                                                m_CustomizationOptions[cat].ToArray(),
                                                this.characterEquip.m_Equiped.ContainsKey(cat) ? this.characterEquip.m_Equiped[cat].path : "",
                                                m_LoadedAssets);
                                        }
                                    }
                                }));
                        }
                    }
                }

                // Initialize UI with currently loaded assets
                m_UI.SetCustomizationOptions(cat,
                    m_CustomizationOptions[cat].ToArray(),
                    this.characterEquip.m_Equiped.ContainsKey(cat) ? this.characterEquip.m_Equiped[cat].path : "",
                    m_LoadedAssets);

                //set material options
                if (this.characterEquip.m_Equiped.ContainsKey(cat))
                    m_UI.SetCustomizationMaterials(this.characterEquip.m_Equiped[cat].renderers);
                else
                    m_UI.SetCustomizationMaterials(null);

                //show UI
                m_UI.ShowCustomization(true);
            }
        }

        public virtual void OnSwapItem(string cat, string asset)
        {
            //if empty, just unequip the current one if any
            if (string.IsNullOrEmpty(asset))
            {
                Unequip(cat, this.characterEquip);
                return;
            }

            //stop loading previous item
            if (m_LoadingCoroutine != null)
                StopCoroutine(m_LoadingCoroutine);

            //load new item
            if (cat.Equals("skin"))
                // m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody(asset.StartsWith("m") ? "m" : "f"));
                m_LoadingCoroutine = StartCoroutine(Co_LoadAndInitBody());
            else
                m_LoadingCoroutine = StartCoroutine(Co_LoadAndEquip(cat, asset));
        }

        public void OnChangeColor(Renderer renderer, int materialIndex, string property, Color color)
        {
            materialManager.ApplyColor(renderer, materialIndex, property, color, this.characterEquip, true);
            // Apply color to the original material
            materialManager.ApplyColor(renderer, materialIndex, property, color, this.characterEquip);

            if (isInteracUI && m_UI.CurrentCategory == "skin")
            {
                // Apply to head if it exists
                if (this.characterEquip.m_Equiped.ContainsKey("head"))
                {
                    foreach (var headRenderer in this.characterEquip.m_Equiped["head"].renderers)
                    {
                        for (int i = 0; i < headRenderer.sharedMaterials.Length; i++)
                        {
                            Material headMat = headRenderer.sharedMaterials[i];
                            // Only apply to face material
                            if (headMat.name.Contains("mat_base_M_face") || headMat.name.Contains("mat_base_F_face"))
                            {
                                if (headMat.HasProperty(property))
                                {
                                    MaterialPropertyBlock headBlock = new MaterialPropertyBlock();
                                    headRenderer.GetPropertyBlock(headBlock, i);
                                    headBlock.SetColor(property, color);
                                    headRenderer.SetPropertyBlock(headBlock, i);
                                    m_MaterialProperties[headMat] = headBlock;
                                    materialManager.SyncMaterialChange(headMat, headBlock, this.characterEquip);

                                    // Save color for head material
                                    CustomizationUtils.SaveColorToPrefs(headMat.name, property, color);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnChangeColorFusion(Renderer renderer, int materialIndex, string property, Color color, LamFusion.CharacterEquip characterEquip)
        {
            // Apply color to the original material
            materialManager.ApplyColor(renderer, materialIndex, property, color, characterEquip);

            if (isInteracUI && m_UI.CurrentCategory == "skin")
            {
                // Apply to head if it exists
                if (characterEquip.m_Equiped.ContainsKey("head"))
                {
                    foreach (var headRenderer in characterEquip.m_Equiped["head"].renderers)
                    {
                        for (int i = 0; i < headRenderer.sharedMaterials.Length; i++)
                        {
                            Material headMat = headRenderer.sharedMaterials[i];
                            // Only apply to face material
                            if (headMat.name.Contains("mat_base_M_face") || headMat.name.Contains("mat_base_F_face"))
                            {
                                if (headMat.HasProperty(property))
                                {
                                    MaterialPropertyBlock headBlock = new MaterialPropertyBlock();
                                    headRenderer.GetPropertyBlock(headBlock, i);
                                    headBlock.SetColor(property, color);
                                    headRenderer.SetPropertyBlock(headBlock, i);
                                    m_MaterialProperties[headMat] = headBlock;
                                    materialManager.SyncMaterialChange(headMat, headBlock, characterEquip);

                                    // Save color for head material
                                    CustomizationUtils.SaveColorToPrefs(headMat.name, property, color);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public void ReloadSavedEquipment()
        {

            LoadEquipmentReloadFromPrefs(); // Load từ PlayerPrefs nếu dictionary trống

            foreach (var kvp in temporaryEquipmentReload)
            {
                string category = kvp.Key;
                string path = kvp.Value;

                // Skip skin category vì skin không thể thay đổi
                if (category == "skin")
                    continue;

                if (!string.IsNullOrEmpty(path))
                {
                    if (m_LoadingCoroutine != null)
                        StopCoroutine(m_LoadingCoroutine);

                    m_LoadingCoroutine = StartCoroutine(Co_LoadAndEquip(category, path));
                }
                else
                {
                    Unequip(category, this.characterEquip);
                }
            }
        }

        public void ReloadSavedEquipmentByCategory(string category)
        {

            LoadEquipmentReloadFromPrefs();

            // Skip skin category as it cannot be changed
            if (category == "skin")
                return;

            if (temporaryEquipmentReload.TryGetValue(category, out string path))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (m_LoadingCoroutine != null)
                        StopCoroutine(m_LoadingCoroutine);

                    m_LoadingCoroutine = StartCoroutine(Co_LoadAndEquip(category, path));
                }
                else
                {
                    Unequip(category, this.characterEquip);
                }
            }
        }

        public void SetUI(BaseUICustomization uiCustomization)
        {
            m_BaseUI = uiCustomization;
        }
    }
}
