using System;
using System.IO;
using UnityEngine;

public class BasicMeshExporter
{
    public static void SaveMeshAsOBJ(Mesh mesh, string filePath)
    {
        CreateDirectory(filePath);
        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine("# Unity3D Mesh Exporter");

        // Write vertices
        foreach (Vector3 v in mesh.vertices)
        {
            Vector3 worldV = (v); // To keep mesh in world position
            writer.WriteLine("v " + worldV.x + " " + worldV.y + " " + worldV.z);
        }

        // Write normals
        foreach (Vector3 n in mesh.normals)
        {
            Vector3 worldN = (n); // Transform normals
            writer.WriteLine("vn " + worldN.x + " " + worldN.y + " " + worldN.z);
        }

        // Write UVs
        foreach (Vector2 uv in mesh.uv)
        {
            writer.WriteLine("vt " + uv.x + " " + uv.y);
        }

        // Write faces
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int idx0 = mesh.triangles[i] + 1;
            int idx1 = mesh.triangles[i + 1] + 1;
            int idx2 = mesh.triangles[i + 2] + 1;
            writer.WriteLine("f " + idx0 + "/" + idx0 + "/" + idx0 + " " + idx1 + "/" + idx1 + "/" + idx1 + " " + idx2 + "/" + idx2 + "/" + idx2);
        }

        writer.Close();
        Debug.Log("Mesh exported to " + filePath);
    }

    private static void CreateDirectory(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static void SaveMeshAsSTL(Mesh mesh, string name,  string filePath)
    {
        CreateDirectory(filePath);
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            string headerText = $"{name}|" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            byte[] headerBytes = new byte[80];
            System.Text.Encoding.ASCII.GetBytes(headerText).CopyTo(headerBytes, 0);
            writer.Write(headerBytes);
            // Number of triangles
            writer.Write(mesh.triangles.Length / 3);

            // Write each triangle
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                // Triangle vertices
                Vector3 v0 = mesh.vertices[mesh.triangles[i]];
                Vector3 v1 = mesh.vertices[mesh.triangles[i + 1]];
                Vector3 v2 = mesh.vertices[mesh.triangles[i + 2]];

                // Calculate face normal
                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Write the normal vector
                writer.Write(normal.x);
                writer.Write(normal.y);
                writer.Write(normal.z);

                // Write vertices
                WriteVector(writer, v0);
                WriteVector(writer, v1);
                WriteVector(writer, v2);

                // Attribute byte count (unused)
                writer.Write((ushort)0);
            }
        }

    }

        public static void GetSDCardRootOrDownloadPath(out string path)
        {
            string sdCardPath = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            // Access the Android environment class
            using (AndroidJavaClass environmentClass = new AndroidJavaClass("android.os.Environment"))
            {
                // Call the static method getExternalStorageDirectory to get the SD card path
                using (AndroidJavaObject externalStorageDirectory = environmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    sdCardPath = externalStorageDirectory.Call<string>("getAbsolutePath");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error retrieving SD card path: " + e.Message);
        }
#else
            // For non-Android platforms or in the Unity editor, set a default path (e.g., project directory)
            sdCardPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";
#endif

        path = sdCardPath;
        }
    

    private static void WriteVector(BinaryWriter writer, Vector3 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
        writer.Write(vector.z);
    }

}
