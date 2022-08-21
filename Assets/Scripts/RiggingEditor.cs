using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;

[CanEditMultipleObjects]
[CustomEditor(typeof(AutoRigging))]
public class RiggingEditor : Editor
{
    //[SerializeField]
    //private int boneNums;
    private SerializedProperty m_RootBoneProperty;
    private SerializedProperty m_BoneTransformsProperty;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();
        EditorGUILayout.LabelField("My Custom Rigging Editor");
        ;
        AutoRigging autoRigging = (AutoRigging)target;

        //if (GUILayout.Button("Auto Create Rigging"))
        //{
        //    Debug.Log("Rigging Created");
        //    autoRigging.TestFunction();
        //}

        if (GUILayout.Button("Generate Sprite Bones"))
        {
            autoRigging.GenerateSpriteBones();
        }

        if (GUILayout.Button("Delete Sprite Bones"))
        {
            autoRigging.DeleteSpriteBones();
        }

        if (GUILayout.Button("Create Joints"))
        {
            Debug.Log("Create Joints Here");
        }

        if (GUILayout.Button("Delete Bone Objects"))
        {
            autoRigging.RemoveGameObjectBones();
        }

        if (GUILayout.Button("Attach Physics 2D"))
        {
            autoRigging.AttachRigidBody2GameObjectBones();
            autoRigging.AttachCircleCollider2GameObjectBones();
            autoRigging.AttachSpringJoint2GameObjectBones();
            autoRigging.AttachDistanceJoint2GameObjectBones();
            autoRigging.AttachWrappingSpringJoint2GameObjectBones();
        }

        if (GUILayout.Button("Attach Jelly"))
        {
            autoRigging.AttachJelly2GameObjectBones();
        }

        if (GUILayout.Button("Copy Sprite Asset"))
        {
            autoRigging.CopySprite();
        }

        if (GUILayout.Button("Debug Sprite Vertices"))
        {
            autoRigging.DebugSpriteVetrices();
        }

        if (GUILayout.Button("Debug Sprite Triangles"))
        {
            autoRigging.DebugSpriteTriangles();
        }

        //if (GUILayout.Button("Get Root Bone"))
        //{
        //    Debug.Log("Root Bone Information");
        //    autoRigging.GetRootBone();
        //}
    }

    private void DoGenerateBonesButton()
    {
        //if (GUILayout.Button("Create Bones", GUILayout.MaxWidth(125f)))
        //{
        //    foreach (var t in targets)
        //    {
        //        var spriteSkin = t as SpriteSkin;
        //        var sprite = spriteSkin.spriteRenderer.sprite;

        //        if (sprite == null || spriteSkin.rootBone != null)
        //            continue;

        //        Undo.RegisterCompleteObjectUndo(spriteSkin, "Create Bones");

        //        spriteSkin.CreateBoneHierarchy();

        //        foreach (var transform in spriteSkin.boneTransforms)
        //            Undo.RegisterCreatedObjectUndo(transform.gameObject, "Create Bones");

        //        ResetBoundsIfNeeded(spriteSkin);

        //        EditorUtility.SetDirty(spriteSkin);
        //    }
        //    //BoneGizmo.instance.boneGizmoController.OnSelectionChanged();
        //}
    }
}
