using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public static bool DoesSceneExist( string sceneName )
    {
        List<string> sceneNames = new List<string>();
        for( int i = 0; i < SceneManager.sceneCountInBuildSettings; i++ ) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = scenePath.LastIndexOf("/");
            int lastDot = scenePath.LastIndexOf(".");
            sceneNames.Add( scenePath.Substring(lastSlash + 1, lastDot - lastSlash - 1) );
        }

        return sceneNames.Contains(sceneName);
    }
}
