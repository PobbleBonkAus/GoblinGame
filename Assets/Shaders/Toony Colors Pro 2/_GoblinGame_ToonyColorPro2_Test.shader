// Toony Colors Pro+Mobile 2
// (c) 2014-2025 Jean Moreno

Shader "GoblinGame_ToonyColorPro2"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		[MainTexture] _BaseMap ("Albedo", 2D) = "white" {}
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		
		_RampThreshold ("Threshold", Range(0,1)) = 0.5
		[TCP2Separator]

		[TCP2HeaderHelp(Emission)]
		[TCP2ColorNoAlpha] [HDR] _Emission ("Emission Color", Color) = (0,0,0,1)
		[TCP2Separator]
		
		[TCP2HeaderHelp(Subsurface Scattering)]
		_SubsurfaceDistortion ("Distortion", Range(0,2)) = 0.2
		_SubsurfacePower ("Power", Range(0.1,16)) = 3
		_SubsurfaceScale ("Scale", Float) = 1
		[TCP2ColorNoAlpha] _SubsurfaceColor ("Color", Color) = (0.5,0.5,0.5,1)
		[TCP2Separator]
		
		[TCP2HeaderHelp(Sketch)]
		[Toggle(TCP2_SKETCH)] _UseSketch ("Enable Sketch Effect", Float) = 0
		_SketchTexture ("Sketch Texture", 2D) = "black" {}
		_SketchTexture_OffsetSpeed ("Sketch Texture UV Offset Speed", Float) = 120
		[TCP2Separator]
		
		[TCP2HeaderHelp(Wind)]
		[Toggle(TCP2_WIND)] _UseWind ("Enable Wind", Float) = 0
		_WindDirection ("Direction", Vector) = (1,0,0,0)
		_WindStrength ("Strength", Range(0,0.2)) = 0.025
		_WindSpeed ("Speed", Range(0,10)) = 2.5
		
		[TCP2HeaderHelp(Vertex Waves Animation)]
		_WavesSpeed ("Speed", Float) = 2
		_WavesHeight ("Height", Float) = 0.1
		_WavesFrequency ("Frequency", Range(0,10)) = 1
		
		[TCP2HeaderHelp(Depth Based Effects)]
		[TCP2ColorNoAlpha] _DepthColor ("Depth Color", Color) = (0,0,1,1)
		[PowerSlider(5.0)] _DepthColorDistance ("Depth Color Distance", Range(0.01,3)) = 0.5
		[PowerSlider(5.0)] _DepthAlphaDistance ("Depth Alpha Distance", Range(0.01,10)) = 0.5
		_DepthAlphaMin ("Depth Alpha Min", Range(0,1)) = 0.5
		_FoamSpread ("Foam Spread", Range(0,5)) = 2
		_FoamStrength ("Foam Strength", Range(0,1)) = 0.8
		_FoamColor ("Foam Color (RGB) Opacity (A)", Color) = (0.9,0.9,0.9,1)
		_FoamTex ("Foam Texture Custom", 2D) = "black" {}
		_FoamSmoothness ("Foam Smoothness", Range(0,0.5)) = 0.02
		
		[TCP2HeaderHelp(Dissolve)]
		[NoScaleOffset] _DissolveMap ("Map", 2D) = "gray" {}
		_DissolveValue ("Value", Range(0,1)) = 0.5
		[NoScaleOffset] _DissolveGradientTexture ("Gradient Texture", 2D) = "gray" {}
		_DissolveGradientWidth ("Ramp Width", Range(0,1)) = 0.2
		[TCP2Separator]
		
		[TCP2HeaderHelp(Vertical Fog)]
		_VerticalFogThreshold ("Y Threshold", Float) = 0
		_VerticalFogSmoothness ("Smoothness", Float) = 0.5
		_VerticalFogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
		[TCP2Separator]
		[TCP2HeaderHelp(Silhouette Pass)]
		_SilhouetteColor ("Silhouette Color", Color) = (0,0,0,0.33)
		[TCP2Separator]

		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff ("Receive Shadows", Float) = 1

		// Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"IgnoreProjectors"="True"
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#if UNITY_VERSION >= 202020
			#define URP_10_OR_NEWER
		#endif
		#if UNITY_VERSION >= 202120
			#define URP_12_OR_NEWER
		#endif
		#if UNITY_VERSION >= 202220
			#define URP_14_OR_NEWER
		#endif

		// Texture/Sampler abstraction
		#define TCP2_TEX2D_WITH_SAMPLER(tex)						TEXTURE2D(tex); SAMPLER(sampler##tex)
		#define TCP2_TEX2D_NO_SAMPLER(tex)							TEXTURE2D(tex)
		#define TCP2_TEX2D_SAMPLE(tex, samplertex, coord)			SAMPLE_TEXTURE2D(tex, sampler##samplertex, coord)
		#define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod)	SAMPLE_TEXTURE2D_LOD(tex, sampler##samplertex, coord, lod)

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		// Uniforms

		// Shader Properties
		TCP2_TEX2D_WITH_SAMPLER(_BaseMap);
		TCP2_TEX2D_WITH_SAMPLER(_DissolveMap);
		TCP2_TEX2D_WITH_SAMPLER(_DissolveGradientTexture);
		TCP2_TEX2D_WITH_SAMPLER(_FoamTex);
		TCP2_TEX2D_WITH_SAMPLER(_SketchTexture);
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

		CBUFFER_START(UnityPerMaterial)
			
			// Shader Properties
			float _WindSpeed;
			float4 _WindDirection;
			float _WindStrength;
			float _WavesFrequency;
			float _WavesHeight;
			float _WavesSpeed;
			fixed4 _SilhouetteColor;
			float4 _BaseMap_ST;
			float _DissolveValue;
			float _DissolveGradientWidth;
			fixed4 _BaseColor;
			fixed4 _DepthColor;
			float _DepthColorDistance;
			float _FoamSpread;
			float _FoamStrength;
			fixed4 _FoamColor;
			float4 _FoamTex_ST;
			float _FoamSmoothness;
			float _DepthAlphaDistance;
			float _DepthAlphaMin;
			half4 _Emission;
			float _RampThreshold;
			float4 _SketchTexture_ST;
			half _SketchTexture_OffsetSpeed;
			fixed4 _SColor;
			fixed4 _HColor;
			float _SubsurfaceDistortion;
			float _SubsurfacePower;
			float _SubsurfaceScale;
			fixed4 _SubsurfaceColor;
			float _VerticalFogThreshold;
			float _VerticalFogSmoothness;
			fixed4 _VerticalFogColor;
		CBUFFER_END

		// Hash without sin and uniform across platforms
		// Adapted from: https://www.shadertoy.com/view/4djSRW (c) 2014 - Dave Hoskins - CC BY-SA 4.0 License
		float2 hash22(float2 p)
		{
			float3 p3 = frac(p.xyx * float3(443.897, 441.423, 437.195));
			p3 += dot(p3, p3.yzx + 19.19);
			return frac((p3.xx+p3.yz)*p3.zy);
		}
		
		// Built-in renderer (CG) to SRP (HLSL) bindings
		#define UnityObjectToClipPos TransformObjectToHClip
		#define _WorldSpaceLightPos0 _MainLightPosition
		
		ENDHLSL

		// Silhouette Pass
		Pass
		{
			Name "Silhouette"
			Tags
			{
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Greater
			ZWrite Off

			Stencil
			{
				Ref 1
				Comp NotEqual
				Pass Replace
				ReadMask 1
				WriteMask 1
			}

			HLSLPROGRAM
			#pragma vertex vertex_silhouette
			#pragma fragment fragment_silhouette
			#pragma multi_compile_instancing
			#pragma target 3.0

			struct appdata_sil
			{
				float4 vertex : POSITION;
				fixed4 vertexColor : COLOR;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_sil
			{
				float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD0;
				float3 pack1 : TEXCOORD1; /* pack1.xyz = worldPos */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_sil vertex_silhouette (appdata_sil v)
			{
				v2f_sil output = (v2f_sil)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Shader Properties Sampling
				float __windTimeOffset = ( v.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( v.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				float3 worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;
				#if defined(TCP2_WIND)
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				#endif
				v.vertex.xyz = mul(UNITY_MATRIX_I_M, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = worldPos.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				v.vertex.y += (waveFactorX + waveFactorZ);
				float xn = -_waveHeight * cos(_vertexWavePos.x + _phase);
				float zn = -_waveHeight * cos(_vertexWavePos.z + _phase);
				v.normal = normalize(float3(xn, 1, zn));
				output.pack1.xyz = worldPos;
				output.vertex = TransformObjectToHClip(v.vertex.xyz);
				float4 clipPos = output.vertex;

				// Screen Position
				float4 screenPos = ComputeScreenPos(clipPos);
				output.screenPosition = screenPos;

				return output;
			}

			half4 fragment_silhouette (v2f_sil input) : SV_Target
			{

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Shader Properties Sampling
				float4 __silhouetteColor = ( _SilhouetteColor.rgba );

				return __silhouetteColor;
			}
			ENDHLSL
		}

		Pass
		{
			Name "Main"
			Tags
			{
				"LightMode"="UniversalForward"
			}
			Blend SrcAlpha OneMinusSrcAlpha

			// Stencil value used for Silhouette Pass to make sure we don't see a
			// silhouette when the same mesh occludes parts of itself
			Stencil
			{
				Ref 1
				Pass Replace
				ReadMask 1
				WriteMask 1
			}

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material keywords
			#pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ _FORWARD_PLUS

			// -------------------------------------

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			//--------------------------------------
			// Toony Colors Pro 2 keywords
			#pragma shader_feature_local_fragment TCP2_SKETCH
			#pragma shader_feature_local_vertex TCP2_WIND

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				half4 vertexColor   : COLOR;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float4 screenPosition : TEXCOORD3;
				float2 pack1 : TEXCOORD4; /* pack1.xy = texcoord0 */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if USE_FORWARD_PLUS
				// Fake InputData struct needed for Forward+ macro
				struct InputDataForwardPlusDummy
				{
					float3  positionWS;
					float2  normalizedScreenSpaceUV;
				};
			#endif

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Texture Coordinates
				output.pack1.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				// Shader Properties Sampling
				float __windTimeOffset = ( input.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( input.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				float3 worldPos = mul(UNITY_MATRIX_M, input.vertex).xyz;
				#if defined(TCP2_WIND)
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				#endif
				input.vertex.xyz = mul(UNITY_MATRIX_I_M, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = worldPos.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				input.vertex.y += (waveFactorX + waveFactorZ);
				float xn = -_waveHeight * cos(_vertexWavePos.x + _phase);
				float zn = -_waveHeight * cos(_vertexWavePos.z + _phase);
				input.normal = normalize(float3(xn, 1, zn));
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif
				float4 clipPos = vertexInput.positionCS;

				float4 screenPos = ComputeScreenPos(clipPos);
				output.screenPosition.xyzw = screenPos;

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// normal
				output.normal = normalize(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = normalize(input.normal);
				half3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);

				//Screen Space UV
				float2 screenUV = input.screenPosition.xyzw.xy / input.screenPosition.xyzw.w;
				
				// Shader Properties Sampling
				float4 __albedo = ( TCP2_TEX2D_SAMPLE(_BaseMap, _BaseMap, input.pack1.xy).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __dissolveMap = ( TCP2_TEX2D_SAMPLE(_DissolveMap, _DissolveMap, input.pack1.xy).r );
				float __dissolveValue = ( _DissolveValue );
				float __dissolveGradientWidth = ( _DissolveGradientWidth );
				float __dissolveGradientStrength = ( 2.0 );
				float3 __depthColor = ( _DepthColor.rgb );
				float __depthColorDistance = ( _DepthColorDistance );
				float __foamSpread = ( _FoamSpread );
				float __foamStrength = ( _FoamStrength );
				float4 __foamColor = ( _FoamColor.rgba );
				float3 __foamTextureCustom = ( TCP2_TEX2D_SAMPLE(_FoamTex, _FoamTex, input.pack1.xy * _FoamTex_ST.xy + _FoamTex_ST.zw).rgb );
				float __foamMask = ( .0 );
				float __foamSmoothness = ( _FoamSmoothness );
				float __depthAlphaDistance = ( _DepthAlphaDistance );
				float __depthAlphaMin = ( _DepthAlphaMin );
				float __ambientIntensity = ( 1.0 );
				float3 __emission = ( _Emission.rgb );
				float __rampThreshold = ( _RampThreshold );
				float3 __sketchColor = ( float3(0,0,0) );
				float3 __sketchTexture = ( TCP2_TEX2D_SAMPLE(_SketchTexture, _SketchTexture, screenUV * _ScreenParams.zw * _SketchTexture_ST.xy + _SketchTexture_ST.zw + hash22(floor(_Time.xx * _SketchTexture_OffsetSpeed.xx) / _SketchTexture_OffsetSpeed.xx)).aaa );
				float __sketchThresholdScale = ( 1.0 );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );
				float __subsurfaceDistortion = ( _SubsurfaceDistortion );
				float __subsurfacePower = ( _SubsurfacePower );
				float __subsurfaceScale = ( _SubsurfaceScale );
				float3 __subsurfaceColor = ( _SubsurfaceColor.rgb );
				float __verticalFogThreshold = ( _VerticalFogThreshold );
				float __verticalFogSmoothness = ( _VerticalFogSmoothness );
				float4 __verticalFogColor = ( _VerticalFogColor.rgba );

				// Sample depth texture and calculate difference with local depth
				float sceneDepth = SampleSceneDepth(input.screenPosition.xyzw.xy / input.screenPosition.xyzw.w);
				if (unity_OrthoParams.w > 0.0)
				{
					// Orthographic camera
					#if UNITY_REVERSED_Z
						sceneDepth = ((_ProjectionParams.z - _ProjectionParams.y) * (1.0 - sceneDepth) + _ProjectionParams.y);
					#else
						sceneDepth = ((_ProjectionParams.z - _ProjectionParams.y) * (sceneDepth) + _ProjectionParams.y);
					#endif
				}
				else
				{
					// Perspective camera
					sceneDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
				}
				
				float localDepth = LinearEyeDepth(positionWS.xyz, GetWorldToViewMatrix());
				float depthDiff = abs(sceneDepth - localDepth);

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;

				half3 emission = half3(0,0,0);
				
				//Dissolve
				half dissolveMap = __dissolveMap;
				half dissolveValue = __dissolveValue;
				half gradientWidth = __dissolveGradientWidth;
				float dissValue = dissolveValue*(1+2*gradientWidth) - gradientWidth;
				float dissolveUV = smoothstep(dissolveMap - gradientWidth, dissolveMap + gradientWidth, dissValue);
				clip((1-dissolveUV) - 0.001);
				half4 dissolveColor = ( TCP2_TEX2D_SAMPLE(_DissolveGradientTexture, _DissolveGradientTexture, dissolveUV.xx).rgba );
				dissolveColor *= __dissolveGradientStrength * dissolveUV;
				emission += dissolveColor.rgb;
				
				albedo *= __mainColor.rgb;
				
				// Depth-based color
				half3 depthColor = __depthColor;
				half3 depthColorDist = __depthColorDistance;
				albedo.rgb = lerp(depthColor, albedo.rgb, saturate(depthColorDist * depthDiff));
				
				// Depth-based water foam
				half foamSpread = __foamSpread;
				half foamStrength = __foamStrength;
				half4 foamColor = __foamColor;
				
				half3 foam = __foamTextureCustom;
				float foamDepth = saturate(foamSpread * depthDiff) * (1.0 - __foamMask);
				half foamSmooth = __foamSmoothness;
				half foamTerm = (smoothstep(foam.r - foamSmooth, foam.r + foamSmooth, saturate(foamStrength - foamDepth)) * saturate(1 - foamDepth)) * foamColor.a;
				albedo.rgb = lerp(albedo.rgb, foamColor.rgb, foamTerm);
				alpha = lerp(alpha, foamColor.a, foamTerm);
				
				// Depth-based alpha
				alpha *= saturate((__depthAlphaDistance * depthDiff) + __depthAlphaMin);

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord = input.shadowCoord;
			#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
			#else
				float4 shadowCoord = float4(0, 0, 0, 0);
			#endif

			#if defined(URP_10_OR_NEWER)
				#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
					half4 shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
				#elif !defined (LIGHTMAP_ON)
					half4 shadowMask = unity_ProbesOcclusion;
				#else
					half4 shadowMask = half4(1, 1, 1, 1);
				#endif

				Light mainLight = GetMainLight(shadowCoord, positionWS, shadowMask);
			#else
				Light mainLight = GetMainLight(shadowCoord);
			#endif

			#if defined(_SCREEN_SPACE_OCCLUSION) || defined(USE_FORWARD_PLUS)
				float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
			#endif

				// ambient or lightmap
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
				half occlusion = 1;

				half3 indirectDiffuse = bakedGI;
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;
				emission += __emission;

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;

				half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

				half ndl = dot(normalWS, lightDir);
				half3 ramp;
				
				half rampThreshold = __rampThreshold;
				ndl = saturate(ndl);
				ramp = step(rampThreshold, ndl);

				// apply attenuation
				ramp *= atten;

				// Sketch
				#if defined(TCP2_SKETCH)
				half3 sketchColor = lerp(__sketchColor, half3(1,1,1), __sketchTexture);
				half3 sketch = lerp(sketchColor, half3(1,1,1), saturate(ramp * __sketchThresholdScale));
				#endif
				#if defined(TCP2_SKETCH)
				indirectDiffuse.rgb *= sketch.rgb;
				#endif

				// highlight/shadow colors
				ramp = lerp(__shadowColor, __highlightColor, ramp);
				
				// output color
				half3 color = half3(0,0,0);
				color += albedo * lightColor.rgb * ramp;
				#if defined(TCP2_SKETCH)
				color.rgb *= sketch.rgb;
				#endif

				//Subsurface Scattering for Main Light
				half3 ssLight = lightDir + normalWS * __subsurfaceDistortion;
				half ssDot = pow(saturate(dot(viewDirWS, -ssLight)), __subsurfacePower) * __subsurfaceScale;
				half3 ssColor = (ssDot * __subsurfaceColor);
				ssColor *= lightColor;
				color.rgb += albedo * ssColor;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				uint pixelLightCount = GetAdditionalLightsCount();

				#if USE_FORWARD_PLUS
					// Additional directional lights in Forward+
					for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
					{
						FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

						Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);

						#if defined(_LIGHT_LAYERS)
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
						#endif
						{
							half atten = light.shadowAttenuation * light.distanceAttenuation;

							#if defined(_LIGHT_LAYERS)
								half3 lightDir = half3(0, 1, 0);
								half3 lightColor = half3(0, 0, 0);
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								{
									lightColor = light.color.rgb;
									lightDir = light.direction;
								}
							#else
								half3 lightColor = light.color.rgb;
								half3 lightDir = light.direction;
							#endif

							half ndl = dot(normalWS, lightDir);
							half3 ramp;
							
							ndl = saturate(ndl);
							ramp = step(rampThreshold, ndl);

							// apply attenuation (shadowmaps & point/spot lights attenuation)
							ramp *= atten;

							// apply highlight color
							ramp = lerp(half3(0,0,0), __highlightColor, ramp);
							
							// output color
							color += albedo * lightColor.rgb * ramp;

						}
					}

					// Data with dummy struct used in Forward+ macro (LIGHT_LOOP_BEGIN)
					InputDataForwardPlusDummy inputData;
					inputData.normalizedScreenSpaceUV = normalizedScreenSpaceUV;
					inputData.positionWS = positionWS;
				#endif

				LIGHT_LOOP_BEGIN(pixelLightCount)
				{
					#if defined(URP_10_OR_NEWER)
						Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);
					#else
						Light light = GetAdditionalLight(lightIndex, positionWS);
					#endif
					half atten = light.shadowAttenuation * light.distanceAttenuation;

					#if defined(_LIGHT_LAYERS)
						half3 lightDir = half3(0, 1, 0);
						half3 lightColor = half3(0, 0, 0);
						if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
						{
							lightColor = light.color.rgb;
							lightDir = light.direction;
						}
					#else
						half3 lightColor = light.color.rgb;
						half3 lightDir = light.direction;
					#endif

					half ndl = dot(normalWS, lightDir);
					half3 ramp;
					
					ndl = saturate(ndl);
					ramp = step(rampThreshold, ndl);

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					// apply highlight color
					ramp = lerp(half3(0,0,0), __highlightColor, ramp);
					
					// output color
					color += albedo * lightColor.rgb * ramp;

				}
				LIGHT_LOOP_END
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				// apply ambient
				color += indirectDiffuse;

				color += emission;
				
				// Vertical Fog
				half vertFogThreshold = input.worldPosAndFog.xyz.y;
				half verticalFogThreshold = __verticalFogThreshold;
				half verticalFogSmooothness = __verticalFogSmoothness;
				half verticalFogMin = verticalFogThreshold - verticalFogSmooothness;
				half verticalFogMax = verticalFogThreshold + verticalFogSmooothness;
				half4 fogColor = __verticalFogColor;
				#if defined(UNITY_PASS_FORWARDADD)
					fogColor.rgb = half3(0, 0, 0);
				#endif
				half vertFogFactor = 1 - saturate((vertFogThreshold - verticalFogMin) / (verticalFogMax - verticalFogMin));
				vertFogFactor *= fogColor.a;
				color.rgb = lerp(color.rgb, fogColor.rgb, vertFogFactor);

				return half4(color, alpha);
			}
			ENDHLSL
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE

		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;
			float3 _LightPosition;

			struct Attributes
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				half4 vertexColor : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
			#if defined(DEPTH_NORMALS_PASS)
				float3 normalWS : TEXCOORD0;
			#endif
				float4 screenPosition : TEXCOORD1;
				float3 pack1 : TEXCOORD2; /* pack1.xyz = positionWS */
				float2 pack2 : TEXCOORD3; /* pack2.xy = texcoord0 */
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normal);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				#if defined(DEPTH_ONLY_PASS)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				#endif

				// Texture Coordinates
				output.pack2.xy.xy = input.texcoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				// Shader Properties Sampling
				float __windTimeOffset = ( input.vertexColor.g );
				float __windSpeed = ( _WindSpeed );
				float __windFrequency = ( 1.0 );
				float3 __windDirection = ( _WindDirection.xyz );
				float3 __windMask = ( input.vertexColor.rrr );
				float __windStrength = ( _WindStrength );
				float __wavesFrequency = ( _WavesFrequency );
				float __wavesHeight = ( _WavesHeight );
				float __wavesSpeed = ( _WavesSpeed );

				float3 worldPos = mul(UNITY_MATRIX_M, input.vertex).xyz;
				#if defined(TCP2_WIND)
				// Wind Animation
				float windTimeOffset = __windTimeOffset;
				float windSpeed = __windSpeed;
				float3 windFrequency = worldPos.xyz * __windFrequency;
				float windPhase = (_Time.y + windTimeOffset) * windSpeed;
				float3 windFactor = sin(windPhase + windFrequency);
				float3 windDir = normalize(__windDirection);
				float3 windMask = __windMask;
				float windStrength = __windStrength;
				worldPos.xyz += windDir * windFactor * windMask * windStrength;
				#endif
				input.vertex.xyz = mul(UNITY_MATRIX_I_M, float4(worldPos, 1)).xyz;
				
				// Vertex water waves
				float _waveFrequency = __wavesFrequency;
				float _waveHeight = __wavesHeight;
				float3 _vertexWavePos = worldPos.xyz * _waveFrequency;
				float _phase = _Time.y * __wavesSpeed;
				float waveFactorX = sin(_vertexWavePos.x + _phase) * _waveHeight;
				float waveFactorZ = sin(_vertexWavePos.z + _phase) * _waveHeight;
				input.vertex.y += (waveFactorX + waveFactorZ);
				float xn = -_waveHeight * cos(_vertexWavePos.x + _phase);
				float zn = -_waveHeight * cos(_vertexWavePos.z + _phase);
				input.normal = normalize(float3(xn, 1, zn));
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);

				// Screen Space UV
				float4 screenPos = ComputeScreenPos(vertexInput.positionCS);
				output.screenPosition.xyzw = screenPos;
				output.pack1.xyz = vertexInput.positionWS;

				#if defined(DEPTH_ONLY_PASS)
					output.positionCS = TransformObjectToHClip(input.vertex.xyz);
					#if defined(DEPTH_NORMALS_PASS)
						float3 normalWS = TransformObjectToWorldNormal(input.normal);
						output.normalWS = normalWS; // already normalized in TransformObjectToWorldNormal
					#endif
				#elif defined(SHADOW_CASTER_PASS)
					output.positionCS = GetShadowPositionHClip(input);
				#else
					output.positionCS = float4(0,0,0,0);
				#endif

				return output;
			}

			half4 ShadowDepthPassFragment(
				Varyings input
	#if defined(DEPTH_NORMALS_PASS) && defined(_WRITE_RENDERING_LAYERS)
				, out float4 outRenderingLayers : SV_Target1
	#endif
			) : SV_TARGET
			{
				#if defined(DEPTH_ONLY_PASS)
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				#endif

				float3 positionWS = input.pack1.xyz;

				// Shader Properties Sampling
				float4 __albedo = ( TCP2_TEX2D_SAMPLE(_BaseMap, _BaseMap, input.pack2.xy).rgba );
				float4 __mainColor = ( _BaseColor.rgba );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __dissolveMap = ( TCP2_TEX2D_SAMPLE(_DissolveMap, _DissolveMap, input.pack2.xy).r );
				float __dissolveValue = ( _DissolveValue );
				float __dissolveGradientWidth = ( _DissolveGradientWidth );
				float __dissolveGradientStrength = ( 2.0 );

				half3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
				half3 albedo = half3(1,1,1);
				half alpha = __alpha;
				half3 emission = half3(0,0,0);
				
				//Dissolve
				half dissolveMap = __dissolveMap;
				half dissolveValue = __dissolveValue;
				half gradientWidth = __dissolveGradientWidth;
				float dissValue = dissolveValue*(1+2*gradientWidth) - gradientWidth;
				float dissolveUV = smoothstep(dissolveMap - gradientWidth, dissolveMap + gradientWidth, dissValue);
				clip((1-dissolveUV) - 0.001);
				half4 dissolveColor = ( TCP2_TEX2D_SAMPLE(_DissolveGradientTexture, _DissolveGradientTexture, dissolveUV.xx).rgba );
				dissolveColor *= __dissolveGradientStrength * dissolveUV;
				emission += dissolveColor.rgb;

				#if defined(DEPTH_NORMALS_PASS)
					#if defined(_WRITE_RENDERING_LAYERS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						outRenderingLayers = float4(EncodeMeshRenderingLayer(meshRenderingLayers), 0, 0, 0);
					#endif

					#if defined(URP_12_OR_NEWER)
						return float4(input.normalWS.xyz, 0.0);
					#else
						return float4(PackNormalOctRectEncode(TransformWorldToViewDir(input.normalWS, true)), 0.0, 0.0);
					#endif
				#endif

				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile SHADOW_CASTER_PASS

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			//--------------------------------------
			// Toony Colors Pro 2 keywords
			#pragma shader_feature_local_vertex TCP2_WIND

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthNormals"
			Tags
			{
				"LightMode" = "DepthNormals"
			}

			ZWrite On

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 2.0

			#pragma multi_compile_instancing

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS
			#pragma multi_compile DEPTH_NORMALS_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			ENDHLSL
		}

	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(ver:"2.9.18";unity:"6000.0.40f1";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","UNITY_2019_4","UNITY_2020_1","UNITY_2021_1","UNITY_2021_2","UNITY_2022_2","ENABLE_DEPTH_NORMALS_PASS","ENABLE_FORWARD_PLUS","TEMPLATE_LWRP","CRISP_RAMP_NO_AA","SHADOW_COLOR_MAIN_DIR","EMISSION","SUBSURFACE_SCATTERING","SS_DIR_LIGHTS","WIND_ANIM","WIND_SHADER_FEATURE","DISSOLVE_CLIP","VERTICAL_FOG","VERTICAL_FOG_ALPHA","VERTEX_SIN_WAVES","VSW_WORLDPOS","VERTEX_SIN_NORMALS","DEPTH_BUFFER_COLOR","DEPTH_BUFFER_FOAM","DEPTH_BUFFER_ALPHA","SHADER_BLENDING","SMOOTH_FOAM","WIND_ANIM_SIN","DISSOLVE","DISSOLVE_GRADIENT","PASS_SILHOUETTE","SILHOUETTE_STENCIL","ALPHA_BLENDING","SKETCH","SKETCH_AMBIENT","SKETCH_SHADER_FEATURE"];flags:list[];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0",BASEGEN_ALBEDO_DOWNSCALE="1",BASEGEN_MASKTEX_DOWNSCALE="1/2",BASEGEN_METALLIC_DOWNSCALE="1/4",BASEGEN_SPECULAR_DOWNSCALE="1/4",BASEGEN_DIFFUSEREMAPMIN_DOWNSCALE="1/4",BASEGEN_MASKMAPREMAPMIN_DOWNSCALE="1/4"];shaderProperties:list[];customTextures:list[];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH 47b1bdc11e0335855d6ecbdd9898a435 */
