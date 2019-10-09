#ifndef RANDOM
#define RANDOM

inline float2 hash22(float2 p)
{
	p = float2(dot(p, float2(127.1, 311.7)),
		dot(p, float2(269.5, 183.3)));

	return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

#endif