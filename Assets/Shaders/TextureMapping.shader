// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/TextureMapping" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		[Toggle] _FlipX("X Flip", Float) = 0
		[Toggle] _FlipY("Y Flip", Float) = 0
		_ShadowTex ("Cookie", 2D) = "gray" {}
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
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				
				

				return o;
			}
			
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 projCoord = UNITY_PROJ_COORD(i.uvShadow);
				projCoord /= projCoord.w;

				
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
