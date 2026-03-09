using UnityEngine;

public class LoadSceneConnector : MonoBehaviour
{
    public void Load(int sceneIndx)
    {
        LoadScene loader = FindObjectOfType<LoadScene>();
        loader.LoadSceneFadeScreenToOpaque(sceneIndx);
    }
}
