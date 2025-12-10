using UnityEngine;

// 목표를 따라다니는 카메라
public class CameraFollow : MonoBehaviour
{
    public Transform Target;

    private void LateUpdate()
    {
<<<<<<< Updated upstream
        transform.position = Target.position;
=======
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isFPS = !_isFPS;

        }
        if (_isFPS)
        {
            transform.position = FPSTarget.position;
            //transform.rotation = FPSTarget.rotation;
        }
        else
        {
            transform.position = TPSTarget.position;
            //transform.rotation = TPSTarget.rotation;
        }
>>>>>>> Stashed changes
    }

}