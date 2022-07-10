using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    #region Constants
    private const float splineOffset = 0.01f;
    #endregion

    #region Fields
    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public Transform[] points;
    #endregion

    #region MonoBehavior Callbacks
    private void Awake()
    {
        UpdateVerticies();
    }

    private void Update()
    {
        UpdateVerticies();
    }
    #endregion

    #region privateMethods
    private void UpdateVerticies()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 _vertex = points[i].localPosition;
            
            Vector2 _towardsCenter = (Vector2.zero - _vertex).normalized;

            float _colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            //float _colliderRadius = 2.0f;
            try 
            {
                spriteShape.spline.SetPosition(i, (_vertex - _towardsCenter * _colliderRadius));
            }
            catch
            {
                Debug.Log("Spline points are too close.");
                spriteShape.spline.SetPosition(i, _vertex - _towardsCenter * (_colliderRadius + splineOffset));
            }

            Vector2 _lt = spriteShape.spline.GetLeftTangent(i);

            //Vector2 _newRt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            //Vector2 _newLt = -1 * (_newRt);

            Vector2 _newLt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            Vector2 _newRt = -1 * (_newLt);

            spriteShape.spline.SetRightTangent(i, _newRt);
            spriteShape.spline.SetLeftTangent(i, _newLt);
        }
    }
    #endregion

}
