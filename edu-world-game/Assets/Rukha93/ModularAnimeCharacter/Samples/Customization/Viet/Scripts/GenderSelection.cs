using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Fusion.Menu;
using Rukha93.ModularAnimeCharacter.Customization.UI;

namespace Edu_World_Game
{
    public class GenderSelection : MonoBehaviour
    {
        public static bool IsMale { get; private set; } = true;

        [SerializeField] private RectTransform panelChoose, panelGender, panelCustomize;
        [SerializeField] private Button buttonMale, buttonFemale, ConfirmButton;
        [SerializeField] private RectTransform malePos, femalePos;
        [SerializeField] private RectTransform ellipseMale, checkMale;
        [SerializeField] private RectTransform ellipseFemale, checkFemale;
        [SerializeField] private RectTransform buttonConfirmPos;
        private void Start()
        {
            buttonMale.onClick.AddListener(() => SelectCharacter(true));
            buttonFemale.onClick.AddListener(() => SelectCharacter(false));

            // Set initial state - Male selected by default
            ellipseMale.localScale = Vector3.one * 1.2f;
            checkMale.localScale = Vector3.one * 1.2f;
            ellipseFemale.localScale = Vector3.zero;
            checkFemale.localScale = Vector3.zero;
            ConfirmButton.onClick.AddListener(ConfirmSelection);
            buttonConfirmPos.anchoredPosition = new Vector2(buttonConfirmPos.anchoredPosition.x, -149f);
            buttonConfirmPos.DOAnchorPosY(80f, 1f).SetEase(Ease.InOutSine);
        }

        private void SelectCharacter(bool isMale)
        {
            IsMale = isMale;

            if (isMale)
            {
                // Di chuyển PanelChoose đến vị trí nhân vật nam
                panelChoose.DOMoveX(malePos.position.x, 0.3f).SetEase(Ease.OutQuad);

                // Phóng to Ellipse + Check của Male
                ellipseMale.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad);
                checkMale.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad);

                // Thu nhỏ Ellipse + Check của Female
                ellipseFemale.DOScale(0f, 0.2f).SetEase(Ease.InQuad);
                checkFemale.DOScale(0f, 0.2f).SetEase(Ease.InQuad);
            }
            else
            {
                // Di chuyển PanelChoose đến vị trí nhân vật nữ
                panelChoose.DOMoveX(femalePos.position.x, 0.3f).SetEase(Ease.OutQuad);

                // Thu nhỏ Ellipse + Check của Male
                ellipseMale.DOScale(0f, 0.2f).SetEase(Ease.InQuad);
                checkMale.DOScale(0f, 0.2f).SetEase(Ease.InQuad);

                // Phóng to Ellipse + Check của Female
                ellipseFemale.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad);
                checkFemale.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad);
            }
        }

        private void ConfirmSelection()
        {
            // Save gender selection to PlayerPrefs
            PlayerPrefs.SetString("Customization_gender", IsMale ? "m" : "f");
            PlayerPrefs.Save();

            // Animation sequence for panel transition
            Sequence sequence = DOTween.Sequence();

            // Move gender panel up and fade out
            sequence.Append(panelGender.DOAnchorPosY(800f, 0.5f).SetEase(Ease.InBack));
            sequence.Join(panelGender.GetComponent<CanvasGroup>().DOFade(0f, 0.5f));

            // After gender panel moves out, move in customize panel
            sequence.AppendCallback(() =>
            {
                panelCustomize.gameObject.SetActive(true);
                panelCustomize.anchoredPosition = new Vector2(panelCustomize.anchoredPosition.x, -800f);
            });

            // Animate customize panel moving in from bottom
            sequence.Append(panelCustomize.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack));

            // After all animations complete, disable gender panel completely
            sequence.OnComplete(() =>
            {
                // Controller.Show<UICustomizationDemo>();
            });
        }

    }

}
