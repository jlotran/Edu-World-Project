using System.Collections.Generic;
using Edu_World;

public interface IMajorSelectionView
{
    void ShowMajors(List<MajorData> majorList);
    void ShowMajorWithCategory(string major, string category);
    void ClearCategories();
}
