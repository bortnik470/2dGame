using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform chTransform; 

    void Update()
    {
        transform.position = new Vector3(chTransform.position.x, chTransform.position.y, -10);   
    }
}
