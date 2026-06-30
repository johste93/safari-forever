// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Copyright (c) 2015, Felix Kate All rights reserved.
// Usage of this code is governed by a BSD-style license that can be found in the LICENSE file.

Shader "UI/ColorWheel" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Saturation ("Saturation", Range (0, 1)) = 1.0
        _Vibrance ("Vibrance", Range (0, 1)) = 1.0
	}

	SubShader {
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
	
		Pass{		
			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			//Prepare the inputs
			struct vertIN{
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};
			
			struct fragIN{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float _Saturation;
			float _Vibrance;
			
			//Function for making smooth circles from gradient
			fixed smoothCircle(fixed size, fixed gradient){
				fixed scaleFactor = size + 1;
				return smoothstep(0.5 - 0.0025 * scaleFactor, 0.5 + 0.0025 * scaleFactor, 1 - gradient * scaleFactor);
			}
			
			//Function for making box from gradient
			fixed smoothBox(fixed size, fixed2 gradient){
				fixed scaleFactor = size * 0.5;
				fixed alpha = ceil(gradient.x - scaleFactor);
				alpha *= ceil((1 - gradient.x) - scaleFactor);
				alpha *= ceil(gradient.y - scaleFactor);
				alpha *= ceil((1 - gradient.y) - scaleFactor);
				
				return alpha;
			}

			float Epsilon = 1e-10;
			
			float3 RGBtoHCV(in float3 RGB)
			{
				// Based on work by Sam Hocevar and Emil Persson
				float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
				float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				return float3(H, C, Q.x);
			}

			float3 HUEtoRGB(in float H)
			{
				float R = abs(H * 6 - 3) - 1;
				float G = 2 - abs(H * 6 - 2);
				float B = 2 - abs(H * 6 - 4);
				return saturate(float3(R,G,B));
			}

			float3 HSVtoRGB(in float3 HSV)
			{
				float3 RGB = HUEtoRGB(HSV.x);
				return ((RGB - 1) * HSV.y + 1) * HSV.z;
			}

			float3 RGBtoHSV(in float3 RGB)
			{
				float3 HCV = RGBtoHCV(RGB);
				float S = HCV.y / (HCV.z + Epsilon);
				return float3(HCV.x, S, HCV.z);
			}

			
			
			//Get the values from outside
			fixed4 _Color;
			
			//Fill the vert struct
			fragIN vert (vertIN v){
				fragIN o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord0;
				
				return o;
			}
			
			//Draw the circle
			fixed4 frag(fragIN i) : COLOR{
				fixed4 c = 1;
				
				//Make the inner area of the box
				/*
				fixed2 bGrad = 1;
				bGrad.x = smoothstep(0.25, 0.75, i.uv.x);
				bGrad.y = smoothstep(0.25, 0.75, i.uv.y);
				
				fixed4 cBox = lerp(1, _Color, bGrad.x) * bGrad.y;
				*/
				//Set up PI
				fixed PI = 3.14159265359;

				//Circular gradient
				fixed cGrad = distance(i.uv, fixed2(0.5, 0.5));
				
				//Angle gradient
				fixed aGrad = (atan2(1 - i.uv.x - 0.5, 1 - i.uv.y - 0.5) + PI) / (2 * PI);
				fixed ang = aGrad * PI * 2;

				//Calculate hue
				fixed4 cWheel = 1;
				
				//cWheel.r = clamp(2/PI * asin(cos(ang)) * 1.5 + 0.5, 0, 1);
				//cWheel.g = clamp(2/PI * asin(cos(2 * PI * (1.0/3.0) - ang)) * 1.5 + 0.5, 0, 1);
				//cWheel.b = clamp(2/PI * asin(cos(2 * PI * (2.0/3.0) - ang)) *  1.5 + 0.5, 0, 1);

				float3 hsv = RGBtoHSV(float3(clamp(2/PI * asin(cos(ang)) * 1.5 + 0.5, 0, 1),clamp(2/PI * asin(cos(2 * PI * (1.0/3.0) - ang)) * 1.5 + 0.5, 0, 1),clamp(2/PI * asin(cos(2 * PI * (2.0/3.0) - ang)) *  1.5 + 0.5, 0, 1)));
				float3 adjustedRGB = HSVtoRGB(float3(hsv.x, _Saturation ,_Vibrance));
				cWheel.r = adjustedRGB.r;
				cWheel.g = adjustedRGB.g;
				cWheel.b = adjustedRGB.b;

				//Calculate white part
				//fixed aWhite = smoothCircle(0.025, cGrad);
				
				//aWhite -= smoothCircle(0.36, cGrad);
				
				//aWhite += smoothBox(0.47, i.uv.xy);
				
				//c = lerp(0.5, 1, aWhite);
				
				//Add color
				fixed aCol = smoothCircle(-0.01, cGrad); //Outer outline
				
				//aCol -= smoothCircle(0.4, cGrad); //Inner outline
				
				c = lerp(c, cWheel, aCol);
				
				//aCol = smoothBox(0.5, i.uv.xy);
				
				//c = lerp(c, cBox, aCol);
				
				//Set alpha
				fixed alpha = smoothCircle(0, cGrad);
				
				alpha -= smoothCircle(0.11, cGrad); //Thickness
				
				//alpha += smoothBox(0.45, i.uv.xy);

				c.a = alpha;
				
				return c;
			}
			
			ENDCG
			
		}
	} 
}
