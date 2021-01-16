using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateIcon : MonoBehaviour
{
    public RenderTexture rTex;
    public Camera cam;

#if UNITY_EDITOR
    public void SaveIcon()
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "png");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        cam.targetTexture = rTex;
        cam.Render();
        RenderTexture.active = rTex;

        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false );
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.alphaIsTransparency = true;
        tex.wrapMode = TextureWrapMode.Clamp;

        RenderTexture.active = null;
        cam.targetTexture = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        System.IO.File.WriteAllBytes(path, bytes);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(CreateIcon))]
public class CreateIconEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Save Icon"))
        {
            (target as CreateIcon).SaveIcon();
        }
    }

}
#endif
