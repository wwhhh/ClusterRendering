using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : MonoBehaviour
{
    /// <summary>
    ///  要加载的场景名字
    /// </summary>
    public string sceneName;

    void Start()
    {
        SceneController.instance.LoadScene(sceneName);
    }

}
