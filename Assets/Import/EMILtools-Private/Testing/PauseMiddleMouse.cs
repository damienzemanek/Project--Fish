using UnityEngine;

public class PauseOnMiddleClick : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            Debug.Break();
        }
#endif
    }
}