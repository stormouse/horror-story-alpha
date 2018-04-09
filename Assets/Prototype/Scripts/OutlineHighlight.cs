using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHighlight : MonoBehaviour {
    private static Material outlineMaterial = null;
    public static Material OutlineMaterial
    {
        get
        {
            if(outlineMaterial == null)
            {
                outlineMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/SkillArts/warsense/Outline.mat");
            }
            return outlineMaterial;
        }
    }

    private List<List<Material>> originalMaterials = new List<List<Material>>();
    private Renderer[] m_renderers;
    private Transform viewer;
    private bool activated;

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
        
        for(int i = 0; i < m_renderers.Length; i++)
        {
            originalMaterials.Add(new List<Material>());
            var mats = m_renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                originalMaterials[i].Add(mats[j]);
            }
        }

        activated = false;
    }

    public void Activate()
    {
        if (!activated)
        {
            for (int i = 0; i < m_renderers.Length; i++)
            {
                if (m_renderers[i].materials.Length > 0)
                {
                    m_renderers[i].material = OutlineMaterial;
                }
            }
            activated = true;
        }
    }

    public void Deactivate()
    {
        if (activated)
        {
            for (int i = 0; i < m_renderers.Length; i++)
            {
                if (m_renderers[i].materials.Length > 0)
                {
                    m_renderers[i].material = originalMaterials[i][0];
                }
            }
            activated = false;
        }
    }

}
