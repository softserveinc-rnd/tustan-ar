Shader "Transparent/MaskDepthStensil" {

		SubShader{

		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.

		Tags{ "Queue" = "Geometry-10" }

			Lighting Off
			ZTest LEqual
			ZWrite On
			ColorMask 0
			
			Pass{ 
				Stencil{
				Ref 2
				Comp always
				Pass replace
			} 
		}
	}

}