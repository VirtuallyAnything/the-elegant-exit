shader_type canvas_item;
uniform vec2 player_position;
varying bool unveiled;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	if(FRAGCOORD.x > 10.0){
		unveiled = true;
	}
	if(unveiled){
		COLOR = vec4(0, 0, 0, 0.0);
	}else{
		COLOR = vec4(0, 0, 0, 1.0);
	}
	// Called for every pixel the material is visible on.
	
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
