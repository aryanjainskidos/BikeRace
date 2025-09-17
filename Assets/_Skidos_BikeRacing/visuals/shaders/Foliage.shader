// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_MotoTrial/Foliage" {
	Properties {
		_ColorTop ("Color Top", Color) = (0, 0, 0, 1)
		_ColorBottom ("Color Bottom", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Lighting Off
		
		Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            float4 _ColorTop;
            float4 _ColorBottom;
			
			struct vOut {
				float4 pos:SV_POSITION;
				float4 col:COLOR;
			};
			
			vOut vert(appdata_full v) {
			
                vOut o;
                o.pos = UnityObjectToClipPos (v.vertex);
                
                if(v.color.r == 1) {
                	o.col = _ColorTop;
                } else {
                	o.col = _ColorBottom;
                }
                
                return o;
            }

			fixed4 frag(vOut i) : COLOR0 {

                return i.col;
                
            }
			
			ENDCG
			
		}
	} 
	FallBack "Diffuse"
}