using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Edu_World;

public class MajorSelectionView : MonoBehaviour, IMajorSelectionView
{
    [SerializeField] private Transform majorParent;
    [SerializeField] private MajorItem majorItemPrefab;

    [SerializeField] private Transform majorCategoryParent;
    [SerializeField] private MajorCategoryItem majorCategoryItemPrefab;

    [SerializeField] private SnappingScrollView categoryScrollView;

    private MajorSelectionPresenter presenter;
    private ObjectPool<MajorCategoryItem> categoryItemPool;
    
    private List<MajorData> majorList = new List<MajorData>();
    private string currentSelectedMajor = string.Empty;

    private void Start()
    {
        categoryItemPool = new ObjectPool<MajorCategoryItem>(majorCategoryItemPrefab, majorCategoryParent);
        presenter = new MajorSelectionPresenter(this);
    }

    public void ShowMajors(List<MajorData> majorList)
    {
        this.majorList = new List<MajorData>(majorList);
        
        if (majorList.Count > 0)
        {
            var firstMajor = majorList[0];
            currentSelectedMajor = firstMajor.major;
            categoryItemPool.InitializePool(firstMajor.categories.Count);
        }

        foreach (var major in majorList)
        {
            MajorItem majorItem = Instantiate(majorItemPrefab, majorParent);
            majorItem.Initialize(major.major, OnMajorSelected);

            if (major.major == currentSelectedMajor)
            {
                foreach (var category in major.categories)
                {
                    var categoryItem = categoryItemPool.Get();
                    categoryItem.Initialize(major.major, category, OnMajorCategorySelected);
                }
            }
        }
    }

    private void OnMajorSelected(string majorName)
    {
        categoryItemPool.ReturnAll();
        currentSelectedMajor = majorName;

        var selectedMajor = majorList.Find(m => m.major == majorName);
        if (selectedMajor != null)
        {
            foreach (var category in selectedMajor.categories)
            {
                var categoryItem = categoryItemPool.Get();
                categoryItem.Initialize(majorName, category, OnMajorCategorySelected);
            }
            categoryScrollView.UpdateTotalItemCount(selectedMajor.categories.Count);
        }
    }

    private void OnMajorCategorySelected(string major, string category)
    {
        Debug.Log($"Selected Major: {major}, Category: {category}");
    }

    public void ClearCategories()
    {
        categoryItemPool.ReturnAll();
    }

    public void ShowMajorWithCategory(string major, string category)
    {
        var item = categoryItemPool.Get();
        item.Initialize(major, category, OnMajorCategorySelected);
    }
}