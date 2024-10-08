using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{



    public static CameraSystem Instance { get; private set; } = null;

    public CinemachineVirtualCamera defaultCamera;
    public CinemachineVirtualCamera aimingCamera;
    public Camera minimapCamera;

    public Vector3 AimingTargetPoint { get; protected set; } = Vector3.zero;
    public LayerMask aimingLayers;
    private Camera mainCamera;
    private bool isRightCameraSide = false;
    private float cameraSideBlend = 0f;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimingLayers, QueryTriggerInteraction.Ignore))
        {
            AimingTargetPoint = hit.point;
        }
        else
        {
            AimingTargetPoint = ray.GetPoint(1000f);
        }


        cameraSideBlend = Mathf.Lerp(cameraSideBlend, isRightCameraSide ? 0f : 1f, Time.deltaTime * 10f);
        var defaultOfThirdPersonFollw = defaultCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        defaultOfThirdPersonFollw.CameraSide = cameraSideBlend;

        var aimingOfThirdPersonFollw = aimingCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        aimingOfThirdPersonFollw.CameraSide = cameraSideBlend;

    }
    public void setActiveAimingCamera(bool isAiming)
    {
        aimingCamera.gameObject.SetActive(isAiming);
    }

    public void SetChangeCameraSide()
    {
        isRightCameraSide = !isRightCameraSide;


    }
}
