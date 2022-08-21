using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D;
using UnityEditor;
using Unity.Collections;
using System.IO;

public class AutoRigging : MonoBehaviour
{
    [SerializeField]
    private int boneNum;
    [SerializeField]
    private float boneColliderRadius, boneMass, centerFreq, wrapFreq;

    private SerializedProperty m_RootBoneProperty;
    private SpriteSkin spriteSkin;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestFunction()
    {
        Debug.Log("Test Function Inside AutoRigging Script");
        spriteSkin = GetComponent<SpriteSkin>();
        var sprite = GetComponent<SpriteRenderer>().sprite;

        SerializedObject serializedObject = new SerializedObject(sprite);
        serializedObject.Update();
        Debug.Log("Before the Serialization");
        m_RootBoneProperty = serializedObject.FindProperty("m_Bones");
        Debug.Log("Serialized Value");
        Debug.Log(m_RootBoneProperty.name); // UnityEditor.SerializedProperty
        Debug.Log(m_RootBoneProperty.arraySize);

        string assetPath = AssetDatabase.GetAssetPath(sprite);
        Debug.Log(assetPath);

        Debug.Log(sprite.name);
        spriteSkin.alwaysUpdate = true;

        int boneNum = 36;
        float stepAngle = 360f / boneNum;

        SpriteBone[] newBones = new SpriteBone[boneNum];
        Matrix4x4[] newPoses = new Matrix4x4[boneNum];

        for (int i = 0; i < boneNum; i++)
        {
            string boneName = "bone_" + (i+1).ToString();

            float angle = stepAngle * i;
            Vector3 eulerAngle = new Vector3(0f, 0f, angle + 180);

            Vector3 position = new Vector3(2 * Mathf.Cos(Mathf.Deg2Rad * angle), 2 * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            Quaternion rotation = Quaternion.Euler(eulerAngle);

            newBones[i] = new SpriteBone() { name = boneName, position = position, rotation = rotation, length = 0.5f, parentId = -1 };
            Matrix4x4 pose = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, 1f)).inverse;
            newPoses[i] = pose;
        }
        sprite.SetBones(newBones);

        NativeArray<Matrix4x4> newNativePoses = new NativeArray<Matrix4x4>(newPoses, Allocator.Temp);
        sprite.SetBindPoses(newNativePoses);

        bool updated = serializedObject.ApplyModifiedProperties();
        Debug.Log(updated);

        m_RootBoneProperty = serializedObject.FindProperty("m_Bones");

        Debug.Log("After Creating Bones");
        Debug.Log("Serialized Value");
        Debug.Log(m_RootBoneProperty.name); // UnityEditor.SerializedProperty
        Debug.Log(m_RootBoneProperty.GetArrayElementAtIndex(0).name);
        Debug.Log(m_RootBoneProperty.arraySize);

        SerializedObject serializedObject2 = new SerializedObject(sprite);
        serializedObject.Update();
        Debug.Log("After the Serialization");
        m_RootBoneProperty = serializedObject2.FindProperty("m_Bones");
        Debug.Log("Serialized Value");
        Debug.Log(m_RootBoneProperty.name); // UnityEditor.SerializedProperty
        Debug.Log(m_RootBoneProperty.arraySize);


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string assetPath2 = AssetDatabase.GetAssetPath(sprite);
        Debug.Log(assetPath2);
        string newAssetPath = assetPath2.Replace(".png", "2.png");

        var spriteBones = sprite.GetBones();
        var boneTransforms = spriteSkin.boneTransforms;

        for (int i = 0; i < boneTransforms.Length; ++i)
        {
            var boneTransform = boneTransforms[i];
            var spriteBone = spriteBones[i];

            if (spriteBone.parentId != -1)
            {
                boneTransform.localPosition = spriteBone.position;
                boneTransform.localRotation = spriteBone.rotation;
                boneTransform.localScale = Vector3.one;
            }
        }

        //Debug.Log(newAssetPath);
        AssetDatabase.CopyAsset(assetPath2, newAssetPath);
    }

    public void CopySprite()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        string newAssetPath = assetPath.Replace(".png", "_.png");
        AssetDatabase.CopyAsset(assetPath, newAssetPath);
        Debug.Log(assetPath + " -> " + newAssetPath);
    }

    public void DeleteSpriteBones()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        string metaPath = assetPath + ".meta";
        
        var lines = File.ReadLines(metaPath);
        List<string> newLines = new List<string>();

        bool doDeleteBone = true;

        foreach (string line in lines)
        {

            if (line == "    bones: []")
            {
                doDeleteBone = false;
            }
        }

        if (!doDeleteBone)
        {
            Debug.Log("Skipping Bone Reset Process, Already None.");
            return;
        }
        else
        {
            lines = File.ReadLines(metaPath);
            bool continueWriting = true;

            foreach (string line in lines)
            {

                if (line == "    bones:")
                {
                    continueWriting = false;
                    newLines.Add("    bones: []\n");
                }
                else if (line.Contains("spriteID: "))
                {
                    newLines.Add(line);
                    string dummyLine = "" +
                        "    internalID: 0\n" +
                        "    vertices: []\n" +
                        "    indices: \n" +
                        "    edges: []\n" +
                        "    weights: []\n" +
                        "    secondaryTextures: []\n" +
                        "  spritePackingTag: \n" +
                        "  pSDRemoveMatte: 0\n" +
                        "  pSDShowRemoveMatteOption: 0\n" +
                        "  userData: \n" +
                        "  assetBundleName: \n" +
                        "  assetBundleVariant: \n";
                    newLines.Add(dummyLine);
                    break;
                }

                if (continueWriting)
                {
                    newLines.Add(line);
                }
            }

            File.WriteAllLines(metaPath, newLines);
            AssetDatabase.Refresh();
        }
    }

    public void GenerateSpriteBones()
    {
        SetSpriteBones();
        WriteDownSpriteBones();
    }

    public void GetRootBone()
    {
        spriteSkin = GetComponent<SpriteSkin>();
        var sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log(assetPath);
        //AssetDatabase.ForceReserializeAssets(new List<string>() { assetPath });
        //AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log(spriteSkin.rootBone);
        //spriteSkin.OnBeforeSerialize();
    }

    private void SetSpriteBones()
    {
        //spriteSkin = GetComponent<SpriteSkin>();
        var sprite = GetComponent<SpriteRenderer>().sprite;

        //SerializedObject serializedObject = new SerializedObject(sprite);
        //serializedObject.Update();

        //string assetPath = AssetDatabase.GetAssetPath(sprite);

        //int boneNum = 36;
        float stepAngle = 360f / boneNum;

        SpriteBone[] newBones = new SpriteBone[boneNum];
        Matrix4x4[] newPoses = new Matrix4x4[boneNum];

        for (int i = 0; i < boneNum; i++)
        {
            string boneName = "bone_" + (i + 1).ToString();

            float angle = stepAngle * i;
            Vector3 eulerAngle = new Vector3(0f, 0f, angle + 180);

            Vector3 position = new Vector3(2 * Mathf.Cos(Mathf.Deg2Rad * angle), 2 * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            Quaternion rotation = Quaternion.Euler(eulerAngle);

            newBones[i] = new SpriteBone() { name = boneName, position = position, rotation = rotation, length = 0.5f, parentId = -1 };
            Matrix4x4 pose = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, 1f)).inverse;
            newPoses[i] = pose;
        }
        sprite.SetBones(newBones);

        NativeArray<Matrix4x4> newNativePoses = new NativeArray<Matrix4x4>(newPoses, Allocator.Temp);
        sprite.SetBindPoses(newNativePoses);

        //serializedObject.ApplyModifiedProperties();

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    private void WriteDownSpriteBones()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        string metaPath = assetPath + ".meta";

        float p2u = sprite.pixelsPerUnit;
        var spriteBones = sprite.GetBones();

        string newLine = "    bones:\n";

        for (int i = 0; i < spriteBones.Length; ++i)
        {
            Vector2 pivot = sprite.pivot;
            string formatted = string.Format("    - name: {0}\n" +
                                    "      position: {{ x: {1}, y: {2}, z: {3}}}\n" +
                                    "      rotation: {{ x: {4}, y: {5}, z: {6}, w: {7}}}\n" +
                                    "      length: {8}\n" +
                                    "      parentId: {9}\n", spriteBones[i].name, pivot.x + p2u * spriteBones[i].position.x, pivot.y + p2u * spriteBones[i].position.y, p2u * spriteBones[i].position.z,
                                                            spriteBones[i].rotation.x, spriteBones[i].rotation.y, spriteBones[i].rotation.z, spriteBones[i].rotation.w,
                                                            p2u * spriteBones[i].length, spriteBones[i].parentId);
            newLine += formatted;
        }


        var lines = File.ReadLines(metaPath);
        List<string> newLines = new List<string>();

        foreach (string line in lines)
        {

            if (line == "    bones: []")
            {
                newLines.Add(newLine);
            }
            else
            {
                newLines.Add(line);
            }
        }

        File.WriteAllLines(metaPath, newLines);

        AssetDatabase.Refresh();
    }

    public void RemoveGameObjectBones()
    {

        List<GameObject> Children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                Children.Add(child.gameObject);
            }
        }

        foreach (GameObject child in Children)
        {
            DestroyImmediate(child);
        }
    }

    public void AttachRigidBody2GameObjectBones()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                if (child.gameObject.GetComponent<Rigidbody2D>() == null)
                {
                    child.gameObject.AddComponent<Rigidbody2D>();
                }
                child.gameObject.GetComponent<Rigidbody2D>().mass = boneMass;
            }
        }
    }

    public void AttachCircleCollider2GameObjectBones()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                if (child.gameObject.GetComponent<CircleCollider2D>() == null)
                {
                    child.gameObject.AddComponent<CircleCollider2D>();
                }
                child.gameObject.GetComponent<CircleCollider2D>().radius = boneColliderRadius;
            }
        }
    }

    public void AttachSpringJoint2GameObjectBones()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                if (child.gameObject.GetComponent<SpringJoint2D>() == null)
                {
                    child.gameObject.AddComponent<SpringJoint2D>();
                    child.gameObject.GetComponent<SpringJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
                    child.gameObject.GetComponent<SpringJoint2D>().frequency = centerFreq;
                    child.gameObject.GetComponent<SpringJoint2D>().autoConfigureConnectedAnchor = true;
                }
            }
        }
    }

    public void AttachDistanceJoint2GameObjectBones()
    {

        List<GameObject> Children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                Children.Add(child.gameObject);
            }
        }

        for (int i = 0; i < Children.Count; i++)
        {
            GameObject child = Children[i];
            GameObject connectedChild = Children[(i + 1) % Children.Count];

            if (child.GetComponent<DistanceJoint2D>() == null)
            {
                child.AddComponent<DistanceJoint2D>();
                child.GetComponent<DistanceJoint2D>().connectedBody = connectedChild.GetComponent<Rigidbody2D>();
            }
        }

    }

    public void AttachWrappingSpringJoint2GameObjectBones()
    {

        List<GameObject> Children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                Children.Add(child.gameObject);
            }
        }

        for (int i = 0; i < Children.Count; i++)
        {
            GameObject child = Children[i];
            GameObject connectedChild = Children[(i + 2) % Children.Count];

            SpringJoint2D newJoint = child.AddComponent<SpringJoint2D>();
            newJoint.frequency = wrapFreq;
            newJoint.connectedBody = connectedChild.GetComponent<Rigidbody2D>();
            newJoint.autoConfigureConnectedAnchor = true;
        }

    }

    public void AttachJelly2GameObjectBones()
    {

        foreach (Transform child in transform)
        {
            if (child.name.Contains("bone"))
            {
                if (child.GetComponent<Jelly>() == null)
                {
                    child.gameObject.AddComponent<Jelly>();
                    child.gameObject.GetComponent<Jelly>().SetPlayer(this.gameObject);
                }
            }
        }
    }

    public void DebugSpriteVetrices()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        Debug.Log(sprite.vertices.Length);
        foreach (var vertice in sprite.vertices)
        {
            Debug.Log(vertice);
        }
    }

    public void DebugSpriteEdges()
    {

    }

    public void DebugSpriteTriangles()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        Debug.Log(sprite.triangles.Length);
        foreach (var triangle in sprite.triangles)
        {
            Debug.Log(triangle);
        }
    }
}