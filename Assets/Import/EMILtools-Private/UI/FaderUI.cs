using UnityEngine;

public class FaderUI : MonoBehaviour
{
    public static FaderUI Instance;

    public Object targ;
    
    private void Awake()
    {
        Instance = this;
    }
}
