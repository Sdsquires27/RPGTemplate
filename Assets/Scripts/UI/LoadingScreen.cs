using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private string sceneToLoad;

    void Start()
    {
        StartCoroutine(LoadScene());
    }

private IEnumerator LoadScene()
{
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
    operation.allowSceneActivation = true;

    while (!operation.isDone)
    {
        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        
        if (progressBar != null)
            progressBar.fillAmount = progress;
        
        if (loadingText != null)
            loadingText.text = $"Loading... {(int)(progress * 100)}%";

        yield return null;
    }

    // Scene is fully loaded, now wait for input
    if (loadingText != null)
        loadingText.text = "Press any key to continue...";

    yield return new WaitUntil(() => 
        UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame);
    
    SceneManager.LoadScene(sceneToLoad); // won't be needed if allowSceneActivation handled it
}
}