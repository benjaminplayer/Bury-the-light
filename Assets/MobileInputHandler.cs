using UnityEngine;
using UnityEngine.UI;

public class MobileInputHandler : MonoBehaviour
{
    [SerializeField] private Button[] btns; // 0 left, 1 right, 2 down, 3 space, 4 use
    [SerializeField] private Vector2 input = Vector2.zero;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (btns[0].GetComponent<ButtonPressed>().buttonPressed)
            input = new Vector2(-1, 0);
        else if(btns[1].GetComponent<ButtonPressed>().buttonPressed)
            input = new Vector2(1, 0);
        else
            input = Vector2.zero;
    }

    public float GetHorizontalInput()
    {
        return input.x;
    }

    public float GetVerticalInput()
    {
        return input.y;
    }

    public Vector2 GetInput() 
    {
        return input;
    }

    public bool JumpPressed()
    { 
        return btns[3].GetComponent<ButtonPressed>().buttonPressed;
    }

    public bool UsePressed()
    { 
        return btns[4].GetComponent<ButtonPressed>().buttonPressed;
    }

}
