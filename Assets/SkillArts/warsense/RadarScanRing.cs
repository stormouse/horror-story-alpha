using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Effects/RadarScanRing")]
public class RadarScanRing : SceneViewFilter {

    public static Shader shader = null;

    [SerializeField]
    public Color ringColor;

    [SerializeField]
    public Transform sourceTransform;

    [SerializeField]
    public float radius;

    [SerializeField]
    public float thickness;


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
            if(mainCamera == null)
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
            //[debug] shader = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/SkillArts/warsense/RadarScanRing.shader", typeof(Shader)) as Shader;
            shader = Resources.Load<Shader>("SkillArts/warsense/RadarScanRing");
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

        EffectMaterial.SetColor("_ringColor", ringColor);
        EffectMaterial.SetFloat("radius", radius);
        EffectMaterial.SetFloat("thickness", thickness);
        EffectMaterial.SetVector("ringCenter", sourceTransform.position);
        EffectMaterial.SetVector("cameraPositionWS", MainCamera.transform.position);
        EffectMaterial.SetMatrix("cameraToWorldMatrix", MainCamera.cameraToWorldMatrix);
        EffectMaterial.SetMatrix("_FrustumCornersES", GetFrustumCorners(MainCamera));

        CustomGraphicsBlit(source, destination, EffectMaterial, 0);
    }



    private Matrix4x4 GetFrustumCorners(Camera cam)
    {
        float camFov = cam.fieldOfView;
        float camAspect = cam.aspect;

        Matrix4x4 frustumCorners = Matrix4x4.identity;

        float fovWHalf = camFov * 0.5f;

        float tan_fov = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 toRight = Vector3.right * tan_fov * camAspect;
        Vector3 toTop = Vector3.up * tan_fov;

        Vector3 topLeft = (-Vector3.forward - toRight + toTop);
        Vector3 topRight = (-Vector3.forward + toRight + toTop);
        Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
        Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

        frustumCorners.SetRow(0, topLeft);
        frustumCorners.SetRow(1, topRight);
        frustumCorners.SetRow(2, bottomRight);
        frustumCorners.SetRow(3, bottomLeft);

        return frustumCorners;
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
        GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

        GL.End();
        GL.PopMatrix();
    }

}