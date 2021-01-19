using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string animationLevelName { get { return m_animationLevelName; } }
    [SerializeField]
    private string m_animationLevelName;
    
    public string nextLevelName { get { return nextLevelName; } }
    [SerializeField]
    private string m_nextLevelName;
    [SerializeField]
    private string m_bufferLevelName;
    [SerializeField]
    private CanvasGroupAlpha m_canvasGroupAlpha;
    private string m_currentSceneName;
    [SerializeField]
    private bool m_animationSceneComplete = false;

    private void Start()
    {
        m_canvasGroupAlpha = GetComponent<CanvasGroupAlpha>();
    }

    public void LoadSceneCachedVariables()
    {
        LoadScene(m_animationLevelName, m_nextLevelName, m_bufferLevelName);
    }
    public void LoadScene(string animationLevelName, string nextLevelName, string bufferLevelName)
    {

        Scene currentScene = SceneManager.GetActiveScene();
        m_currentSceneName = currentScene.name;
        m_animationLevelName = animationLevelName;
        m_nextLevelName = nextLevelName;
        m_bufferLevelName = bufferLevelName;
        StartCoroutine(LoadLevel());
     
    }

    IEnumerator LoadLevel()
    {
        // Fade out and load a buffer level
        yield return StartCoroutine(FadeAndLoadBuffer());
        // Clear memory resources from the previous scene
        yield return StartCoroutine(ClearPreviousResources());
        // Load an animation scene to cover long load times for the next scene
        yield return StartCoroutine(LoadAnimationScene());
        // Load the next scene, wait until the animation scene has finished before starting.
        yield return StartCoroutine(LoadNextScene());
        // Unload the buffer and animations scenes, then clear resources.
        yield return StartCoroutine(UnloadBufferAndAnimationScenes());
        // Clear Scenehandler objects (this scene loader).
        ClearSceneLoader();        
     }
    IEnumerator FadeAndLoadBuffer()
    {
        //Call fade to black
        m_canvasGroupAlpha.Fade();
        //begin buffer-scene load but don't let it start.
        AsyncOperation asyncLoadOperation = SceneManager.LoadSceneAsync(m_bufferLevelName);
        asyncLoadOperation.allowSceneActivation = false;

        // buffer-scene is loading
        while (!asyncLoadOperation.isDone)
        {        
            // Once load and fade are both complete (because we don't want to activate the buffer scene before the fade has finished)
            if (asyncLoadOperation.progress >= 0.9f && m_canvasGroupAlpha.m_fadeComplete)
            {
                // Activate buffer level,
                asyncLoadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
        Debug.Log("Fadeout complete and buffer scene loaded");
    }
    
    IEnumerator ClearPreviousResources()
    {       
        // wait for unused resources to clear
        AsyncOperation asyncUnloadResources = Resources.UnloadUnusedAssets();
        while (!asyncUnloadResources.isDone)
        {
            yield return null;
        }

        Debug.Log("Resources cleared");
        
    }
    
    IEnumerator LoadAnimationScene()
    {
        // begin animation scene load
        AsyncOperation asyncLoadAnimSceneOperation = SceneManager.LoadSceneAsync(m_animationLevelName,LoadSceneMode.Additive);
        asyncLoadAnimSceneOperation.allowSceneActivation = false;

        while (!asyncLoadAnimSceneOperation.isDone)
        {

            if (asyncLoadAnimSceneOperation.progress >= 0.9f)
            {
                asyncLoadAnimSceneOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        Debug.Log("Animation Scene Loaded");
    }
    IEnumerator LoadNextScene()
    {
        AsyncOperation asyncLoadNextScene = SceneManager.LoadSceneAsync(m_nextLevelName,LoadSceneMode.Additive);
        asyncLoadNextScene.allowSceneActivation = false;

        while (!asyncLoadNextScene.isDone)
        {

            if (asyncLoadNextScene.progress >= 0.9f && m_animationSceneComplete)
            {
                asyncLoadNextScene.allowSceneActivation = true;
            }
            yield return null;
        }

        Debug.Log("New Scene Loaded");
        yield return null;
    }
    IEnumerator UnloadBufferAndAnimationScenes()
    {
        AsyncOperation asyncUnloadBufferSceneOperation = SceneManager.UnloadSceneAsync(m_bufferLevelName);
        AsyncOperation asyncUnloadAnimationSceneOperation = SceneManager.UnloadSceneAsync(m_animationLevelName);
        // wait for both scenes to clear
        while (!asyncUnloadBufferSceneOperation.isDone || !asyncUnloadAnimationSceneOperation.isDone)
        {
            yield return null;
        }

        // wait for unused resources to clear
        AsyncOperation asyncUnloadResources = Resources.UnloadUnusedAssets();
        while (!asyncUnloadResources.isDone)
        {
            yield return null;
        }

        Debug.Log("Buffer and Animation resources cleared");
    }
    
    // Called externally by an event on the animation scene when the animation has finished.
    public void AnimationSceneComplete()
    {
        m_animationSceneComplete = true;
    }
    private void ClearSceneLoader()
    {
     
        Debug.Log("Final Objects Cleared: Level Load operation complete");
        Destroy(this.gameObject);
    }
}
