using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Scene Loading")]
    public bool loadOnStart = false;
    public string sceneToLoadName;

    [Header("Activation Control")]
    public bool allowActivationOnLoad = false; // Inspector toggle

    [Header("Progress Bar")]
    public GameObject progressBar;
    public Material progressBarMaterial;

    [Header("Optional Settings")]
    public float minDisplayTime = 1f; // Keep the bar on-screen briefly even if loading is fast

    private AsyncOperation preloadOp;

    void Start()
    {
        if (loadOnStart)
            PreloadScene();
    }

    /// <summary>
    /// Starts preloading the scene in the background.
    /// </summary>
    public void PreloadScene()
    {
        preloadOp = SceneManager.LoadSceneAsync(sceneToLoadName);
        preloadOp.allowSceneActivation = allowActivationOnLoad; // depends on inspector
        StartCoroutine(UpdateProgressBar());
    }

    /// <summary>
    /// Call this to activate the scene after your fade animation or other events.
    /// </summary>
    public void ActivateScene()
    {
        if (preloadOp != null)
            preloadOp.allowSceneActivation = true;
    }

    private IEnumerator UpdateProgressBar()
    {
        float timer = 0f;

        while (preloadOp != null && preloadOp.progress < 0.9f)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(preloadOp.progress / 0.9f);

            if (progressBarMaterial != null)
                progressBarMaterial.SetFloat("_Progress", progress);

            yield return null;
        }

        // Ensure progress bar is full while waiting for activation
        if (progressBarMaterial != null)
            progressBarMaterial.SetFloat("_Progress", 1f);

        // Optional brief delay so the player sees full bar
        yield return new WaitForSeconds(minDisplayTime);
        progressBar.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
