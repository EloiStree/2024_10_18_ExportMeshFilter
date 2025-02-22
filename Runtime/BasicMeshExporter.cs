using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class BasicMeshExporter
{
    public static void SaveMeshAsOBJ(Mesh mesh, string filePath)
    {
        CreateDirectory(filePath);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Unity3D Mesh Exporter");

        // Write vertices
        foreach (Vector3 v in mesh.vertices)
        {
            Vector3 worldV = (v); // To keep mesh in world position
            builder.AppendLine("v " + worldV.x + " " + worldV.y + " " + worldV.z);
        }

        // Write normals
        foreach (Vector3 n in mesh.normals)
        {
            Vector3 worldN = (n); // Transform normals
            builder.AppendLine("vn " + worldN.x + " " + worldN.y + " " + worldN.z);
        }

        // Write UVs
        foreach (Vector2 uv in mesh.uv)
        {
            builder.AppendLine("vt " + uv.x + " " + uv.y);
        }

        // Write faces
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int idx0 = mesh.triangles[i] + 1;
            int idx1 = mesh.triangles[i + 1] + 1;
            int idx2 = mesh.triangles[i + 2] + 1;
            builder.AppendLine("f " + idx0 + "/" + idx0 + "/" + idx0 + " " + idx1 + "/" + idx1 + "/" + idx1 + " " + idx2 + "/" + idx2 + "/" + idx2);
        }

        File.WriteAllText(filePath, builder.ToString().Replace(",","."));
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

    public static void ImportMeshAsOBJ(out Mesh mesh, string path)
    {
        mesh = new Mesh();
        mesh.Clear();

        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Read the OBJ file line by line
        foreach (var line in File.ReadAllLines(path))
        {
            string[] tokens = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) 
                continue;

            switch (tokens[0])
            {
                case "v": // Vertex
                    if (tokens.Length >= 4)
                    {
                        float x = float.Parse(tokens[1]);
                        float y = float.Parse(tokens[2]);
                        float z = float.Parse(tokens[3]);
                        vertices.Add(new Vector3(x, y, z));
                    }
                    break;

                case "vn": // Normal
                    if (tokens.Length >= 4)
                    {
                        float x = float.Parse(tokens[1]);
                        float y = float.Parse(tokens[2]);
                        float z = float.Parse(tokens[3]);
                        normals.Add(new Vector3(x, y, z));
                    }
                    break;

                case "vt": // Texture Coordinate
                    if (tokens.Length >= 3)
                    {
                        float u = float.Parse(tokens[1]);
                        float v = float.Parse(tokens[2]);
                        uvs.Add(new Vector2(u, v));
                    }
                    break;

                case "f": // Face
                    if (tokens.Length >= 4)
                    {
                        for (int i = 1; i < tokens.Length; i++)
                        {
                            string[] faceTokens = tokens[i].Split('/');
                            int vertexIndex = int.Parse(faceTokens[0]) - 1; // OBJ uses 1-based indexing
                            triangles.Add(vertexIndex);

                            if (faceTokens.Length > 1 && !string.IsNullOrEmpty(faceTokens[1])) // UV
                            {
                                // Handle UV if necessary
                            }

                            if (faceTokens.Length > 2 && !string.IsNullOrEmpty(faceTokens[2])) // Normal
                            {
                                // Handle normal if necessary
                            }
                        }
                    }
                    break;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }
}
