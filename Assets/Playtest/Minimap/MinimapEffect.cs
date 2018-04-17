using UnityEngine;

public class MinimapEffect : MonoBehaviour
{
    public float dotSize;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/MinimapImageShader"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (dotSize == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat("_DotSize", dotSize);
        Graphics.Blit(source, destination, material);
    }
}