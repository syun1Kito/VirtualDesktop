// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/TextureMapping" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		[Toggle] _FlipX("X Flip", Float) = 0
		[Toggle] _FlipY("Y Flip", Float) = 0
		_KeystoneCorrection("KeystoneCorrection", Range(-0.99,1.0)) = 0
		_ShadowTex ("Cookie", 2D) = "white" {}
		_FalloffTex ("FallOff", 2D) = "white" {}
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct appdata {
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				
			};

			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			float _FlipX;
			float _FlipY;
			float _KeystoneCorrection;

			sampler2D _ShadowTex;
			float4 _ShadowTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				appdata vtmp;
				vtmp = v;
				//float d = tex2Dlod(_ShadowTex, float4(v.uv.xy, 0, 0)).r;
				//v.vertex.y += d;
				
				//vtmp.uv.x = (v.uv.x - 0.5) * (1.0/(1+ _Trapezoid)) + 0.5;
				o.pos = UnityObjectToClipPos(vtmp.vertex);
				o.uvShadow = mul (unity_Projector, vtmp.vertex);

				//o.uvShadow.x = (o.uvShadow.y/1) * _Trapezoid * (o.uvShadow.x-0.5);
				//o.uvShadow.x = (o.uvShadow.x - 20)/ (1+_Trapezoid * (abs(o.uvShadow.z)/20)) +20;
				
				//o.uvShadow.x = clamp(o.uvShadow.xy,0.0,1.0);

				
				o.uvFalloff = mul (unity_ProjectorClip, vtmp.vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				
				

				return o;
			}
			
			fixed4 _Color;
			//sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 projCoord = UNITY_PROJ_COORD(i.uvShadow);
				projCoord /= projCoord.w;

				//
				projCoord.x = (projCoord.x - 0.5) * (1.0 / (1 + (_KeystoneCorrection *projCoord.y))) + 0.5;

				
				clip(clamp(projCoord.xy, 0.0, 1.0) - abs(projCoord.xy));	
				if (_FlipX > 0) projCoord.x = 1 - projCoord.x;
				if (_FlipY > 0) projCoord.y = 1 - projCoord.y;

				
				fixed4 texS = tex2D(_ShadowTex, projCoord.xy);
								
								
				texS *= _Color;
				


				fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = texS * fixed4(1.0, 1.0, 1.0, texF.a);
				UNITY_APPLY_FOG(i.fogCoord, res);
				return res;
			}
			ENDCG
		}
	}
}
