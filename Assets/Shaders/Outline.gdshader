shader_type canvas_item;

const float outline_width = 5.0;

float normaliseValue(float unnormalisedValue, float maxValue){
	return (unnormalisedValue - 0.0) / maxValue;
}

void vertex() 
{
	// Called for every vertex the material is visible on.
}

void fragment() 
{
	vec2 size = vec2(textureSize(TEXTURE, 0));
	ivec2 UVtoTexel = ivec2(UV * vec2(size));
	vec4 thisTexel = texelFetch(TEXTURE, ivec2(UVtoTexel), 0);

	if(thisTexel.a < 0.3)
	{
		vec2 maxDistanceVec = vec2(float(UVtoTexel.x) + outline_width, float(UVtoTexel.y) + outline_width);
		float maxDistance = sqrt(pow(float(UVtoTexel.x) - maxDistanceVec.x, 2) + pow(float(UVtoTexel.y) - maxDistanceVec.y, 2));
		float distanceToNearestEdge = maxDistance;
		for(float i = -outline_width; i <= outline_width; i += 0.5){
			for(float j = -outline_width; j <= outline_width; j += 0.5){
				int iRounded = int(round(i));
				int jRounded = int(round(j));
				ivec2 relativeOffset = UVtoTexel + ivec2(iRounded, jRounded);
				vec4 offsetTexel = texelFetch(TEXTURE, relativeOffset, 0);

				if(offsetTexel.a > 0.8)
				{
					float distanceToEdge = distance(vec2(relativeOffset), vec2(UVtoTexel));
					if(distanceToEdge < distanceToNearestEdge)
					{
						distanceToNearestEdge = distanceToEdge;
					}
				}
			}
		}
		float normalizedDistance = normaliseValue(distanceToNearestEdge, maxDistance);
		float alpha = 1.0 - normalizedDistance;
		vec3 colorWhite = vec3(1.0, 1.0, 1.0);
		COLOR = vec4(colorWhite, alpha);	
	}
}
	
