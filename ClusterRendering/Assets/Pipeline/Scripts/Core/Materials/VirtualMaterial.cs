using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public unsafe class VirtualMaterial
{

    #region EDITOR_UTILS
#if UNITY_EDITOR
    ComputeShader TextureProcess;

    public Dictionary<Material, int> GetMaterialsData(MeshRenderer[] allRenderers, string textureSavePath)
    {
        float3 ColorToVector(Color c)
        {
            return new float3(c.r, c.g, c.b);
        }

        var dict = new Dictionary<Material, int>();
        allNames = new List<string>();
        allProperties = new List<MaterialProperties>();
        var albedoTexs = new List<Texture>();
        var normalTexs = new List<Texture>();
        var albedoDict = new Dictionary<Texture, int>();
        var normalDict = new Dictionary<Texture, int>();
        int len = 0;

        int GetTextureIndex(List<Texture> lst, Dictionary<Texture, int> texDict, Texture tex)
        {
            int ind = -1;
            if (tex)
            {
                if (!texDict.TryGetValue(tex, out ind))
                {
                    ind = lst.Count;
                    lst.Add(tex);
                    texDict.Add(tex, ind);
                }
            }
            return ind;
        }

        foreach (var r in allRenderers)
        {
            var ms = r.sharedMaterials;
            foreach (var m in ms)
            {
                if (!dict.ContainsKey(m))
                {
                    allNames.Add(m.name);

                    dict.Add(m, len);
                    Texture albedo = m.GetTexture("_MainTex");
                    Texture normal = m.GetTexture("_BumpMap");
                    int albedoIdx = GetTextureIndex(albedoTexs, albedoDict, albedo);
                    int normalIdx = GetTextureIndex(normalTexs, normalDict, normal);
                    MaterialProperties property = new MaterialProperties
                    {
                        _Color = ColorToVector(m.GetColor("_Color")),
                        _SmoothnessIntensity = m.GetFloat("_Glossiness"),
                        _MetallicIntensity = m.GetFloat("_Metallic"),
                        _AlbedoTex = albedoIdx,
                        _NormalTex = normalIdx
                    };
                    allProperties.Add(property);

                    len++;
                }
            }
        }

        TextureProcess = Resources.Load<ComputeShader>("TextureProcessor");
        WriteToFile(out albedoGUIDs, albedoTexs, 0, textureSavePath);
        WriteToFile(out normalGUIDs, normalTexs, 1, textureSavePath);

        return dict;
    }

    private void WriteToFile(out string[] strs, List<Texture> list, int typeIndex, string path)
    {
        strs = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            strs[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(list[i]));
            ComputeBuffer floatBuffer = new ComputeBuffer(list[i].width * list[i].height, sizeof(float4));

            Texture tex = list[i];
            int pass = typeIndex == 0 ? 0 : 1;// 正常纹理和法线纹理
            TextureProcess.SetBuffer(pass, "_TextureDatas", floatBuffer);
            TextureProcess.SetTexture(pass, "_TargetTexture", tex);
            TextureProcess.SetInt("_Width", tex.width);
            TextureProcess.SetInt("_Height", tex.height);
            TextureProcess.SetInt("_Offset", 0);
            TextureProcess.Dispatch(pass, tex.width / 8, tex.height / 8, 1);

            float4[] resultDatas = new float4[floatBuffer.count];
            floatBuffer.GetData(resultDatas);

            string savePath = path + "/TexBytes/";
            ClusterUtils.CheckPath(savePath);
            savePath += strs[i] + ".bytes";

            switch (typeIndex)
            {
                case 0:
                    byte[] albedoColors = new byte[resultDatas.Length * sizeof(Color32)];
                    Color32* albedoColorsPtr = (Color32*)(UnsafeUtility.AddressOf(ref albedoColors[0]));
                    for (int j = 0; j < resultDatas.Length; ++j)
                    {
                        float4 v = resultDatas[j];
                        albedoColorsPtr[j] = new Color32((byte)(v.x * 255), (byte)(v.y * 255), (byte)(v.z * 255), (byte)(v.w * 255));
                    }
                    break;
                case 1:
                    byte[] normalColors = new byte[resultDatas.Length * sizeof(half2)];
                    half2* normalColorsPtr = (half2*)(UnsafeUtility.AddressOf(ref normalColors[0]));
                    for (int j = 0; j < resultDatas.Length; ++j)
                    {
                        float4 v = resultDatas[j];
                        normalColorsPtr[j] = (half2)v.xy;
                    }
                    break;
            }
            ClusterUtils.WriteBytes(savePath, resultDatas);
            floatBuffer.Dispose();
        }
    }

#endif
    #endregion

    #region RUNTIME
    public string[] albedoGUIDs;
    public string[] normalGUIDs;
    public List<MaterialProperties> allProperties;
    public List<string> allNames;
    #endregion

}