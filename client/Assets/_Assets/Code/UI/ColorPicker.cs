using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ColorPicker : MonoBehaviour
{    
    public GameObject hueObject;
    public GameObject saturationObject;
    public GameObject vibranceObject;
	public Material colorWheelMat;

	public TextMeshProUGUI hexColor;

    public Image selectedColor;

    public Image hueImage;
    public Image saturationImage;
    public Image vibranceImage;

    public UIGradient saturationGradient;
    public UIGradient vibranceGradient;

    public Slider saturation;
    public Slider vibrance;
    public RadialSlider hue;

    private Gradient gradient;

    public Constraints constraints{
        get{
            return _constraints;
        }
        set{
            hue.constraints = value.hue;
            _constraints = value;
        }
    }
    private Constraints _constraints = new Constraints();
    public class Constraints
    {
        public Vector2 hue = new Vector2(0, 1);
        public Vector2 saturation = new Vector2(0, 1);
        public Vector2 vibrance = new Vector2(0, 1);
    }

    public delegate void ValueChanged(Color color);
    public ValueChanged onValueChanged;

    public System.Action<Color> onColorUpdate;
    
    public Color color {
        get{ 
            return selectedColor.color;
        }  
        set{
            Color.RGBToHSV(value, out float h, out float s, out float v);

            hue.value = h;
            saturation.value = s;
            vibrance.value = v;

            UpdateColorPicker();
        }
    }

    private void UpdateColorPicker()
    {
        Vector3 hsv = new Vector3(Mathf.Lerp(constraints.hue.x, constraints.hue.y, hue.value), Mathf.Lerp(constraints.saturation.x, constraints.saturation.y, saturation.value), Mathf.Lerp(constraints.vibrance.x, constraints.vibrance.y, vibrance.value));      
        
        saturationGradient.m_color1 = Color.HSVToRGB(hsv.x, constraints.saturation.x, hsv.z);
        saturationGradient.m_color2 = Color.HSVToRGB(hsv.x, constraints.saturation.y, hsv.z);
        saturationGradient.enabled = false;
        saturationGradient.enabled = true;

        vibranceGradient.m_color1 = Color.HSVToRGB(hsv.x, hsv.y, constraints.vibrance.x);
        vibranceGradient.m_color2 = Color.HSVToRGB(hsv.x, hsv.y, constraints.vibrance.y);
        vibranceGradient.enabled = false;
        vibranceGradient.enabled = true;

		colorWheelMat.SetFloat("_Saturation", hsv.y);
		colorWheelMat.SetFloat("_Vibrance", hsv.z);

        selectedColor.color = vibranceImage.color = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

        var gradientColorKey = new GradientColorKey[2];

        hueImage.color = Color.HSVToRGB(hue.value, hsv.y, hsv.z);
        saturationImage.color = Color.Lerp(saturationGradient.m_color1,  saturationGradient.m_color2, saturation.value);

		//hexColor.text = $"<mspace=0.55em>#{ColorUtility.ToHtmlStringRGB(selectedColor.color)}</mspace>";
    }

    private void OnValueChanged(float value)
    {
        UpdateColorPicker();

        onValueChanged?.Invoke(color);
        onColorUpdate?.Invoke(color);
    }

    private void OnEnable()
    {
        hue.onValueChanged += OnValueChanged;
        saturation.onValueChanged.AddListener(OnValueChanged);
        vibrance.onValueChanged.AddListener(OnValueChanged);
    }

    private void Unsubscribe()
    {
        hue.onValueChanged -= OnValueChanged;
        saturation.onValueChanged.RemoveListener(OnValueChanged);
        vibrance.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
