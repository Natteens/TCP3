using UnityEngine;

public class IsometricAiming : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings

    [SerializeField] private LayerMask groundMask;
    #endregion
    #region Private Fields

    private Camera mainCamera;

    #endregion

    #endregion


    #region Methods

    #region Unity Callbacks

    private void Start()
    {
        mainCamera = Camera.main;
    }

    #endregion

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }

    #endregion
}