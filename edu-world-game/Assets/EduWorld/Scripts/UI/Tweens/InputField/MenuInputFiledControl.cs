using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_InputField),typeof(InputFieldTweens))]
public class MenuInputFiledControl : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;

    public InputFieldTweens inputFieldTweens;

    private void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
        inputFieldTweens = GetComponent<InputFieldTweens>();
        inputField.onSelect.AddListener(OnSelect);
        inputField.onDeselect.AddListener(OnDeselect);
        inputField.onEndEdit.AddListener(OnEndEdit);
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(string arg0)
    {

    }

    private void OnEndEdit(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            inputFieldTweens.DoPlaceHolderMoveBack();
        }
    }

    private void OnDeselect(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            inputFieldTweens.DoPlaceHolderMoveBack();
        }
        inputFieldTweens.DoHighLightBack();
    }

    private void OnSelect(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            inputFieldTweens.DoPlaceHolderMove();
        }
        inputFieldTweens.DoHighLight();
    }
}
