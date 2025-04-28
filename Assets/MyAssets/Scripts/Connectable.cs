using UnityEngine;

public class Connectable : MonoBehaviour
{
    public bool connectable;

    public Building building = null;

    public void SetConnectable(bool connectable, Building building)
    {
        this.connectable = connectable;
        if (connectable)
        {
            GetComponent<Renderer>().material.color = Color.blue;
            this.building = building;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (Vector3.Distance(transform.position, other.transform.position) < 0.5f && other.gameObject.layer == LayerMask.NameToLayer("Road"))
        {
            building.road = other.gameObject;
            other.gameObject.GetComponent<RoadManager>().IsIntersection = true;
        }
    }
}
