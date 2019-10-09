Shader "Deferred/StandardLighting"
{
    Properties
    {
    }
    SubShader
    {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
		#pragma target 5.0
		#include "UnityCG.cginc"
		#include "UnityDeferredLibrary.cginc"
		#include "UnityPBSLighting.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityGBuffer.cginc"
		#include "CGINC/StandardPBR.cginc"
		#include "CGINC/DirLightShadow.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;

		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		float4x4 _InvVP;
		float3 _CurLightDir;
		float3 _CurLightColor;
		Texture2D _GBuffer0; SamplerState sampler_GBuffer0;
		Texture2D _GBuffer1; SamplerState sampler_GBuffer1;
		Texture2D _GBuffer2; SamplerState sampler_GBuffer2;
		Texture2D _GBuffer3; SamplerState sampler_GBuffer3;
		Texture2D _DepthTexture; SamplerState sampler_DepthTexture;
		
		ENDCG

		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			float4 frag(v2f i) : SV_Target
			{
				float4 gbuffer0 = _GBuffer0.Sample(sampler_GBuffer0, i.uv);
				float4 gbuffer1 = _GBuffer1.Sample(sampler_GBuffer1, i.uv);
				float4 gbuffer2 = _GBuffer2.Sample(sampler_GBuffer2, i.uv);
				float4 gbuffer3 = _GBuffer3.Sample(sampler_GBuffer3, i.uv);

				float depth = _DepthTexture.Sample(sampler_DepthTexture, i.uv).x;
#ifdef SHADER_API_D3D11
				float4 ndcpos = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, depth, 1);
#else
				float4 ndcpos = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, depth * 2 - 1, 1);
#endif
				float4 worldPos = mul(_InvVP, ndcpos);
				//worldPos.xyz  /= worldPos.w;

				UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);
				float3 eyeVec = normalize(worldPos.xyz - _WorldSpaceCameraPos);
				float oneMinusReflectivity = 1 - SpecularStrength(data.specularColor.rgb);

				UnityLight light;
				light.dir = _CurLightDir;
				light.color = _CurLightColor;

				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				fixed4 color = StandardPBR(
					data.diffuseColor, data.specularColor, 
					oneMinusReflectivity, data.smoothness, 
					data.normalWorld, -eyeVec, 
					light
				);

				float atten = GetDirLightShadow(worldPos);
				return color* (1 - atten);
			}

            ENDCG
        }
    }
}