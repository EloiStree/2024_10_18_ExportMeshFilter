using Eloi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class MetaMono_ExportMeshFilter : MonoBehaviour
{
    public A_PathTypeAbsoluteFileMono m_whereToExport;
    public MeshFilter m_meshFilter;
    public UnityEvent m_onExported;

    public bool m_allExportOnEnable = false;
    public bool m_allExportOnDisable = false;

    private void OnEnable()
    {
        if (m_allExportOnEnable)
            SaveObj();
    }

    private void OnDisable()
    {
        if (m_allExportOnDisable)
            SaveObj();
    }

    [ContextMenu("Save persistent OBJ")]
    public void SaveObj()
    {
        Mesh mesh = m_meshFilter.sharedMesh;
        if (mesh == null)
            return;
        if (m_whereToExport == null)
            return;
        string path = m_whereToExport.GetPath();
        BasicMeshExporter.SaveMeshAsOBJ(mesh, path);
        m_onExported.Invoke();
        Debug.Log("Saved mesh to " + path);
    }

}
