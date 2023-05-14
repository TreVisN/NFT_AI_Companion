using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneButton : MonoBehaviour
{
    private Image _image;
    private Color _initialColor;

    private void Start()
    {
        _image = GetComponent<Image>();
        _initialColor = _image.color;
    }
    public void Answer(bool answer)
    {
        
        if (answer)
        {
            _image.color = Color.green;
            Debug.Log("Correct!");
        }
        else
        {
            _image.color = Color.red;
            Reset();
            Debug.Log("Wrong!");
        }
        
    }
    
    public void Reset()
    {
        WisdomStone.Instance.HideDialog();
        _image.color = _initialColor;
    }
}
