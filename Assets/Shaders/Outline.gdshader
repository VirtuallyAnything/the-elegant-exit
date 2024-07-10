shader_type canvas_item;

varying vec4 prev_color;
const float outlineWidth = 10.0;
const float blurRadius = 2.0;

float normaliseValue(float unnormalisedValue, float maxValue){
	return (unnormalisedValue - 0.0) / maxValue;
}

float vectorSum(vec3 vector){
	return vector.x + vector.y + vector.z;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 size = vec2(textureSize(TEXTURE, 0));
	ivec2 UVtoTexel = ivec2(UV * vec2(size));
	vec4 thisTexel = texelFetch(TEXTURE, ivec2(UVtoTexel), 0);

	if(thisTexel.a == 0.0){
		float finalAlpha = 0.0;
		for(float i = -outlineWidth; i <= outlineWidth; i += 0.5){
			for(float j = -outlineWidth; j <= outlineWidth; j += 0.5){
				int iRounded = int(round(i));
				int jRounded = int(round(j));
				ivec2 relativeOffset = UVtoTexel + ivec2(iRounded, jRounded);
				vec4 offsetTexel = texelFetch(TEXTURE, relativeOffset, 0);

				if(offsetTexel.a > 0.0){
					vec3 colorWhite = vec3(1.0, 1.0, 1.0);
					float deltaBlur = outlineWidth - blurRadius;
					vec2 relativeBlur = vec2(relativeOffset) + vec2(deltaBlur);
					float alpha = 1.0;
					float maxDistance = sqrt(pow(outlineWidth, 2) + pow(outlineWidth, 2));
					float distanceToEdge = sqrt(pow(float(relativeOffset.x - UVtoTexel.x), 2) + pow(float(relativeOffset.y - UVtoTexel.y), 2));
					float blurDistance = sqrt(pow(relativeBlur.x - float(relativeOffset.x), 2) + pow(relativeBlur.y - float(relativeOffset.y), 2));
					
					if(distanceToEdge > blurDistance){
						float normalizedDistance = normaliseValue(distanceToEdge, maxDistance);
						alpha /= normalizedDistance;
						if(alpha > finalAlpha){
							finalAlpha = alpha;
						}
					}
					COLOR = vec4(colorWhite, finalAlpha);
					break;
				}
			}
		}
			
	}
}
	


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
