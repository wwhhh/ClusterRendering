#ifndef DEFERRED_LIBRARY_INCLUDED
#define DEFERRED_LIBRARY_INCLUDED

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "Common.cginc"

#if SHADER_TARGET >= 45
StructuredBuffer<MaterialProperties> _MaterialPropertiesBuffer;
#endif

struct VertexData {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv : TEXCOORD0;
};

float GetAlpha(Interpolators i) {
	float alpha = _Tint.a;
#if !defined(_SMOOTHNESS_ALBEDO)
	alpha *= tex2D(_MainTex, i.uv.xy).a;
#endif
	return alpha;
}

float3 GetAlbedo(Interpolators i) {
	MaterialProperties property = _MaterialPropertiesBuffer[i.materialID];
	float3 albedo = property.color;
	//float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Tint.rgb;
#if defined (_DETAIL_ALBEDO_MAP)
	float3 details = tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
	albedo = lerp(albedo, albedo * details, GetDetailMask(i));
#endif
	return albedo;
}

float GetOcclusion(Interpolators i) {
#if defined(_OCCLUSION_MAP)
	return lerp(1, tex2D(_OcclusionMap, i.uv.xy).g, _OcclusionStrength);
#else
	return 1;
#endif
}

float3 GetTangentSpaceNormal(Interpolators i) {
	float3 normal = float3(0, 0, 1);
#if defined(_NORMAL_MAP)
	normal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
#endif
#if defined(_DETAIL_NORMAL_MAP)
	float3 detailNormal =
		UnpackScaleNormal(
			tex2D(_DetailNormalMap, i.uv.zw), _DetailBumpScale
		);
	detailNormal = lerp(float3(0, 0, 1), detailNormal, GetDetailMask(i));
	normal = BlendNormals(normal, detailNormal);
#endif
	return normal;
}

float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign) {
	return cross(normal, tangent.xyz) *
		(binormalSign * unity_WorldTransformParams.w);
}

void InitializeFragmentNormal(inout Interpolators i) {
	float3 tangentSpaceNormal = GetTangentSpaceNormal(i);
#if defined(BINORMAL_PER_FRAGMENT)
	float3 binormal = CreateBinormal(i.normal, i.tangent.xyz, i.tangent.w);
#else
	float3 binormal = i.binormal;
#endif

	i.normal = normalize(
		tangentSpaceNormal.x * i.tangent +
		tangentSpaceNormal.y * binormal +
		tangentSpaceNormal.z * i.normal
	);
}

float GetMetallic(Interpolators i) {
#if defined(_METALLIC_MAP)
	return tex2D(_MetallicMap, i.uv.xy).r;
#else
	MaterialProperties property = _MaterialPropertiesBuffer[i.materialID];
	//return _Metallic;
	return property.metallic;
#endif
}

float GetSmoothness(Interpolators i) {
	float smoothness = 1;
#if defined(_SMOOTHNESS_ALBEDO)
	smoothness = tex2D(_MainTex, i.uv.xy).a;
#elif defined(_SMOOTHNESS_METALLIC) && defined(_METALLIC_MAP)
	smoothness = tex2D(_MetallicMap, i.uv.xy).a;
#endif
	//return smoothness * _Smoothness;
	MaterialProperties property = _MaterialPropertiesBuffer[i.materialID];
	return smoothness * property.smoothness;
}

float3 GetEmission(Interpolators i) {
#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
#if defined(_EMISSION_MAP)
	return tex2D(_EmissionMap, i.uv.xy) * _Emission;
#else
	return _Emission;
#endif
#else
	return 0;
#endif
}

UnityLight CreateLight(Interpolators i) {
	UnityLight light;

#if defined(DEFERRED_PASS)
	light.dir = float3(0, 1, 0);
	light.color = 0;
#else
#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
	light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
#else
	light.dir = _WorldSpaceLightPos0.xyz;
#endif

	UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);

	light.color = _LightColor0.rgb * attenuation;
#endif
	return light;
}

float3 BoxProjection(
	float3 direction, float3 position,
	float4 cubemapPosition, float3 boxMin, float3 boxMax
) {
#if UNITY_SPECCUBE_BOX_PROJECTION
	UNITY_BRANCH
		if (cubemapPosition.w > 0) {
			float3 factors =
				((direction > 0 ? boxMax : boxMin) - position) / direction;
			float scalar = min(min(factors.x, factors.y), factors.z);
			direction = direction * scalar + (position - cubemapPosition);
		}
#endif
	return direction;
}

UnityIndirect CreateIndirectLight(Interpolators i, float3 viewDir) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

#if defined(VERTEXLIGHT_ON)
	indirectLight.diffuse = i.vertexLightColor;
#endif

#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
	indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
	float3 reflectionDir = reflect(-viewDir, i.normal);
	Unity_GlossyEnvironmentData envData;
	envData.roughness = 1 - GetSmoothness(i);
	envData.reflUVW = BoxProjection(
		reflectionDir, i.worldPos,
		unity_SpecCube0_ProbePosition,
		unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
	);
	float3 probe0 = Unity_GlossyEnvironment(
		UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData
	);
	envData.reflUVW = BoxProjection(
		reflectionDir, i.worldPos,
		unity_SpecCube1_ProbePosition,
		unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax
	);
#if UNITY_SPECCUBE_BLENDING
	float interpolator = unity_SpecCube0_BoxMin.w;
	UNITY_BRANCH
		if (interpolator < 0.99999) {
			float3 probe1 = Unity_GlossyEnvironment(
				UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0),
				unity_SpecCube0_HDR, envData
			);
			indirectLight.specular = lerp(probe1, probe0, interpolator);
		}
		else {
			indirectLight.specular = probe0;
		}
#else
	indirectLight.specular = probe0;
#endif

	float occlusion = GetOcclusion(i);
	indirectLight.diffuse *= occlusion;
	indirectLight.specular *= occlusion;
#endif

	return indirectLight;
}

struct FragmentOutput {
#if defined(DEFERRED_PASS)
	float4 gBuffer0 : SV_Target0;
	float4 gBuffer1 : SV_Target1;
	float4 gBuffer2 : SV_Target2;
	float4 outEmission : SV_Target3;
#else
	float4 color : SV_Target;
#endif
};

FragmentOutput frag_deferredLighting(Interpolators i)
{
	float alpha = GetAlpha(i);
#if defined(_RENDERING_CUTOUT)
	clip(alpha - _AlphaCutoff);
#endif

	InitializeFragmentNormal(i);

	float3 specularTint;
	float oneMinusReflectivity;

	//漫反射与高光都与金属度挂钩，
	float3 albedo = DiffuseAndSpecularFromMetallic(
		GetAlbedo(i), GetMetallic(i), specularTint, oneMinusReflectivity
	);

#if defined(_RENDERING_TRANSPARENT)
	albedo *= alpha;
	alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
#endif

	FragmentOutput output;
#if defined(DEFERRED_PASS)
	output.gBuffer0.rgb = albedo;
	output.gBuffer0.a = GetOcclusion(i);
	output.gBuffer1.rgb = specularTint;
	output.gBuffer1.a = GetSmoothness(i);;
	output.gBuffer2 = float4(i.normal * 0.5 + 0.5, 1);
	output.outEmission.rgb = GetEmission(i);
	output.outEmission.a = 1;
#else
	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
	float4 color = UNITY_BRDF_PBS(
		albedo, specularTint,
		oneMinusReflectivity, GetSmoothness(i),
		i.normal, viewDir,
		CreateLight(i), CreateIndirectLight(i, viewDir)
	);
	color.rgb += GetEmission(i);
#if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
	color.a = alpha;
#endif
	output.color = color;
#endif
	return output;
}

#endif