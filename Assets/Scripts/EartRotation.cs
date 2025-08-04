using UnityEngine;

public class EartRotation : MonoBehaviour
{
    public Rigidbody rb;
    public int forcee;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        float inp = Input.GetAxis("Horizontal") * forcee * Time.deltaTime;
        Debug.Log(inp);
        rb.AddTorque(transform.up * inp );
       
    }
}
