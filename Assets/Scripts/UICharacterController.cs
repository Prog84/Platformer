using UnityEngine.UI;
using UnityEngine;

public class UICharacterController : MonoBehaviour
{
    [SerializeField] private PressedButton left;
    public PressedButton Left
    {
        get { return left; }
    }
    [SerializeField] private PressedButton right;
    public PressedButton Right
    {
        get { return right; }
    }

    [SerializeField] private Button fire;
    public Button Fire
    {
        get { return fire; }
    }
    [SerializeField] private Button jump;
    public Button Jump
    {
        get { return jump; }
    }
    void Start()
    {
        Player.Instance.InitUIController(this);
    }

}
