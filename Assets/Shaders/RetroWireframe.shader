Shader "Custom/Retro Wireframe" 
{
    Properties 
	{
	    _corPrincipal ("Main Color: ", Color) = (1, 1, 1, 1)
		_corLinha ("Line Color: ", Color) = (1, 1, 1, 1)
		_larguraLinha ("Line Width: ", Range(0, 1)) = 0.1
		_tParcela ("Cell Size: ", Range(0, 100)) = 1
	}

  	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		float4 _corLinha;
		float4 _corPrincipal;
		float _tParcela;
		fixed _larguraLinha;

		struct Input {float2 uv_MainTex;	float3 worldPos;};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half val1 = step(_larguraLinha * 2, frac(IN.worldPos.x / _tParcela) + _larguraLinha);
			half val2 = step(_larguraLinha * 2, frac(IN.worldPos.z / _tParcela) + _larguraLinha);
			fixed val = 1 - (val1 * val2);
			o.Albedo = lerp(_corPrincipal, _corLinha, val);
			o.Alpha = 1;
	    }
		ENDCG
	} 
	FallBack "Diffuse"
}