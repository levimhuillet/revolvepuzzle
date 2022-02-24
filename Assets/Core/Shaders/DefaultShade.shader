// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DefaultShade" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {} // Regular object texture 
		_FrostTex("Frost (RGB)", 2D) = "white" {} // Regular object texture 
		_FlameTex("Flame (RGB)", 2D) = "white" {} // Regular object texture 
		_TowerRotation("Tower Rotation", float) = 0 // The rotation of the tower - will be set by script
		_RevolveDir("Revolve Dir", int) = 0 // Whether moving Clockwise (1) or Anticlockwise (2) around the tower - will be set by script
		_Radius("Radius", float) = 0 // radius of the sphere
		_NumCycles("Num Cycles", int) = 0 // times around
		_PassedPrelim("Passed Prelim", int) = 0 // 1 if in zone, 0 if not
	}
		CGINCLUDE
		#pragma vertex vert
		#include "UnityCG.cginc"
		// Access the shaderlab properties

		// Input to vertex shader
			struct vertexInput {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};
		// Input to fragment shader
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 position_in_world_space : TEXCOORD0;
			float4 tex : TEXCOORD1;
		};

		uniform sampler2D _MainTex;
		uniform sampler2D _FrostTex;
		uniform sampler2D _FlameTex;
		uniform float _TowerRotation;
		uniform int _RevolveDir;
		uniform float _Radius;
		uniform int _NumCycles;
		uniform int _PassedPrelim;
		const float pi = 3.141592653589793238462;

		float calcDistMarginClockwise(vertexOutput input, float4 startPoint, float adjustedRadius, float adjustedRotation) {
			float endX = adjustedRadius * -sin(radians(adjustedRotation));
			float endZ = adjustedRadius * -cos(radians(adjustedRotation));
			float4 maxEndpoint = float4(endX, input.position_in_world_space.y, endZ, 1.0);
			//float maxDist = sqrt((endX) * (endX)+(endZ + adjustedRadius) * (endZ + adjustedRadius));
			//float vertDist = sqrt((endX - input.position_in_world_space.x) * (endX - input.position_in_world_space.x) + (endZ - input.position_in_world_space.z) * (endZ - input.position_in_world_space.z));
			float maxDist = (endX) * (endX)+(endZ + adjustedRadius) * (endZ + adjustedRadius);
			float vertDist = (endX - input.position_in_world_space.x) * (endX - input.position_in_world_space.x) + (endZ - input.position_in_world_space.z) * (endZ - input.position_in_world_space.z);
			
			return maxDist - vertDist;
		}

		float calcDistMarginAnticlockwise(vertexOutput input, float4 startPoint, float adjustedRadius, float adjustedRotation) {
			float endX = adjustedRadius * sin(radians(adjustedRotation));
			float endZ = adjustedRadius * cos(radians(adjustedRotation));
			float4 maxEndpoint = float4(endX, input.position_in_world_space.y, endZ, 1.0);
			//float maxDist = sqrt((endX) * (endX) + (endZ + adjustedRadius) * (endZ + adjustedRadius));
			//float vertDist = sqrt((input.position_in_world_space.x - endX) * (input.position_in_world_space.x - endX) + (input.position_in_world_space.z - endZ) * (input.position_in_world_space.z - endZ));
			float maxDist = (endX) * (endX) + (endZ + adjustedRadius) * (endZ + adjustedRadius);
			float vertDist = (input.position_in_world_space.x - endX) * (input.position_in_world_space.x - endX) + (input.position_in_world_space.z - endZ) * (input.position_in_world_space.z - endZ);

			return maxDist - vertDist;
		}

		float4 tryFillQuadrantClockwise(vertexOutput input, int quadrant, float adjustedRadius, float adjustedRotation)
		{
			if (quadrant == 1)
			{
				float4 startPoint = float4(0.0, input.position_in_world_space.y, -adjustedRadius, 1.0);
				float distMargin = calcDistMarginClockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
					&& input.position_in_world_space.x < 0 && input.position_in_world_space.z < 0)
				{
					return tex2D(_FlameTex, float4(input.tex)); // Flame Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else if (quadrant == 2)
			{
				float4 startPoint = float4(-adjustedRadius, input.position_in_world_space.y, 0.0, 1.0);
				float distMargin = calcDistMarginClockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x < 0 && input.position_in_world_space.z > 0)
				{
					return tex2D(_FlameTex, float4(input.tex)); // Flame Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else if (quadrant == 3)
			{
				float4 startPoint = float4(0.0, input.position_in_world_space.y, adjustedRadius, 1.0);
				float distMargin = calcDistMarginClockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x > 0 && input.position_in_world_space.z > 0)
				{
					return tex2D(_FlameTex, float4(input.tex)); // Flame Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else {
				float4 startPoint = float4(adjustedRadius, input.position_in_world_space.y, 0.0, 1.0);
				float distMargin = calcDistMarginClockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x > 0 && input.position_in_world_space.z < 0)
				{
					return tex2D(_FlameTex, float4(input.tex)); // Flame Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
		}

		float4 tryFillQuadrantAnticlockwise(vertexOutput input, int quadrant, float adjustedRadius, float adjustedRotation)
		{
			if (quadrant == 1)
			{
				float4 startPoint = float4(-adjustedRadius, input.position_in_world_space.y, 0.0, 1.0);
				float distMargin = calcDistMarginAnticlockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x < 0 && input.position_in_world_space.z < 0)
				{
					return tex2D(_FrostTex, float4(input.tex)); // Frost Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else if (quadrant == 2)
			{
				float4 startPoint = float4(0.0, input.position_in_world_space.y, adjustedRadius, 1.0);
				float distMargin = calcDistMarginAnticlockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x < 0 && input.position_in_world_space.z > 0)
				{
					return tex2D(_FrostTex, float4(input.tex)); // Frost Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else if (quadrant == 3)
			{
				float4 startPoint = float4(adjustedRadius, input.position_in_world_space.y, 0.0, 1.0);
				float distMargin = calcDistMarginAnticlockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x > 0 && input.position_in_world_space.z > 0)
				{
					return tex2D(_FrostTex, float4(input.tex)); // Frost Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			else {
				float4 startPoint = float4(0.0, input.position_in_world_space.y, -adjustedRadius, 1.0);
				float distMargin = calcDistMarginAnticlockwise(input, startPoint, adjustedRadius, adjustedRotation);

				if (distMargin > 0
				&& input.position_in_world_space.x > 0 && input.position_in_world_space.z < 0)
				{
					return tex2D(_FrostTex, float4(input.tex)); // Frost Color
				}
				else
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
			}
		}

		ENDCG

		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
			Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			 			 
			 // VERTEX SHADER
			 vertexOutput vert(vertexInput input)
			 {
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
				output.tex = input.texcoord;
				return output;
			 }

			 // FRAGMENT SHADER
			float4 frag(vertexOutput input) : COLOR
			{
				if (_NumCycles > 0 || (_PassedPrelim == 1 && (_TowerRotation < 0.5f || _TowerRotation > 359.5f)))
				{
					// solidly in Frost
					if (_RevolveDir == 1)
					{
						return tex2D(_FlameTex, float4(input.tex)); // Flame Color
					}
					// solidly in Flame
					else
					{
						return tex2D(_FrostTex, float4(input.tex)); // Frost Color
					}
				}
				else {

					// Else calculate segments based on tower rotation
					
					float adjustedRadius = sqrt(_Radius * _Radius - input.position_in_world_space.y * input.position_in_world_space.y);
					float adjustedRotation = _TowerRotation / 2.0;

					// Q1
					float4 fillAttempt = float4(0.0, 0.0, 0.0, 0.0);
					if (_TowerRotation >= 0 && _TowerRotation < 90)
					{
						// Clockwise
						if (_RevolveDir == 1)
						{
							for (int i = 1; i <= 1; i = i + 1) {
								fillAttempt = tryFillQuadrantClockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						//Anticlockwise
						else if (_RevolveDir == 2) {
							for (int i = 4; i >= 1; i = i - 1) {
								fillAttempt = tryFillQuadrantAnticlockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						// Neither
						else {
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
					}
					// Q2
					else if (_TowerRotation >= 90 && _TowerRotation < 180)
					{
						// Clockwise
						if (_RevolveDir == 1)
						{
							for (int i = 1; i <= 2; i = i + 1) {
								fillAttempt = tryFillQuadrantClockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						//Anticlockwise
						else if (_RevolveDir == 2) {
							for (int i = 4; i >= 2; i = i - 1) {
								fillAttempt = tryFillQuadrantAnticlockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						// Neither
						else {
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
					}
					// Q3
					else if (_TowerRotation >= 180 && _TowerRotation < 270)
					{
						// Clockwise
						if (_RevolveDir == 1)
						{
							for (int i = 1; i <= 3; i = i + 1) {
								fillAttempt = tryFillQuadrantClockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						//Anticlockwise
						else if (_RevolveDir == 2) {
							for (int i = 4; i >= 3; i = i - 1) {
								fillAttempt = tryFillQuadrantAnticlockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						// Neither
						else {
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
					}
					// Q4
					else if (_TowerRotation >= 270 && _TowerRotation <= 360)
					{
						// Clockwise
						if (_RevolveDir == 1)
						{
							for (int i = 1; i <= 4; i = i + 1) {
								fillAttempt = tryFillQuadrantClockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						//Anticlockwise
						else if (_RevolveDir == 2) {
							for (int i = 4; i >= 4; i = i - 1) {
								fillAttempt = tryFillQuadrantAnticlockwise(input, i, adjustedRadius, adjustedRotation);
								if (!all(fillAttempt == float4(0.0, 0.0, 0.0, 0.0)))
								{
									return fillAttempt;
								}
							}
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
						// Neither
						else {
							return tex2D(_MainTex, float4(input.tex)); // Base Color
						}
					}
					// Edge cases
					else {
						return tex2D(_MainTex, float4(input.tex)); // Base Color
					}
				}

			}


			/*
			// Tiling
			float4 _MainTex_ST;
			float4 _FrostTex_ST;
			float4 _FlameTex_ST;

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


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _FrostTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _FlameTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				bool isFrost = false;
				bool isFlame = false;

				fixed4 col;

				if (isFrost)
				{
					col = tex2D(_FrostTex, i.uv);
				}
				else if (isFlame)
				{
					col = tex2D(_FlameTex, i.uv);
				}
				else {
					col = tex2D(_MainTex, i.uv);
				}
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			*/

	   ENDCG
	   }
	}
			//FallBack "Diffuse"
}