using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using static Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using Unity.Mathematics;
using System.IO;

public unsafe class ClusterUtils
{
    public static void GetPoints(NativeList<Vector3> posList, MeshRenderer[] renderers)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Transform trs = renderers[i].transform;
            MeshFilter filter = renderers[i].GetComponent<MeshFilter>();
            Mesh mesh = filter.sharedMesh;
            if (mesh != null)
                GetPoints(posList, mesh, trs);
        }
    }

    private static void GetPoints(NativeList<Vector3> posList, Mesh mesh, Transform meshTrans)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        int i;
        for (i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = meshTrans.localToWorldMatrix.MultiplyPoint(vertices[i]);
        }

        for (i = 0; i < triangles.Length; i++)
        {
            posList.Add(vertices[triangles[i]]);
        }
    }

    public static void WriteBytes<T>(string path, NativeList<T> data) where T : unmanaged
    {
        int size = data.Length * sizeof(T);
        byte[] bytes = new byte[size];
        UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref bytes[0]), data.GetUnsafePtr<T>(), size);

        File.WriteAllBytes(path, bytes);
    }

    public static void WriteBytes<T>(string path, T[] data) where T : unmanaged
    {
        int size = data.Length * sizeof(T);
        byte[] bytes = new byte[size];
        UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref bytes[0]), AddressOf(ref data[0]), size);

        File.WriteAllBytes(path, bytes);
    }

    public static T[] ReadBytes<T>(string path) where T : unmanaged
    {
        byte[] bytes = File.ReadAllBytes(path);
        T[] datas = new T[(bytes.Length * sizeof(byte)) / sizeof(T)];
        UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref datas[0]), UnsafeUtility.AddressOf(ref bytes[0]), bytes.Length * sizeof(byte));
        return datas;
    }

    public static void CheckPath(string name)
    {
        if (!Directory.Exists(name))
            Directory.CreateDirectory(name);
    }

}
