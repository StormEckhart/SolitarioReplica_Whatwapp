using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBehaviourManager<CameraManager>
{
    [Header("References")]

    [Tooltip("A reference to the main camera, which this script is attached to, to not have to use Camera.main")]
    [ReadOnly]
    [SerializeField]
    private Camera m_Camera;
    public new Camera camera => m_Camera;


    
    [Tooltip("The camera's original position, to reset it to")]
    [ReadOnly]
    [SerializeField]
    private Vector3 m_OriginalPosition;
    public Vector3 OriginalPosition => m_OriginalPosition;
    [Tooltip("The camera's original euler rotation, to reset it to")]
    [ReadOnly]
    [SerializeField]
    private Vector3 m_OriginalEulerRotation;
    public Vector3 OriginalEulerRotation => m_OriginalEulerRotation;

    [Tooltip("The camera's moving tween animation")]
    private Tween m_MovingCameraTween;
    [Tooltip("The camera's rotating tween animation")]
    private Tween m_RotatingCameraTween;



#if UNITY_EDITOR
    //To set all the possible references through code
    //(useful when you have multiple different objects with the same script attached (multiple type of enemies etc...))
    [Button]
    public void SetRefs()
    {
        m_Camera = GetComponent<Camera>();
    }
#endif


    #region Unity Events

    private void OnEnable()
    {
        GameManager.OnLevelStartEvent += ResetCamera;
    }
    private void OnDisable()
    {
        GameManager.OnLevelStartEvent -= ResetCamera;
    }

    #endregion


    #region Camera Modifyers

    [Button]
    public void SetOriginalValues()
    {
        m_OriginalPosition = m_Camera.transform.position;

        m_OriginalEulerRotation = m_Camera.transform.eulerAngles;
    }

    //A method to use to move the main camera
    public void MoveCamera(Vector3 i_EndPoint, float i_AnimationDuration, Ease i_EaseType)
    {
        m_MovingCameraTween?.Kill();

        m_MovingCameraTween = m_Camera.transform.DOMove(i_EndPoint, i_AnimationDuration)
                          .SetEase(i_EaseType);
    }
    //A method to use to move the main camera
    public void RotateCamera(Vector3 i_EndEulerRotation, float i_AnimationDuration, Ease i_EaseType)
    {
        m_RotatingCameraTween?.Kill();

        m_RotatingCameraTween = m_Camera.transform.DORotate(i_EndEulerRotation, i_AnimationDuration)
                          .SetEase(i_EaseType);
    }


    public void ResetCamera()
    {
        m_MovingCameraTween?.Kill();
        m_RotatingCameraTween?.Kill();

        m_Camera.transform.localPosition = m_OriginalPosition;
        m_Camera.transform.localEulerAngles = m_OriginalEulerRotation;
    }

    #endregion
}
