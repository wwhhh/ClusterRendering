//High Quality Character Shadow Map  
// 32-bits depth shader
// Native depth shader 

Shader "Shadow/shadowDepth"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
		ZTest less
		Cull back

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"
			#include "CGINC/Cluster.cginc"

			float4x4 _ShadowMatrixVPRT;

			struct v2f {
				float4 pos : SV_POSITION;
			};

			v2f vert(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID) {
				Point p = GetPoint(vertexID, instanceID);

				v2f o;
				o.pos = mul(_ShadowMatrixVPRT, float4(p.vertex, 1));
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				fixed4 r = 0;
				r = EncodeFloatRGBA(i.pos.z);
				return r;
			}

			ENDCG
		}
	}
}