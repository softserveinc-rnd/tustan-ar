Shader "Transparent/Mask" {

	Properties

	{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
	}

	SubShader{

		Stencil{
			ZFail decrWrap
		}

			Tags{ "Queue" = "Geometry-10" }
			Lighting Off
			ZTest LEqual
			ZWrite On
			ColorMask 0

		Pass
		{
			AlphaTest Less[_Cutoff]
			SetTexture[_MainTex]
		}
	}
}