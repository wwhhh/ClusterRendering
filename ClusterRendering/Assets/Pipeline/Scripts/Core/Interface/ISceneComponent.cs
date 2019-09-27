using UnityEngine;
using System.Collections;

public interface ISceneComponent
{

    void LoadScene(string scene, int instanceCount);

    void UnLoadScene();

}
