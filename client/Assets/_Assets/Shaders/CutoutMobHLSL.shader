Shader "Custom/CutoutMobHLSL" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(-2,2)) = 0.5
		_Sharpness("Border sharpness", Range(1,200)) = 100
	}
		SubShader{
			Tags{ "Queue" = "Transparent" }
			Pass{
			ZWrite Off // don't write to depth buffer 
			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
			CGPROGRAM

#pragma vertex vert
#pragma fragment frag

			uniform float4 _Color;
			uniform float _Cutoff;
			uniform float _Sharpness;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.tex = input.texcoord;
				output.pos = UnityObjectToClipPos(input.vertex);
				return output;
			}

			fixed4 frag(vertexOutput input) : COLOR{
				return fixed4(_Color.rgb, ((length(input.tex.xy - float2(0.5, 0.5)) * 2) - _Cutoff) * _Sharpness);
			}

		ENDCG
		}
	}
}
