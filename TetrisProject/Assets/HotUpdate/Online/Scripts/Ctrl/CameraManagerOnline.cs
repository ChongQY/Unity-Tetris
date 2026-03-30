using DG.Tweening;
using UnityEngine;

public class CameraManagerOnline : MonoBehaviour
{
    private Camera mainCamera;
    void Awake() {
        mainCamera = Camera.main;
    }

    // ∑≈¥Û
    public void ZoomIn() {
        mainCamera.DOOrthoSize(13.8f, 0.5f);
    }
    // Àı–°
    public void ZoomOut() {
        mainCamera.DOOrthoSize(20.3f, 0.5f);
    }
}
