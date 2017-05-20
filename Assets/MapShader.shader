// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/MapShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SelectedProvinceColor ("Selected Province Color", Color) = (1,1,1,1)
		_SelectedStateColor ("Selected State Color", Color) = (1,1,1,1)
		_SelectedRegionColor ("Selected Region Color", Color) = (1,1,1,1)
		_SelectedSupplyColor ("Selected Supply Color", Color) = (1,1,1,1)

		_LitUpProvinceColor ("Lit Province Color", Color) = (1,1,1,1)
		_LitUpStateColor ("Lit State Color", Color) = (1,1,1,1)
		_LitUpRegionColor ("Lit Region Color", Color) = (1,1,1,1)
		_LitUpSupplyColor ("Lit Supply Color", Color) = (1,1,1,1)
		_NoStateColor("No State Color", Color) = (1,1,1,1)
		_BorderColor ("BorderColor", Color) = (1,1,1,1)
		_StateOverlay ("StateOverlay", float) = 1
		_ShowOnlySupply("ShowOnlySupply", float) = 0
		_ShowOnlyRegions("ShowOnlyRegions", float) = 0
		_InternalStateBordersColor("InternalStateBordersColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed4 _SelectedProvinceColor;
			fixed4 _SelectedStateColor;
			fixed4 _SelectedRegionColor;
			fixed4 _SelectedSupplyColor;
			fixed4 _LitUpProvinceColor;
			fixed4 _LitUpStateColor;
			fixed4 _LitUpRegionColor;
			fixed4 _LitUpSupplyColor;
			fixed4 _BorderColor;
			fixed4 _NoStateColor;
			fixed4 _InternalStateBordersColor;
			float _StateOverlay;
			float _ShowOnlyRegions;
			float _ShowOnlySupply;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 baseCol = col;
				if(col.a > 0.5)
				{
					//it's an internal filling color, half of it is region color and half is state
					float2 eq = (baseCol.rg == _SelectedProvinceColor.rg);
					fixed res = eq.x * eq.y;
					col = lerp(col, _LitUpProvinceColor, res);
					eq = (baseCol.ba == _SelectedStateColor.ba);
					float resS = eq.x * eq.y;

					col = lerp(col, _LitUpStateColor, _StateOverlay * resS * (1 - res));

					
				}
				else
				{
					//it's a border color, half of it is a pointer to strategic region and half to supply region
					if(_ShowOnlyRegions == 1)
					{
						float2 eq = (baseCol.rg == _SelectedRegionColor.rg);
						fixed res = eq.x * eq.y;
						col = lerp(_BorderColor, _LitUpRegionColor, res);
					} else if (_ShowOnlySupply == 1)
					{
						float2 eq = (baseCol.ba == _SelectedSupplyColor.ba);
						fixed res = eq.x * eq.y;
						col = lerp(col, _LitUpSupplyColor, res);
					}
					else
					{
						float litValue = (_SinTime.a + 1)/2;
						float2 eq = (baseCol.rg == _SelectedRegionColor.rg);
						fixed res = eq.x * eq.y;
						col = lerp(_BorderColor, _LitUpRegionColor, litValue * res);
						eq = (baseCol.ba == _SelectedSupplyColor.ba);
						res = eq.x * eq.y;
						col = lerp(col, _LitUpSupplyColor, (1-litValue) * res);
					}
					
				}
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}
