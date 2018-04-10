using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/CircleMask")]
public class CircleMask : SceneViewFilter
{

    public static Shader shader = null;

    public Texture2D mask;


    private Material effectMaterial;
    public Material EffectMaterial
    {
        get
        {
            if (!effectMaterial && shader)
            {
                effectMaterial = new Material(shader);
                effectMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return effectMaterial;
        }
    }

    private Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = GetComponent<Camera>();
            }
            return mainCamera;
        }
    }


    private void Awake()
    {
        if (shader == null)
        {
            shader = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/SkillArts/spritetree/CircleMask.shader", typeof(Shader)) as Shader;
        }
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!EffectMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        EffectMaterial.SetTexture("_Mask", mask);

        CustomGraphicsBlit(source, destination, EffectMaterial, 0);
    }

    static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
    {
        RenderTexture.active = dest;

        fxMaterial.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho(); // Note: z value of vertices don't make a difference because we are using ortho projection

        fxMaterial.SetPass(passNr);

        GL.Begin(GL.QUADS);

        // Here, GL.MultitexCoord2(0, x, y) assigns the value (x, y) to the TEXCOORD0 slot in the shader.
        // GL.Vertex3(x,y,z) queues up a vertex at position (x, y, z) to be drawn.  Note that we are storing
        // our own custom frustum information in the z coordinate.
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.1f); // BL

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.1f); // BR

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.1f); // TR

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.1f); // TL

        GL.End();
        GL.PopMatrix();
    }

}