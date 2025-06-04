using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool buttonPressed;
    public string buttonName = "";

    void Start()
    {
        buttonName = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
        {
            new DebugItem<bool>("Button pressed?", buttonPressed);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) 
    {
        buttonPressed = true;    
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }

}
