using UnityEngine;

public class LoadSceneConnector : MonoBehaviour
{
    public void Load(int sceneIndx)
    {
        Debug.Log("Attempting to load new level: " + sceneIndx);

        LoadScene loader = FindAnyObjectByType<LoadScene>();
        loader.LoadSceneFadeScreenToOpaque(sceneIndx);
    }
}
