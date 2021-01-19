using UnityEngine;


public class UIDirectionControl : MonoBehaviour
{
    public bool useRelativeRotation = true;
    private Quaternion relativeRotation;

    private void Start()
    {
        relativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (useRelativeRotation)
        {
            //UIの回転を初期のまま保つ
            transform.rotation = relativeRotation;
        }
    }
}
