using Eloi;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class MetaMono_ImportMeshFilter : MonoBehaviour
{
    public A_PathTypeAbsoluteFileMono m_whereToExport;
    public MeshFilter m_meshFilter;
    public UnityEvent m_onImported;

    public bool m_importOnEnable = false;

    private void OnEnable()
    {
        if (m_importOnEnable)
            ImportObj();
    }


    [ContextMenu("Import OBJ")]
    public void ImportObj()
    {
        if (m_whereToExport == null)
            return;
        if( m_meshFilter == null)
            return;
        string path = m_whereToExport.GetPath();
        BasicMeshExporter.ImportMeshAsOBJ(out Mesh mesh, path);
        mesh.name = Path.GetFileNameWithoutExtension( path);
        m_meshFilter.sharedMesh = mesh;
        m_onImported.Invoke();
        Debug.Log("Import mesh from " + path);
    }

}
