using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{

    [SerializeField] Image confirmIcon;
    [SerializeField] TMP_Text keyLabel;
    [SerializeField] TMP_Text label;

    private InteractOption optionData;

    public InteractOption OptionData
    {
        get { return optionData; }

        set
        {
            optionData = value;
            label.text = value.label;
            keyLabel.text = value.requiredKey.ToString();
            confirmIcon.fillAmount = 0;
        }
    }

    public float Fill
    {
        get { return confirmIcon.fillAmount; }
        set { confirmIcon.fillAmount = value; }
    }

    public string KeyLabel
    {
        get { return keyLabel.text; }
        set { keyLabel.text = value; }
    }

    public string Label
    {
        get { return label.text; }
        set { label.text = value; }
    }

    public Image IconProperties { get { return confirmIcon; } }
    public TMP_Text KeyLabelProperties { get { return keyLabel; } }
    public TMP_Text LabelProperties { get { return label; } }

}
