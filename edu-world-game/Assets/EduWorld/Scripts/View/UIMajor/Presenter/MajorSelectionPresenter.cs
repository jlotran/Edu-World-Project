using System.Collections.Generic;
using System.Linq;
using Edu_World;

public class MajorSelectionPresenter
{
    private IMajorSelectionView view;
    private List<MajorData> data;

    public MajorSelectionPresenter(IMajorSelectionView view)
    {
        this.view = view;
        LoadData();
    }

    private void LoadData()
    {
        data = MajorLoader.LoadMajors();
        view.ShowMajors(data);
    }
}
