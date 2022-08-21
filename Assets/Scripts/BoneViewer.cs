using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEditor;
using UnityEngine.U2D;
using System.IO;

public class BoneViewer : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteSkin spriteSkin;

    public Sprite GetCurrentSprite()
    {
        //Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        sprite = GetComponent<SpriteRenderer>().sprite;
        return sprite;
    }

    public SpriteRenderer GetCurrentSpriteRender()
    {
        //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        return spriteRenderer;
    }

    public SpriteSkin GetCurrentSpriteSkin()
    {
        //SpriteSkin spriteSkin = GetComponent<SpriteSkin>();
        spriteSkin = GetComponent<SpriteSkin>();
        return spriteSkin;
    }


    public void DebugSpritePath()
    {
        //Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        Debug.Log(assetPath);
    }

    public void DebugSpriteRenderPath()
    {
        //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        string assetPath = AssetDatabase.GetAssetPath(spriteRenderer);
        Debug.Log(assetPath);
        EditorUtility.SetDirty(spriteSkin);
    }

    public void DebugSpriteSkinPath()
    {
        //SpriteSkin spriteSkin = GetComponent<SpriteSkin>();
        spriteSkin = GetComponent<SpriteSkin>();
        string assetPath = AssetDatabase.GetAssetPath(spriteSkin);
        Debug.Log(assetPath);
    }

    public void DebugSpriteGetBones()
    {
        //Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        sprite = GetComponent<SpriteRenderer>().sprite;
        string assetPath = AssetDatabase.GetAssetPath(sprite);
        float p2u = sprite.pixelsPerUnit;
        //float p2u = 1.0f;

        var spriteBones = sprite.GetBones();

        string newLine = "    bones:\n";

        for (int i = 0; i < spriteBones.Length; ++i)
        {
            //Debug.Log(spriteBones[i].name);
            //Debug.Log(spriteBones[i].position);
            //Debug.Log(spriteBones[i].rotation);
            //Debug.Log(spriteBones[i].length);
            //Debug.Log(spriteBones[i].parentId);
            //string formatted = string.Format("    - name: {0}\n" +
            //                                 "      position: {{ x: {1}, y: {2}, z: {3}}}\n" +
            //                                 "      rotation: {{ x: {4}, y: {5}, z: {6}, w: {7}}}\n" +
            //                                 "      length: {8}\n" +
            //                                 "      parentId: {9}\n", spriteBones[i].name, spriteBones[i].position.x, spriteBones[i].position.y, spriteBones[i].position.z,
            //                                                          spriteBones[i].rotation.x, spriteBones[i].rotation.y, spriteBones[i].rotation.z, spriteBones[i].rotation.w,
            //                                                          spriteBones[i].length, spriteBones[i].parentId);
            //string formatted = string.Format("    - name: {0}\n" +
            //         "      position: {{ x: {1}, y: {2}, z: {3}}}\n" +
            //         "      rotation: {{ x: {4}, y: {5}, z: {6}, w: {7}}}\n" +
            //         "      length: {8}\n" +
            //         "      parentId: {9}\n", spriteBones[i].name, p2u * spriteBones[i].position.x, p2u * spriteBones[i].position.y, p2u * spriteBones[i].position.z,
            //                                  spriteBones[i].rotation.x, spriteBones[i].rotation.y, spriteBones[i].rotation.z, spriteBones[i].rotation.w,
            //                                  p2u * spriteBones[i].length, spriteBones[i].parentId);
            Vector2 pivot = sprite.pivot;
            string formatted = string.Format("    - name: {0}\n" +
                                 "      position: {{ x: {1}, y: {2}, z: {3}}}\n" +
                                 "      rotation: {{ x: {4}, y: {5}, z: {6}, w: {7}}}\n" +
                                 "      length: {8}\n" +
                                 "      parentId: {9}\n", spriteBones[i].name, pivot.x + p2u * spriteBones[i].position.x, pivot.y + p2u * spriteBones[i].position.y, p2u * spriteBones[i].position.z,
                                                          spriteBones[i].rotation.x, spriteBones[i].rotation.y, spriteBones[i].rotation.z, spriteBones[i].rotation.w,
                                                          p2u * spriteBones[i].length, spriteBones[i].parentId);
            newLine += formatted;
            //Debug.Log(formatted);
            //Debug.Log(assetPath);
            //string filePath = string.Format("Assets/Sprites/{0}.meta", i);
            //File.WriteAllText(filePath, formatted);
        }

        string metaPath = assetPath + ".meta";
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

        // Override Meta Data
        //string fakePath = "Assets/Sprites/fake.meta";
        //File.WriteAllLines(fakePath, newLines);
        File.WriteAllLines(metaPath, newLines);

        AssetDatabase.Refresh();

    }
}
