using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLoadScene : MonoBehaviour
{
    public string m_loadScene;
    public void LoadScene()
    {
        SceneManager.LoadScene(m_loadScene, LoadSceneMode.Single);
    }
}
