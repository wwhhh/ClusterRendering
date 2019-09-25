#ifndef STANDARD_PBR
#define STANDARD_PBR

#include "UnityCG.cginc"

half4 StandardPBR(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
	float3 normal, float3 viewDir,
	UnityLight light) 
{
	float roughness =  (1 - smoothness);
	float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);
	half nv = abs(dot(normal, viewDir));
	half nl = saturate(dot(normal, light.dir));
	float nh = saturate(dot(normal, halfDir));
	half lv = saturate(dot(light.dir, viewDir));
	half lh = saturate(dot(light.dir, halfDir));

	// Diffuse:Disney Diffuse
	half diffuseTerm = DisneyDiffuse(nv, nl, lh, roughness) * nl;

	//float roughness = roughness * roughness;
	//roughness = max(roughness, 0.002);
	//// Specular:D(GGX)*G()*F()
	//half V = SmithJointGGXVisibilityTerm(nl, nv, roughness);
	//float D = GGXTerm(nh, roughness);
	//half specularTerm = V * D * UNITY_PI;
	
	half3 color = diffColor * (/*light.color **/ diffuseTerm)
		/*+ specularTerm * light.color * FresnelTerm(specColor, lh)*/;

	return half4(color, 1);

}

#endif