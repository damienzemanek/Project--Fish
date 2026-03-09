using UnityEngine;

public class LoadSceneConnector : MonoBehaviour
{
    public void Load(int sceneIndx)
    {
        LoadScene loader = FindAnyObjectByType<LoadScene>();
        loader.LoadSceneFadeScreenToOpaque(sceneIndx);
    }
}
