using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Script for controlling the different bars showing amount of an int
// Most off them being EnergyBar or HealthBar
public class BarScript : MonoBehaviour
{
    [Header("Bar Stuff")]
    public TextMeshProUGUI Text;
    public Slider Slider;

    public int CurrentValue;
    public int MaxValue;

    [Header("Block Indicator")]
    public GameObject BlockIndicator;
    public TextMeshProUGUI BlockText;

    [Header("Stuff for showing BarValue")]
    public Gradient FillGradient;
    public Image Fill;

    public void SetupBar(int MaxVal, Color Color)
    {
        MaxValue = MaxVal;

        GradientColorKey[] GradientColor = new GradientColorKey[1];
        GradientColor[0] = new GradientColorKey(Color, 0.0f);

        GradientAlphaKey[] Alpha = new GradientAlphaKey[1];
        Alpha[0] = new GradientAlphaKey(1.0f, 0.0f);

        FillGradient.SetKeys(GradientColor, Alpha);

        ResetBar();
    }

    public void UpdateDefence(int DefenceAmount)
    {
        BlockText.text = DefenceAmount.ToString();

        if (DefenceAmount > 0)
        {
            BlockIndicator.SetActive(true);
        }
        else
        {
            BlockIndicator.SetActive(false);
        }
    }

    public void ResetBar()
    {
        CurrentValue = MaxValue;

        Slider.maxValue = MaxValue;
        Slider.value = CurrentValue;

        Text.text = $"{CurrentValue} / {MaxValue}";
    }

    public void UpdateBar(int ChangingValue)
    {
        // Send positive value to add to CurrentValue, Send negative value to remove from CurrentValue

        CurrentValue += ChangingValue;

        if (CurrentValue > MaxValue)
        {
            CurrentValue = MaxValue;
        }

        Slider.value = CurrentValue;

        Fill.color = FillGradient.Evaluate(Slider.normalizedValue); ;

        Text.text = $"{CurrentValue} / {MaxValue}";
    }
}
