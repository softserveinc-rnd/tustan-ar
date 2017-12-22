Shader "Transparent/Mask" {

	Properties

	{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
	}

	SubShader{

		Tags{ "Queue" = "Geometry-10" }

		ColorMask 0

		Pass
		{
			AlphaTest Less[_Cutoff]
			SetTexture[_MainTex]
		}
	}

}