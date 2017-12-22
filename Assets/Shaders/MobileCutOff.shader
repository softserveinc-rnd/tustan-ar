Shader "Custom/CutoutMob" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert alpha noforwardadd
		sampler2D _MainTex;
	fixed4 _Color;
	float _Cutoff;
	struct Input {
		float2 uv_MainTex;
	};
	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		if (c.a > _Cutoff)
			o.Alpha = c.a;
		else
			o.Alpha = 0;
	}
	ENDCG
	}
		Fallback "Diffuse"
}