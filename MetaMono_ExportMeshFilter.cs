using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MetaMono_ExportMeshFilter : MonoBehaviour
{
    public string m_subDirectory = "eLabRC/Mesh";
    public string m_meshName="ExportMesh";
    public MeshFilter m_meshFilter;

    public bool m_allExportOnStart = false;
    public bool m_allExportOnDestroy = false;

    private void Start()
    {
        if (m_allExportOnStart)
            SaveAllWithoutDate();
    }

    private void OnDestroy()
    {
        if (m_allExportOnDestroy)
            SaveAllWithoutDate();
    }

    [ContextMenu("Save all")]
    public void SaveAllWithoutDate()
    {
        try
        {
            SaveStlInPersistentDataPath();
        }
        catch (Exception e) { Debug.Log("Fail to save:STL"); }
        try
        {
            SaveObjInPersistentDataPath();
        }
        catch (Exception e) { Debug.Log("Fail to save:OBJ"); }
        try
        {
            SaveObjInRootOrDownloadWithoutDate();
        }
        catch (Exception e) { Debug.Log("Fail to save:Root OBJ"); }
    }

    [ContextMenu("Save persistent STL")]
    public void SaveStlInPersistentDataPath()
    {
        Mesh mesh = m_meshFilter.sharedMesh;
        if (mesh == null)
            return;
        string path =  $"{Application.persistentDataPath}/{m_subDirectory}/{m_meshName}.stl";
        BasicMeshExporter.SaveMeshAsSTL(mesh, m_meshName , path);

    }
    [ContextMenu("Save persistent OBJ")]
    public void SaveObjInPersistentDataPath() { 
    
        Mesh mesh = m_meshFilter.sharedMesh;
        if (mesh == null)
            return;
        string path = $"{Application.persistentDataPath}/{m_subDirectory}/{m_meshName}.obj";
        BasicMeshExporter.SaveMeshAsOBJ(mesh, path);
    }

    [ContextMenu("Save in Download OBJ")]
    public void SaveObjInRootOrDownloadWithDate()
    {

        SaveObjInRootOrDownload(true);
    }
    [ContextMenu("Save in Download without date OBJ")]
    public void SaveObjInRootOrDownloadWithoutDate()
    {
        SaveObjInRootOrDownload(false);
    }


    public void SaveObjInRootOrDownload(bool withDate)
    {
        string dir = GetAndroidRootOrDownloadInSubDirectory();
        string date = DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss");
        string file = Path.Join(dir, m_meshName +(withDate?date:"")+ ".obj");

        Mesh mesh = m_meshFilter.sharedMesh;
        if (mesh == null)
            return;
        BasicMeshExporter.SaveMeshAsOBJ(mesh, file);

    }

    private string GetAndroidRootOrDownloadInSubDirectory()
    {
        BasicMeshExporter.GetSDCardRootOrDownloadPath(out string path);
        string dir = Path.Join(path, m_subDirectory);
        return dir;
    }
}
