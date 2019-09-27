Shader "Shadow/ShadowDepth"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"
			#include "CGINC/Cluster.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

            v2f vert (uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
            {
				Point p = GetPoint(vertexID, instanceID);
				float4 worldPos = float4(p.vertex, 1);
                
				v2f o;
				o.vertex = mul(_ShadowMatrixVP, worldPos);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				return 1;// i.vertex.z;
            }
            ENDCG
        }
    }
}
