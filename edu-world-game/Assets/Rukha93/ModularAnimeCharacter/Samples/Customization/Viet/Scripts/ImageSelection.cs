using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageSelection : MonoBehaviour
{
    public GameObject imageChoose; // ImageChoose object (ban đầu ẩn)
    public Image imageItem; // Icon của ImageItem
    public Sprite selectedSprite; // Hình ảnh khi được chọn
    public Sprite unselectedSprite; // Hình ảnh khi chưa chọn
    
    private bool isSelected = false;

    void Start()
    {
        imageChoose.SetActive(false); // Ẩn ImageChoose ban đầu
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        
        if (isSelected)
        {
            imageChoose.SetActive(true);
            imageChoose.transform.DOLocalMoveX(0, 0.3f).From(new Vector3(-100, 0, 0)).SetEase(Ease.OutBack); // Đi từ bên trái vào
            imageItem.sprite = selectedSprite; // Chuyển ảnh sang trạng thái được chọn
        }
        else
        {
            imageChoose.SetActive(false);
            imageItem.sprite = unselectedSprite; // Quay về ảnh ban đầu
        }
    }
}
