using UnityEngine;

public class Culling : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the camera component
        Camera camera = GetComponent<Camera>();

        // Create an array for layer culling distances
        float[] layerCullDistances = new float[32];

        // Set all layers to use the default far clip plane initially
        for (int i = 0; i < 32; i++)
        {
            layerCullDistances[i] = 0;
        }

        // Define the layer name and culling distance
        string layerName = "Grid";
        float cullingDistance = 25f;

        // Get the layer index
        int layerIndex = LayerMask.NameToLayer(layerName);
        Debug.Log("" + layerIndex);
        // Set the culling distance for the specific layer
        layerCullDistances[layerIndex] = cullingDistance;

        // Apply the layer culling distances
        camera.layerCullDistances = layerCullDistances;

        // Ensure the far clip plane is not limiting your culling distances
        if (cullingDistance > camera.farClipPlane)
        {
            camera.farClipPlane = cullingDistance;
        }
    }
}
