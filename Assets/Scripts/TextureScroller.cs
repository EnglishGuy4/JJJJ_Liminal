using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureScroller : MonoBehaviour
{
    [Header("Scroll Speed")]
    [Tooltip("Speed to scroll in X direction")]
    public float scrollSpeedX = 0f;

    [Tooltip("Speed to scroll in Y direction")]
    public float scrollSpeedY = 0.1f;

    [Header("Material Settings")]
    [Tooltip("If true, use a specific material index on the renderer")]
    public bool useMaterialIndex = false;

    [Tooltip("Index of the material to scroll (only used if useMaterialIndex is true)")]
    public int materialIndex = 0;

    [Tooltip("Name of the texture property to scroll (usually '_MainTex')")]
    public string textureProperty = "_MainTex";

    private Material _material;
    private Vector2 _currentOffset;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (useMaterialIndex && renderer.materials.Length > materialIndex)
        {
            _material = renderer.materials[materialIndex];
        }
        else
        {
            _material = renderer.material;
        }

        _currentOffset = _material.GetTextureOffset(textureProperty);
    }

    void Update()
    {
        // Calculate scrolling based on time and speeds
        _currentOffset.x += scrollSpeedX * Time.deltaTime;
        _currentOffset.y += scrollSpeedY * Time.deltaTime;

        _material.SetTextureOffset(textureProperty, _currentOffset);
    }
}
