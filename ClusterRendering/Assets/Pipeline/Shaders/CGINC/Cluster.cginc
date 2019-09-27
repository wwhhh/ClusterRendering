#ifndef CLUSTER
#define CLUSTER

#include "Common.cginc"

#if SHADER_TARGET >= 45
StructuredBuffer<float> _ResultBuffer;
StructuredBuffer<Point> _VertexBuffer;
#endif

float4x4 _ShadowMatrixVP;

float GetCullResult(uint instanceID)
{
#if SHADER_TARGET >= 45
	float3 result = _ResultBuffer[instanceID];
#else
	float result = 1;
#endif
	return result;
}

float3 GetWorldPosition(uint instanceID, Point p)
{
#if SHADER_TARGET >= 45
	float3 worldPosition = GetCullResult(instanceID) * p.vertex;
#else
	float3 worldPosition = 0;
#endif
	return worldPosition;
}

Point GetPoint(uint vertexID, uint instanceID)
{
#if SHADER_TARGET >= 45
	Point p = _VertexBuffer[instanceID * 255 + vertexID];
	p.vertex = GetWorldPosition(instanceID, p);
#else
	Point p;
#endif

	return p;
}

Interpolators vert_cluster(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
{
	Point p = GetPoint(vertexID, instanceID);

	Interpolators o;
	o.pos = mul(UNITY_MATRIX_VP, float4(p.vertex, 1.0f));
	o.uv = float4(p.uv0, 0, 0);
	o.worldPos = p.vertex;
	o.normal = p.normal;

#if defined(BINORMAL_PER_FRAGMENT)
	o.tangent = p.tangent;
#else
	o.tangent = p.tangent;
	o.binormal = CreateBinormal(o.normal, o.tangent, p.tangent.w);
#endif

	o.uv.xy = TRANSFORM_TEX(p.uv0, _MainTex);
	o.materialID = p.materialID;
	o.shadowVertex = mul(_ShadowMatrixVP, p.vertex);

	return o;
}

#endif