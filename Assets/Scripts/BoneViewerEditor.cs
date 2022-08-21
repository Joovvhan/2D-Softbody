using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.IO;

[CanEditMultipleObjects]
[CustomEditor(typeof(BoneViewer))]

public class BoneViewerEditor : Editor
{
    [SerializeField]
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private SpriteSkin spriteSkin;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoneViewer boneViewer = (BoneViewer)target;

        //DrawDefaultInspector();
        EditorGUILayout.LabelField("My Custom Bone Viewer Editor");
        ;
        BoneViewer boneViewr = (BoneViewer)target;

        if (GUILayout.Button("Fetch Sprite Informations"))
        {
            sprite = boneViewer.GetCurrentSprite();
            spriteRenderer = boneViewer.GetCurrentSpriteRender();
            spriteSkin = boneViewer.GetCurrentSpriteSkin();
        }

        if (GUILayout.Button("Get Sprite Bones"))
        {
            SerializedObject serializedObject = new SerializedObject(sprite);
            serializedObject.Update();
            SerializedProperty m_RootBoneProperty;
            m_RootBoneProperty = serializedObject.FindProperty("m_Bones");
            Debug.Log(m_RootBoneProperty.name); // UnityEditor.SerializedProperty
            Debug.Log(m_RootBoneProperty.arraySize);

            EditorUtility.SetDirty(sprite);
        }


        if (GUILayout.Button("Get SpriteSkin Bones"))
        {
            Transform[] skinBones = spriteSkin.boneTransforms;
            Debug.Log(skinBones.Length);
        }

        if (GUILayout.Button("Get SpriteSkin Root Bone"))
        {
            Transform rootBone = spriteSkin.rootBone;
            Debug.Log(rootBone);
        }

        if (GUILayout.Button("Get Sprite Path"))
        {
            string assetPath = AssetDatabase.GetAssetPath(sprite);
            Debug.Log(assetPath);
            boneViewer.DebugSpritePath();
            Debug.Log(sprite.GetInstanceID());
        }

        if (GUILayout.Button("Get SpriteRender Sprite Path"))
        {
            Sprite localSprite = spriteRenderer.sprite;
            string assetPath = AssetDatabase.GetAssetPath(localSprite);
            Debug.Log(assetPath);
            Debug.Log(localSprite.GetInstanceID());
            //boneViewer.DebugSpriteRenderPath();
        }

        if (GUILayout.Button("Display Sprite Info"))
        {
            Sprite localSprite = spriteRenderer.sprite;
            Vector2 pivot = localSprite.pivot;
            Rect rect = localSprite.rect;
            Debug.Log(pivot);
            Debug.Log(rect);
        }

        if (GUILayout.Button("Copy Sprite Asset"))
        {
            Sprite localSprite = spriteRenderer.sprite;
            Sprite newSprite = Sprite.Create(localSprite.texture, localSprite.rect, localSprite.pivot, localSprite.pixelsPerUnit);
            string assetPath = AssetDatabase.GetAssetPath(localSprite);
            Debug.Log(assetPath);
            string newAssetPath = assetPath.Replace(".png", "_.png");
            //AssetDatabase.CreateAsset(newSprite, newAssetPath);

            Texture2D tex = newSprite.texture;
            Debug.Log(tex.format);
            byte[] png = tex.EncodeToPNG();

            File.WriteAllBytes(newAssetPath, png);
            //AssetDatabase.Refresh();
            //AssetDatabase.AddObjectToAsset(newSprite, newAssetPath);
            //AssetDatabase.SaveAssets();

            TextureImporter ti = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;

            //ti.spritePixelsPerUnit = newSprite.pixelsPerUnit;
            //ti.spritePivot = newSprite.pivot;
            ti.spritePivot = localSprite.pivot;
            Debug.Log(localSprite.pivot);
            Debug.Log(newSprite.pivot);
            Debug.Log(ti.spritePivot);
            EditorUtility.SetDirty(ti);
            ti.SaveAndReimport();

        }

        if (GUILayout.Button("Modify Meta File Directly"))
        {
            boneViewer.DebugSpriteGetBones();
        }

        if (GUILayout.Button("Refresh Asset Data Base"))
        {
            AssetDatabase.Refresh();
        }

        //if (GUILayout.Button("Get SpriteSkin Path"))
        //{
        //    string assetPath = AssetDatabase.GetAssetPath(spriteSkin);
        //    Debug.Log(assetPath);
        //    boneViewer.DebugSpriteSkinPath();
        //}


    }
}
