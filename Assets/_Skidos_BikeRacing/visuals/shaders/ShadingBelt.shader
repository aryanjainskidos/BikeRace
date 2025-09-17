// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_MotoTrial/ShadingBelt" {
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Lighting Off
		
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct vOut {
				float4 pos:SV_POSITION;
				float4 col:COLOR;
			};
			
			vOut vert(appdata_full v) {
                vOut o;
                
                o.pos = UnityObjectToClipPos (v.vertex);
                
                if(v.color.r == 1) {
                	o.col = float4(0.0, 0.0, 0.0, 0.5);
                } else {
                	o.col = float4(0.0, 0.0, 0.0, 0.0);
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