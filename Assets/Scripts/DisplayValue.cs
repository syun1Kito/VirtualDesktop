using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayValue : MonoBehaviour
{
    [SerializeField]
    //Text text;
    InputField inputField;
    [SerializeField]
    Slider slider;
    float preValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (slider.value != preValue)
        {
            inputField.text = slider.value.ToString("f2");
        }

        preValue = slider.value;
    }

    public void InputText()
    {
        //スライダーにinputFieldの内容を反映
        slider.value = float.Parse(inputField.text);

    }
}
