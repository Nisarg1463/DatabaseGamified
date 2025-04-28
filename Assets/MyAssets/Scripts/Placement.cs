using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public GameObject unplaceable;
    public bool canConnect = false;
    public void GenerateBorder(Building building)
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        Spawn(unplaceable, -transform.forward, building);
        Spawn(unplaceable, -transform.right, building);
        Spawn(unplaceable, transform.forward, building);
        Spawn(unplaceable, transform.right, building);
    }

    private GameObject Spawn(GameObject target, Vector3 direction, Building building)

    {
        Ray ray = new(transform.position, direction);
        Physics.Raycast(ray, out RaycastHit hit, 1);
        if (hit.collider.gameObject != null)
        {
            GameObject gameObject = Instantiate(target, hit.collider.transform.position, Quaternion.identity);
            if (canConnect)
            {
                gameObject.GetComponent<Connectable>().SetConnectable(true, building);
                canConnect = false;
            }
            return gameObject;
        }
        return null;
    }

}
