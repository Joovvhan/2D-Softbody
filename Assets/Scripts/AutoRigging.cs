using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D;
using UnityEditor;
using Unity.Collections;
using System.IO;
using System.Linq;

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

        //SpriteBone[] newBones = new SpriteBone[boneNum];
        //Matrix4x4[] newPoses = new Matrix4x4[boneNum];

        //for (int i = 0; i < boneNum; i++)
        //{
        //    string boneName = "bone_" + (i + 1).ToString();

        //    float angle = stepAngle * i;
        //    Vector3 eulerAngle = new Vector3(0f, 0f, angle + 180);

        //    Vector3 position = new Vector3(2 * Mathf.Cos(Mathf.Deg2Rad * angle), 2 * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        //    Debug.Log(position.ToString("F5"));
        //    Quaternion rotation = Quaternion.Euler(eulerAngle);

        //    newBones[i] = new SpriteBone() { name = boneName, position = position, rotation = rotation, length = 0.5f, parentId = -1 };
        //    Matrix4x4 pose = Matrix4x4.TRS(position, rotation, new Vector3(1f, 1f, 1f)).inverse;
        //    newPoses[i] = pose;
        //}

        float r = 0.15f;

        List<Vector2> validVertices = GetValidVertices();
        //List<Vector2> new_vertices = OffsetVertices(validVertices, r);
        List<Vector2> new_vertices;
        List<Vector2> normals;
        List<List<Vector2>> vertices_with_normals = OffsetVerticesWithNormals(validVertices, r);
        new_vertices = vertices_with_normals[0];
        normals = vertices_with_normals[1];
        List<Vector2> dense_vertices = IncreaseVertices(new_vertices, 5);
        List<Vector2> dense_normals = IncreaseVertices(normals, 5);
        //List<Vector2> sparse_vertices = SampleVertices(dense_vertices, null, 2.0f * r);
        List<Vector2> sparse_vertices;
        List<Vector2> sparse_normals;
        vertices_with_normals = SampleVerticesWithNormals(dense_vertices, dense_normals, null, 2.0f * r);
        sparse_vertices = vertices_with_normals[0];
        sparse_normals = vertices_with_normals[1];

        using StreamWriter outputFile = new StreamWriter("Edges.txt");
        foreach (var vert in validVertices)
            outputFile.WriteLine(vert.ToString("F5"));

        using StreamWriter outputFileNew = new StreamWriter("NewEdges.txt");
        foreach (var vert in sparse_vertices)
            outputFileNew.WriteLine(vert.ToString("F5"));

        boneNum = sparse_vertices.Count();
        stepAngle = 360f / boneNum;
        SpriteBone[] newBones = new SpriteBone[boneNum];
        Matrix4x4[] newPoses = new Matrix4x4[boneNum];

        for (int i = 0; i < sparse_vertices.Count(); i++)
        {
            string boneName = "bone_" + (i + 1).ToString();
            Vector2 vertex = sparse_vertices[i];
            Vector2 normal = sparse_normals[i];

            //float angle = Mathf.Atan2(vertex.y, vertex.x) * Mathf.Rad2Deg;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;

            Debug.Log(vertex);
            Debug.Log(normal);
            Debug.Log(angle);

            Vector3 eulerAngle = new Vector3(0f, 0f, angle);

            Vector3 position = new Vector3(vertex.x, vertex.y, 0f);
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
        Debug.Log(sprite.vertices);
        Debug.Log(sprite.vertices[0]);
        //foreach (var vertice in sprite.vertices)
        //{
        //    Debug.Log(vertice);
        //}
    }

    public List<Vector2> GetValidVertices()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;

        List<Vector2> vertices = sprite.vertices.ToList();
        var boundaryPath = EdgeHelpers.GetEdges(sprite.triangles).FindBoundary().SortEdges();
        var hashSet = new HashSet<int>();

        foreach (var edge in boundaryPath)
        {
            hashSet.Add(edge.v1);
            hashSet.Add(edge.v2);
        }

        List<int> list = hashSet.ToList();
        list.Sort();
        List<Vector2> validVertices = vertices.GetRange(list.First(), list.Last());

        return validVertices;
    }

    public void DebugSpriteEdges()
    {

        List<Vector2> validVertices = GetValidVertices();
        Debug.Log(validVertices);
        Debug.Log(validVertices[0]);
        Debug.Log(validVertices[0].ToString("F5"));
        Debug.Log(validVertices.Last().ToString("F5"));
        Debug.Log(validVertices.Count());

        using StreamWriter outputFile = new StreamWriter("Edges.txt");
        foreach (var vert in validVertices)
            outputFile.WriteLine(vert.ToString("F5"));

        Vector2 center_point = GetCenterPointFromVectices(validVertices);
        Debug.Log(center_point.ToString("F5"));

        float r = 0.15f;

        List<Vector2> new_vertices = OffsetVertices(validVertices, r);
        List<Vector2> dense_vertices = IncreaseVertices(new_vertices, 5);
        List<Vector2> sparse_vertices = SampleVertices(dense_vertices, null, 1.5f * r);

        using StreamWriter outputFileNew = new StreamWriter("NewEdges.txt");
        foreach (var vert in sparse_vertices)
            outputFileNew.WriteLine(vert.ToString("F5"));
    }

    public List<Vector2> SampleVertices(List<Vector2> vertices, int? target_vertex_num=null, float? target_distance=null)
    {
        int last_index = vertices.Count() - 1;

        float total_distance = GetlineLengthFromVertices(vertices);
        List<float> target_distances = new List<float>();

        if (target_vertex_num != null) {
            target_distances = Linspace(0f, total_distance, (int)target_vertex_num);
        }
        else if (target_distance != null)
        {
            target_vertex_num = Mathf.CeilToInt(total_distance / (float)target_distance);
            target_distances = Linspace(0, total_distance, (int)target_vertex_num);
        }

        List<float> cummulated_distance = new List<float>();
        float distance = 0;

        for (int i = 0; i < vertices.Count(); i++)
        {
            int j = (i + 1) % vertices.Count();
            float d = Vector2.Distance(vertices[i], vertices[j]);
            distance += d;
            cummulated_distance.Add(distance);
        }

        List<Vector2> selected_vertices = new List<Vector2>();
        int idx = 0;
        foreach (float target in target_distances)
        {
            idx = FindClosestIndex(cummulated_distance, target, idx);
            selected_vertices.Add(vertices[idx]);
        }

        return selected_vertices;
    }

    public List<List<Vector2>> SampleVerticesWithNormals(List<Vector2> vertices, List<Vector2> normals, int? target_vertex_num = null, float? target_distance = null)
    {
        int last_index = vertices.Count() - 1;

        float total_distance = GetlineLengthFromVertices(vertices);
        List<float> target_distances = new List<float>();

        if (target_vertex_num != null)
        {
            target_distances = Linspace(0f, total_distance, (int)target_vertex_num);
        }
        else if (target_distance != null)
        {
            target_vertex_num = Mathf.CeilToInt(total_distance / (float)target_distance);
            target_distances = Linspace(0, total_distance, (int)target_vertex_num);
        }

        List<float> cummulated_distance = new List<float>();
        float distance = 0;

        for (int i = 0; i < vertices.Count(); i++)
        {
            int j = (i + 1) % vertices.Count();
            float d = Vector2.Distance(vertices[i], vertices[j]);
            distance += d;
            cummulated_distance.Add(distance);
        }

        List<Vector2> selected_vertices = new List<Vector2>();
        List<Vector2> selected_normals = new List<Vector2>();
        int idx = 0;
        foreach (float target in target_distances)
        {
            idx = FindClosestIndex(cummulated_distance, target, idx);
            selected_vertices.Add(vertices[idx]);
            selected_normals.Add(normals[idx]);
        }

        //List<List<Vector2>> ret = new List<List<Vector2>>(2);
        //ret[0] = selected_vertices;
        //ret[1] = selected_normals;
        List<List<Vector2>> ret = new List<List<Vector2>>();
        ret.Add(selected_vertices);
        ret.Add(selected_normals);

        return ret;
    }

    public int FindClosestIndex(List<float> cummulated_distance, float target_distance, int starting_idx=0)
    {
        float last_d = Mathf.Infinity;
        int idx = 0;
        for (int i = starting_idx; i < cummulated_distance.Count(); i++)
        {
            float d = Mathf.Abs(cummulated_distance[i] - target_distance);

            if (d < last_d)
                idx = i;
            else
                break;

            last_d = d;

        }

        return idx;
    }

    public float GetlineLengthFromVertices(List<Vector2> vertices)
    {
        float total_distance = 0f;

        for (int i = 0; i < vertices.Count(); i++)
        {
            int j = (i + 1) % vertices.Count();
            float distance = Vector2.Distance(vertices[i], vertices[j]);
            total_distance += distance;
        }

        return total_distance;
    }

    //def SampleVertices(vertices, target_vertex_num= None, target_distance= None):


    //    assert not(target_vertex_num == None and target_distance == None), "Target Vertex Number and Target Distance Should Not be both None"

    //    last_index = len(vertices) - 1

    //    total_distance = GetlineLengthFromVertices(vertices)

    //    if target_vertex_num is not None:
    //        target_distances = np.linspace(0, total_distance, target_vertex_num, endpoint=False)
    //    elif target_distance is not None:
    //        target_vertex_num = int (np.ceil(total_distance / target_distance))
    //        target_distances = np.linspace(0, total_distance, target_vertex_num, endpoint=False)

    //    cummulated_distance = list()
    //    distance = 0
    //    for i, vertex in enumerate(vertices) :
    //        _, next_index = GetPreviousAndNextIndex(i, last_index)
    //        d = GetVertexDistance(vertex, vertices[next_index])
    //        distance += d
    //        cummulated_distance.append(distance)


    //    cummulated_distance = np.array(cummulated_distance)

    //    selected_vertices = [0]
    //    for target in target_distances[1:]:
    //        i = np.argmin(np.abs(cummulated_distance - target))
    //        selected_vertices.append(i+1)

    //    selected_vertices = np.array(selected_vertices)

    //    return vertices[selected_vertices]

    public void DebugSpriteTriangles()
    {
        var sprite = GetComponent<SpriteRenderer>().sprite;
        Debug.Log(sprite.triangles.Length);
        Debug.Log(sprite.triangles);
        //foreach (var triangle in sprite.triangles)
        //{
        //    Debug.Log(triangle);
        //}
    }

    public List<Vector2> IncreaseVertices(List<Vector2> vertices, int r=5)
    {
        List<Vector2> new_vertices = new List<Vector2>();

        for (int i = 0; i < vertices.Count(); i++)
        {
            int j = (i + 1) % vertices.Count();
            List<Vector2> new_points = Linspace(vertices[i], vertices[j]);
            new_vertices = new_vertices.Concat(new_points).ToList();
        }

        return new_vertices; 
    }

    public List<Vector2> Linspace(Vector2 a, Vector2 b, int r = 5)
    {
        List<Vector2> new_points = new List<Vector2>();

        float offset = 1.0f / r;

        for (int i = 0; i < r; i++)
        {
            Vector2 new_point = Vector2.Lerp(a, b, offset * i);
            new_points.Add(new_point);
        }

        return new_points;
    }

    public List<float> Linspace(float a, float b, int r = 5)
    {
        List<float> new_points = new List<float>();

        float offset = 1.0f / r;

        for (int i = 0; i < r; i++)
        {
            float dis = Mathf.Lerp(a, b, offset * i);
            new_points.Add(dis);
        }

        return new_points;
    }

    public Vector2 GetCenterPointFromVectices(List<Vector2> edges)
    {
        float max_x, min_x = max_x = edges[0].x;
        float max_y, min_y = max_y = edges[0].y;

        foreach (Vector2 edge in edges)
        {
            max_x = Mathf.Max(edge.x, max_x);
            min_x = Mathf.Min(edge.x, min_x);
            max_y = Mathf.Max(edge.y, max_y);
            min_y = Mathf.Min(edge.y, min_y);
        }

        return new Vector2((max_x + min_x) * 0.5f, (max_y + min_y) * 0.5f);
    }

    public List<Vector2> OffsetVertices(List<Vector2> vertices, float d)
    {
        List<Vector2> new_vertices = new List<Vector2>();

        Vector2 center_point = GetCenterPointFromVectices(vertices);

        int last_index = vertices.Count() - 1;

        Vector2 last_n = new Vector2(0, 0);

        for (int i = 0; i < vertices.Count(); i++)
        {
            Vector2 vertex = vertices[i];
            Vector2 r_vector = center_point - vertex;
            Vector2 r_vector_norm = r_vector.normalized;

            int previous_index, next_index;
            List<int> previous_and_next_indices = GetPreviousAndNextIndex(i, last_index);
            previous_index = previous_and_next_indices[0];
            next_index = previous_and_next_indices[1];

            Vector2 p1 = vertices[previous_index];
            Vector2 p2 = vertices[next_index];
            Vector2 p = p2 - p1;


            Vector2 n1 = new Vector2(p.y, -1 * p.x);
            Vector2 n2 = new Vector2(-1 * p.y, p.x);

            Vector2 n;

            if (last_n.Equals(new Vector2(0, 0))){

                if (Vector2.Dot(n1, r_vector_norm) > 0)
                    n = n1;
                else
                    n = n2;
            }

            else
            {
                if (Vector2.Dot(n1, last_n) > 0)
                    n = n1;
                else
                    n = n2;
            }

            n.Normalize();


            Vector2 offset_vector = d * n;
            last_n = n;

            Vector2 new_vertex = vertex + offset_vector;
            new_vertices.Add(new_vertex);

        }

        return new_vertices;
    }

    public List<List<Vector2>> OffsetVerticesWithNormals(List<Vector2> vertices, float d)
    {
        List<Vector2> new_vertices = new List<Vector2>();
        List<Vector2> normals = new List<Vector2>();

        Vector2 center_point = GetCenterPointFromVectices(vertices);

        int last_index = vertices.Count() - 1;

        Vector2 last_n = new Vector2(0, 0);

        for (int i = 0; i < vertices.Count(); i++)
        {
            Vector2 vertex = vertices[i];
            Vector2 r_vector = center_point - vertex;
            Vector2 r_vector_norm = r_vector.normalized;

            int previous_index, next_index;
            List<int> previous_and_next_indices = GetPreviousAndNextIndex(i, last_index);
            previous_index = previous_and_next_indices[0];
            next_index = previous_and_next_indices[1];

            Vector2 p1 = vertices[previous_index];
            Vector2 p2 = vertices[next_index];
            Vector2 p = p2 - p1;


            Vector2 n1 = new Vector2(p.y, -1 * p.x);
            Vector2 n2 = new Vector2(-1 * p.y, p.x);

            Vector2 n;

            if (last_n.Equals(new Vector2(0, 0)))
            {

                if (Vector2.Dot(n1, r_vector_norm) > 0)
                    n = n1;
                else
                    n = n2;
            }

            else
            {
                if (Vector2.Dot(n1, last_n) > 0)
                    n = n1;
                else
                    n = n2;
            }

            n.Normalize();


            Vector2 offset_vector = d * n;
            last_n = n;

            Vector2 new_vertex = vertex + offset_vector;
            new_vertices.Add(new_vertex);
            normals.Add(n);

        }

        List<List<Vector2>> ret = new List<List<Vector2>>();
        ret.Add(new_vertices);
        ret.Add(normals);
        //List<List<Vector2>> ret = new List<List<Vector2>>(2);
        //ret[0] = new_vertices;
        //ret[1] = normals;

        return ret;
    }

    private List<int> GetPreviousAndNextIndex(int i, int last_index)
    {
        int previous_index, next_index;

        if (i - 1 >= 0)
            previous_index = i - 1;
        else
            previous_index = last_index;

        if (i + 1 <= last_index)
            next_index = i + 1;
        else
            next_index = 0;

        return new List<int>(){ previous_index, next_index};
    }

    //def OffsetVertices(vertices, d, mode= 'normal'):


    //    new_vertices = list()


    //    center_point = GetCenterPointFromVectices(vertices)


    //    last_index = len(vertices) - 1

    //    last_n = None

    //    for i, vertex in enumerate(vertices) :


    //        r_vector = center_point - vertex
    //        r_vector_norm = r_vector / np.sqrt(np.sum(r_vector ** 2))

    //        if mode == 'center':
    //            offset_vector = d * r_vector_norm


    //        elif mode == 'normal':


    //            previous_index, next_index = GetPreviousAndNextIndex(i, last_index)


    //            p1, p2 = vertices[previous_index], vertices[next_index]
    //            p = p2 - p1


    //            n1 = np.array([p[1], -1 * p[0]])
    //            n2 = np.array([-1 * p[1], p[0]])

    //            # Based on Continous Normal Vector Assumption
    //            # 
    //            if last_n is None:
    //                n = n1 if np.sum(n1* r_vector_norm) > 0 else n2
    //            else:
    //                n = n1 if np.sum(n1* last_n) > 0 else n2

    //           n = n / np.sqrt(np.sum(n * *2))


    //            offset_vector = d* n
    //            last_n = n

    //        else:
    //            print(f"Invalid Offset Mode [{mode}]")


    //        new_vertex = vertex + offset_vector

    //        new_vertices.append(new_vertex)


    //    new_vertices = np.array(new_vertices)

    //    return new_vertices

}