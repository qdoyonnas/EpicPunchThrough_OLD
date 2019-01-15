using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transitional))]
public class TransitionalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Transitional transitionalScript = (Transitional)target;
        RectTransform rectTransform = transitionalScript.gameObject.GetComponent<RectTransform>();

        if( GUILayout.Button("Save In Position") ) {
            transitionalScript.inPosition = rectTransform.anchoredPosition;
        }
        if( GUILayout.Button("Load In Position") ) {
            rectTransform.anchoredPosition = transitionalScript.inPosition;
        }
        if( GUILayout.Button("Save Out Position") ) {
            transitionalScript.outPosition = rectTransform.anchoredPosition;
        }
        if( GUILayout.Button("Load Out Position") ) {
            rectTransform.anchoredPosition = transitionalScript.outPosition;
        }
    }
}
