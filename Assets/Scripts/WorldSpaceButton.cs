using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class WorldSpaceButton : MonoBehaviour
{

    public UnityEvent OnClick;
    public UnityEvent OnHover;
    private void OnMouseDown()
    {
        OnClick.Invoke();
    }

    private void OnMouseEnter()
    {
        transform.DOScale(1.1f, 0.2f);
    }
    private void OnMouseExit()
    {
        transform.DOScale(1f, 0.2f);
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
