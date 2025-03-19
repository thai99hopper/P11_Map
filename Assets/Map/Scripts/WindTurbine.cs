using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    [SerializeField] float speedRotation = 5;

    // Update is called once per frame
    void Update()
    {
        var angleCurrent = transform.eulerAngles;
        var z = angleCurrent.z + speedRotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(angleCurrent.x, angleCurrent.y, z);
    }
}
