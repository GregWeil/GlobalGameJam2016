Shader "Custom/GodHandShadowShader" {
	Properties{
		_MainTex("Alpha (A)", 2D) = "white" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_CloudHeight("Cloud height", Float) = 0.0
	}

		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }

		Pass{
		Name "Caster"
		Tags{ "LightMode" = "ShadowCaster" }
		Offset 1, 1

		Fog{ Mode Off }
		ZWrite On ZTest LEqual Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

	struct v2f {
		V2F_SHADOW_CASTER;
		float2  uv : TEXCOORD1;
		float3 wpos : TEXCOORD2;
	};

	uniform float4 _MainTex_ST;

	v2f vert(appdata_base v) {
		v2f o;
		TRANSFER_SHADOW_CASTER(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.wpos = mul(_Object2World, v.vertex).xyz;
		return o;
	}

	uniform sampler2D _MainTex;
	uniform fixed _Cutoff;
	uniform fixed _CloudHeight;

	float4 frag(v2f i) : COLOR{
		fixed alpha = tex2D(_MainTex, i.uv).a;
	// or if you want to use grayscale texture:
	// fixed alpha = tex2D( _MainTex, i.uv ).r;
	clip(alpha - _Cutoff);
	clip(_CloudHeight - i.wpos.y);

	SHADOW_CASTER_FRAGMENT(i)
	}
		ENDCG
	}
	}
		Fallback Off
}