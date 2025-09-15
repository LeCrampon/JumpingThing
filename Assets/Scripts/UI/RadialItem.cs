using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialItem : MonoBehaviour
{
    [SerializeField]
    private float _characterIndex;

    private bool _highlighted = false;

    [SerializeField]
    private Sprite _defaultSprite, _highlightedSprite;
    [SerializeField]
    private Image _image;

    public bool IsHighlighted()
    {
        return _highlighted;
    }

    public void SetHighlighted(bool highlighted)
    {
        _highlighted = highlighted;
        if (highlighted)
        {
            _image.sprite = _highlightedSprite;
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            _image.sprite = _defaultSprite;
            transform.localScale = Vector3.one;
        }
           
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
