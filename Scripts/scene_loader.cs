using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scene_loader : Singleton<scene_loader>
{
    private string scenetobeloaded;

    public void onload(string namescene)
    {
        scenetobeloaded = namescene;

        StartCoroutine(initializes_scene_loading());
    }
    IEnumerator initializes_scene_loading()
    {
        yield return SceneManager.LoadSceneAsync("Scene_Loading");
        StartCoroutine("Actual_Scene_Loading");
    }
    IEnumerator Actual_Scene_Loading()
    {
        var asyncsceneloading = SceneManager.LoadSceneAsync(scenetobeloaded);
        asyncsceneloading.allowSceneActivation = false;
        while(!asyncsceneloading.isDone)
        {
            if(asyncsceneloading.progress>=0.9f)
            {
                asyncsceneloading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
