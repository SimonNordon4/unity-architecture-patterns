using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LockPosition : MonoBehaviour
{
    private RectTransform _rectTransform;

    public float positionX;
    public float positionY;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(positionX, positionY);
    }

    [ContextMenu("Save Position")]
    public void SavePosition()
    {
        positionX = _rectTransform.anchoredPosition.x;
        positionY = _rectTransform.anchoredPosition.y;
    }

    private void OnValidate()
    {
        _rectTransform = GetComponent<RectTransform>();
        SavePosition();
    }
}
