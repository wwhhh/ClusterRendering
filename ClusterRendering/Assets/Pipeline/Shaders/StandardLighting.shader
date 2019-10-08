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
		#include "CGINC/Shadow.cginc"

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
		TextureCube _CubeMap; SamplerState sampler_CubeMap;
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
				float3 ndcpos = float3(i.uv * 2 - 1, depth * 2 - 1);
				float4 worldPos = mul(_InvVP, float4(ndcpos, 1));
				worldPos.xyz  /= worldPos.w;

				float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

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

				float4 clipposShadow = mul(_ShadowMatrixVP, float4(worldPos.xyz, 1));
				float3 ndcposShadow = clipposShadow.xyz / clipposShadow.w;
				float3 uvposShadow = ndcpos * 0.5 + 0.5;

				float4 shadowmap = _Shadowmap.Sample(sampler_Shadowmap, uvposShadow.xy);
				float shadowCameraDepth = DecodeFloatRGBA(shadowmap);

				float shadow = step(shadowCameraDepth, ndcpos.z);
				float4 result = shadow * fixed4(0, 0, 0, 0) + color * (1- shadow);

				return color;// float4(ndcpos, 1);
			}

            ENDCG
        }
    }
}