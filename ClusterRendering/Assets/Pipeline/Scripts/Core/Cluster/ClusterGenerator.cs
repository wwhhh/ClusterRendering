using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ClusterComponents;
using static Unity.Mathematics.math;

public class ClusterGenerator : MonoBehaviour
{
    public ClusterResources res;

    [Range(100, 500)]
    public int voxelCount = 100;

    private NativeList<Vector3> vertexes;
    private NativeList<Cluster> clusters;
    private string sceneName;
    private string resPath;

    public struct CombinedModel
    {
        public NativeList<Point> allPoints;
        public Bounds bound;
        public NativeList<int> allMatIndex;
        public VirtualMaterial vm;
    }

    private void Start()
    {
    }

    [EasyButtons.Button]
    private void Do()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name.Replace("workscene_", "");

        LODGroup[] groups = GetComponentsInChildren<LODGroup>();
        Dictionary<MeshRenderer, bool> lowLevelDict = new Dictionary<MeshRenderer, bool>();
        foreach (var i in groups)
        {
            LOD[] lods = i.GetLODs();
            for (int j = 1; j < lods.Length; ++j)
            {
                foreach (var k in lods[j].renderers)
                {
                    if (k.GetType() == typeof(MeshRenderer))
                        lowLevelDict.Add(k as MeshRenderer, true);
                }
            }
        }

        resPath = PATH_FOLDER + sceneName + "/";

        CombinedModel model = ProcessCluster(GetComponentsInChildren<MeshRenderer>(false), lowLevelDict);
        GetCluster(model.allPoints, model.bound, out NativeList<Cluster> boxes, out NativeList<Point> points, voxelCount);

        ClusterUtils.CheckPath(PATH_FOLDER);
        ClusterUtils.CheckPath(resPath.ToString());
        ClusterUtils.WriteBytes(resPath + PATH_VERTEX + sceneName, points);
        ClusterUtils.WriteBytes(resPath + PATH_CLUSTER + sceneName, boxes);

        AddSceneStreaming(model.vm);

        SaveAsGameScene();
    }

    public CombinedModel ProcessCluster(MeshRenderer[] allRenderers, Dictionary<MeshRenderer, bool> lowLODLevels)
    {
        List<MeshFilter> allFilters = new List<MeshFilter>(allRenderers.Length);
        int sumVertexLength = 0;
        int sumTriangleLength = 0;

        for (int i = 0; i < allRenderers.Length; ++i)
        {
            if (!lowLODLevels.ContainsKey(allRenderers[i]))
            {
                MeshFilter filter = allRenderers[i].GetComponent<MeshFilter>();
                if (filter.sharedMesh == null)
                {
                    Debug.LogError("存在没有Mesh的物件："+allRenderers[i].transform.name);
                }
                allFilters.Add(filter);
                sumVertexLength += (int)(filter.sharedMesh.vertexCount * 1.2f);
            }
        }
        sumTriangleLength = (int)(sumVertexLength * 1.5);
        NativeList<Point> points = new NativeList<Point>(sumVertexLength, Allocator.Temp);
        NativeList<int> triangleMaterials = new NativeList<int>(sumVertexLength / 3, Allocator.Temp);

        VirtualMaterial vm = new VirtualMaterial();
        Dictionary<Material, int> Mat2Index = vm.GetMaterialsData(allRenderers, resPath);

        for (int i = 0; i < allFilters.Count; ++i)
        {
            Mesh mesh = allFilters[i].sharedMesh;
            GetPoints(points, mesh, allFilters[i].transform, allRenderers[i].sharedMaterials, Mat2Index);
        }
        float3 less = points[0].vertex;
        float3 more = points[0].vertex;

        for (int i = 1; i < points.Length; ++i)
        {
            float3 current = points[i].vertex;
            if (less.x > current.x) less.x = current.x;
            if (more.x < current.x) more.x = current.x;
            if (less.y > current.y) less.y = current.y;
            if (more.y < current.y) more.y = current.y;
            if (less.z > current.z) less.z = current.z;
            if (more.z < current.z) more.z = current.z;
        }

        float3 center = (less + more) / 2;
        float3 extent = more - center;
        Bounds b = new Bounds(center, extent * 2);

        CombinedModel md;
        md.bound = b;
        md.allPoints = points;
        md.allMatIndex = triangleMaterials;
        md.vm = vm;

        return md;
    }

    public void GetPoints(NativeList<Point> points, Mesh targetMesh, Transform transform, Material[] materials, Dictionary<Material, int> dict)
    {
        //# 长度相同时直接赋值，否则要补齐
        Vector3[] vertices = targetMesh.vertices;
        Vector3[] normals = targetMesh.normals;
        Vector2[] uvs = targetMesh.uv;
        Vector4[] tangents = targetMesh.tangents;

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = transform.localToWorldMatrix.MultiplyPoint(vertices[i]);
        }

        for (int idx = 0; idx < targetMesh.subMeshCount; idx++)
        {
            int[] triangles = targetMesh.GetTriangles(idx);
            Material mat = materials[idx];
            int matIndex = dict[mat];
            foreach (var i in triangles)
            {
                points.Add(new Point
                {
                    vertex = vertices[i],
                    normal = normals[i],
                    uv0 = uvs[i],
                    tangent = tangents[i],
                    materialID = matIndex
                });
            }
        }
    }

    class Triangle
    {
        public Point a;
        public Point b;
        public Point c;
        public Triangle last;
        public Triangle next;
    }

    class Voxel
    {
        public Triangle start;
        public int count;
        public void Add(Triangle ptr)
        {
            if (start != null)
            {
                start.last = ptr;
                ptr.next = start;
            }
            start = ptr;
            count++;
        }
        public Triangle Pop()
        {
            if (start.next != null)
            {
                start.next.last = null;
            }
            Triangle last = start;
            start = start.next;
            count--;
            return last;
        }
    }

    private static void GetCluster(NativeList<Point> pointsFromMesh, Bounds bd, out NativeList<Cluster> boxes, out NativeList<Point> points, int voxelCount)
    {
        List<Triangle> trs = GenerateTriangle(pointsFromMesh);
        Voxel[,,] voxels = GetVoxelData(trs, voxelCount, bd);
        GetClusterFromVoxel(voxels, out boxes, out points, pointsFromMesh.Length, voxelCount);
    }

    private static List<Triangle> GenerateTriangle(NativeList<Point> points)
    {
        List<Triangle> retValue = new List<Triangle>(points.Length / 3);
        for (int i = 0; i < points.Length; i += 3)
        {
            Triangle tri = new Triangle
            {
                a = points[i],
                b = points[i + 1],
                c = points[i + 2],
                last = null,
                next = null
            };
            retValue.Add(tri);
        }
        return retValue;
    }

    private static Voxel[,,] GetVoxelData(List<Triangle> trianglesFromMesh, int voxelCount, Bounds bound)
    {
        Voxel[,,] voxels = new Voxel[voxelCount, voxelCount, voxelCount];
        for (int x = 0; x < voxelCount; ++x)
            for (int y = 0; y < voxelCount; ++y)
                for (int z = 0; z < voxelCount; ++z)
                {
                    voxels[x, y, z] = new Voxel();
                }
        float3 downPoint = bound.center - bound.extents;
        for (int i = 0; i < trianglesFromMesh.Count; ++i)
        {
            Triangle tr = trianglesFromMesh[i];
            float3 position = (tr.a.vertex + tr.b.vertex + tr.c.vertex) / 3;
            float3 localPos = saturate((position - downPoint) / bound.size);
            int3 coord = (int3)(localPos * (voxelCount - 1));
            coord = min(coord, voxelCount);
            voxels[coord.x, coord.y, coord.z].Add(tr);
        }
        return voxels;
    }

    private static void GetClusterFromVoxel(Voxel[,,] voxels, out NativeList<Cluster> Clusteres, out NativeList<Point> points, int vertexCount, int voxelSize)
    {
        bool deGenerated = (vertexCount % CLUSTERCLIPCOUNT == 0);
        int clusterCount = Mathf.CeilToInt((float)vertexCount / CLUSTERCLIPCOUNT);
        points = new NativeList<Point>(clusterCount * CLUSTERCLIPCOUNT, Allocator.Temp);
        Clusteres = new NativeList<Cluster>(clusterCount, Allocator.Temp);

        int index = 0;
        for (int i = 0; i < voxelSize; i++)
            for (int j = 0; j < voxelSize; j++)
                for (int k = 0; k < voxelSize; k++)
                {
                    Voxel voxel = voxels[i, j, k];
                    if (voxel.count == 0) continue;

                    while (voxel.count > 0)
                    {
                        Triangle tri = voxel.Pop();
                        points.Add(tri.a);
                        points.Add(tri.b);
                        points.Add(tri.c);

                        index += 3;
                    }
                }

        if (!deGenerated)
        {
            for (int i = vertexCount; i < clusterCount * CLUSTERCLIPCOUNT; i++)
            {
                points.Add(new Point());
            }
        }

        float3 lessPoint;
        float3 morePoint;
        for (int i = 0; i < clusterCount; i++)
        {
            lessPoint = points[i * CLUSTERCLIPCOUNT].vertex;
            morePoint = points[i * CLUSTERCLIPCOUNT].vertex;
            for (int j = i * CLUSTERCLIPCOUNT; j < (i + 1) * CLUSTERCLIPCOUNT; j++)
            {
                lessPoint = lerp(lessPoint, points[j].vertex, (int3)(lessPoint > points[j].vertex));
                morePoint = lerp(morePoint, points[j].vertex, (int3)(morePoint < points[j].vertex));
            }

            Cluster cb = new Cluster
            {
                extent = (morePoint - lessPoint) / 2,
                position = (morePoint + lessPoint) / 2
            };
            Clusteres.Add(cb);
        }
    }

    private void AddSceneStreaming(VirtualMaterial vm)
    {
        foreach (var s in res.clusterProperties)
        {
            if (s.name == sceneName)
            {
                res.clusterProperties.Remove(s);
                break;
            }
        }

        SceneStreaming ss = new SceneStreaming();
        ss.name = sceneName;
        ss.vm = vm;

        res.clusterProperties.Add(ss);
    }

    public void SaveAsGameScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.name.StartsWith("workscene_", StringComparison.CurrentCultureIgnoreCase))
        {
            Debug.LogError("场景名不符合规范：workscene_XXXXX");
            return;
        }

        string dstScenePath = string.Concat(Path.GetDirectoryName(scene.path), "/", scene.name.Replace("workscene_", ""), ".unity");
        if (!EditorSceneManager.SaveScene(scene, dstScenePath))
        {
            Debug.LogError("Failed to save scene: " + dstScenePath);
            return;
        }

        // 删除Dynamic
        GameObject go = GameObject.Find("ClusterRenderer");
        if (go != null)
            UnityEngine.Object.DestroyImmediate(go);

        // 把场景加入Build setting
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        // 重新打开此场景，因为删除lightingDataAsset不能立即生效
        EditorSceneManager.OpenScene(scene.path);
    }

}