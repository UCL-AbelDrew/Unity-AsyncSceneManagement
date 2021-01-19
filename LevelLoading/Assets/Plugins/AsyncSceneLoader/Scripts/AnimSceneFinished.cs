using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSceneFinished : MonoBehaviour
{
    private GameObject m_sceneLoaderObject;
    private SceneLoader m_sceneLoader;
    // get the scene loader

    private void Start()
    {
        m_sceneLoaderObject = GameObject.Find("SceneLoader");
        m_sceneLoader = m_sceneLoaderObject.GetComponent<SceneLoader>();
    }

    public void NotifySceneLoader()
    {
        if (m_sceneLoader) {
            m_sceneLoader.AnimationSceneComplete();
        }
        else
        {
            Debug.LogError("Scene Loader Object Not Found - AnimFinished.cs");
        }
    }
}
