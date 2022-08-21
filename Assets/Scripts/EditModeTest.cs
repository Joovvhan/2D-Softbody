using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditModeTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Editor가 이 Awake를 호출합니다");
    }

    private void OnEnable()
    {
        Debug.Log("Editor가 이 OnEnable를 호출합니다");
    }

    private void Start()
    {
        Debug.Log("Editor가 이 Start를 호출합니다");
    }

    private void Update()
    {
        Debug.Log("Editor가 이 Update를 호출합니다");
    }

    private void OnDisable()
    {
        Debug.Log("Editor가 이 OnDisable을 호출합니다");
    }

    private void OnDestroy()
    {
        Debug.Log("Editor가 이 OnDestroy를 호출합니다");
    }
}