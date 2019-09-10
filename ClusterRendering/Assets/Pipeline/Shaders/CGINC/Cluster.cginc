#ifndef CLUSTER
#define CLUSTER

struct Point
{
	float3 vertex;
	float3 normal;
	float4 tangent;
	float2 uv0;
	int materialID;
};

#if SHADER_TARGET >= 45
StructuredBuffer<float> _ResultBuffer;
StructuredBuffer<Point> _VertexBuffer;
#endif

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

struct clusterdata
{
	float4 vertex : SV_POSITION;
	float3 wpos : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 tangent : TEXCOORD2;
	float2 uv0 : TEXCOORD3;
};

clusterdata vert_cluster(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
{
	Point p = GetPoint(vertexID, instanceID);

	clusterdata o;
	o.vertex = mul(UNITY_MATRIX_VP, float4(p.vertex, 1.0f));
	o.wpos = p.vertex;
	o.normal = p.normal;
	o.tangent = p.tangent;
	o.uv0 = p.uv0;
	return o;
}

#endif