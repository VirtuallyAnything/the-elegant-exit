shader_type canvas_item;

varying vec4 prev_color;
uniform float outlineWidth = 10.0;
uniform float blurRadius = 5.0;

float normaliseValue(float unnormalisedValue, float maxValue){
	return (unnormalisedValue - 0.0) / maxValue;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 currentColor = texture(TEXTURE, UV);
	float totalOutlineWidth = outlineWidth + blurRadius;
	ivec2 size = textureSize(TEXTURE, 0);
	vec4 nextTexel = texelFetch(TEXTURE, ivec2(FRAGCOORD.xy), 0);
	if(nextTexel.a > 0.0){
		COLOR = vec4(1.0, 1.0, 1.0, 1.0);
	}
	vec4 surroundingTexels = textureGather(TEXTURE, UV, 3);
	if(currentColor.a == 0.0){
		for(int i = 0; i < 3; i++){
			if(surroundingTexels[i] > 0.0){
				//COLOR = vec4(1.0);
			}
		}
		
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
