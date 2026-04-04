using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class FinisherSlider : MonoBehaviour
{
    public SplineAnimate indicator;


    public void StartEvent()
    {
        indicator.Play();
    }
}
