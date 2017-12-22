Shader "Custom/Hole" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader{
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.

		Tags{ "Queue" = "Geometry+10" }

		// Don't draw in the RGBA channels; just the depth buffer

		ColorMask A
		ZWrite On

		// Do nothing specific in the pass:
		UsePass "Transparent/Diffuse/FORWARD"
		Pass{}
		
	}
}