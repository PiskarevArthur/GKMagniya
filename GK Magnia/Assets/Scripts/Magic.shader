// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Magic"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (0,0,0,0)
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_MagicEmission("Magic Emission", Range( 0 , 10)) = 0
		_MagicEmissionColorSaturation("Magic Emission Color Saturation", Range( 0 , 1)) = 0
		_MagicColorSaturation("Magic Color Saturation", Range( 0 , 1)) = 0
		_MagicColorGain("Magic Color Gain", Range( 0 , 3)) = 1
		_MagicPosterize("Magic Posterize", Range( 1 , 255)) = 1
		_MagicScale("Magic Scale", Range( 0.1 , 20)) = 1
		_MagicSpeed("Magic Speed", Range( 0 , 0.5)) = 0.05
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _BaseColor;
		uniform float _MagicPosterize;
		uniform float _MagicSpeed;
		uniform float _MagicScale;
		uniform float _MagicColorSaturation;
		uniform float _MagicColorGain;
		uniform float _MagicEmissionColorSaturation;
		uniform float _MagicEmission;
		uniform float _Metallic;
		uniform float _Smoothness;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_54_0 = ( _Time.y * _MagicSpeed );
			float2 temp_cast_1 = (temp_output_54_0).xx;
			float2 uv_TexCoord7 = i.uv_texcoord * float2( 1,1 ) + temp_cast_1;
			float simplePerlin3D6 = snoise( float3( uv_TexCoord7 ,  0.0 )*_MagicScale );
			simplePerlin3D6 = simplePerlin3D6*0.5 + 0.5;
			float2 temp_cast_3 = (temp_output_54_0).xx;
			float2 uv_TexCoord16 = i.uv_texcoord * float2( 2,2 ) + temp_cast_3;
			float simplePerlin3D17 = snoise( float3( uv_TexCoord16 ,  0.0 )*_MagicScale );
			simplePerlin3D17 = simplePerlin3D17*0.5 + 0.5;
			float2 temp_cast_5 = (temp_output_54_0).xx;
			float2 uv_TexCoord19 = i.uv_texcoord * float2( 3,3 ) + temp_cast_5;
			float simplePerlin3D20 = snoise( float3( uv_TexCoord19 ,  0.0 )*_MagicScale );
			simplePerlin3D20 = simplePerlin3D20*0.5 + 0.5;
			float3 appendResult15 = (float3(simplePerlin3D6 , simplePerlin3D17 , simplePerlin3D20));
			float div47=256.0/float((int)_MagicPosterize);
			float4 posterize47 = ( floor( float4( appendResult15 , 0.0 ) * div47 ) / div47 );
			float grayscale34 = Luminance(posterize47.rgb);
			float4 temp_cast_9 = (grayscale34).xxxx;
			float4 lerpResult36 = lerp( temp_cast_9 , posterize47 , _MagicColorSaturation);
			o.Albedo = ( _BaseColor + ( lerpResult36 * _MagicColorGain ) ).rgb;
			float4 temp_cast_11 = (grayscale34).xxxx;
			float4 lerpResult90 = lerp( temp_cast_11 , posterize47 , _MagicEmissionColorSaturation);
			o.Emission = ( ( _BaseColor + lerpResult90 ) * _MagicEmission ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
587;73;1165;928;673.2268;970.7169;1;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;52;-2102.198,-732.1199;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2052.198,-555.1199;Inherit;False;Property;_MagicSpeed;Magic Speed;9;0;Create;True;0;0;False;0;0.05;1;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;8;-1651.519,-807.6146;Inherit;False;Constant;_XY;XY;0;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1752.198,-644.1199;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;18;-1654.881,-472.4685;Inherit;False;Constant;_XY2;XY2;1;0;Create;True;0;0;False;0;2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;21;-1640.981,-276.5685;Inherit;False;Constant;_XY3;XY3;2;0;Create;True;0;0;False;0;3,3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1387.839,-509.2527;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1396.939,-349.3527;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1396.477,-664.3988;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-1463.905,-811.5425;Inherit;False;Property;_MagicScale;Magic Scale;8;0;Create;True;0;0;False;0;1;0;0.1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;6;-1130.45,-654.4013;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;20;-1125.913,-341.3552;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;17;-1116.813,-504.2552;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-594.3306,-732.488;Inherit;False;Property;_MagicPosterize;Magic Posterize;7;0;Create;True;0;0;False;0;1;1;1;255;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;15;-819.1929,-601.8893;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosterizeNode;47;-306.4415,-600.488;Inherit;False;1;2;1;COLOR;0,0,0,0;False;0;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;34;-67.28741,-804.5891;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-86.88521,-592.1811;Inherit;False;Property;_MagicColorSaturation;Magic Color Saturation;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-79.4774,-442.3065;Inherit;False;Property;_MagicEmissionColorSaturation;Magic Emission Color Saturation;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;36;238.0612,-683.8761;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;90;300.4208,-527.8387;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;57;417.6305,-962.2847;Inherit;False;Property;_BaseColor;Base Color;0;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;174.1309,-786.7858;Inherit;False;Property;_MagicColorGain;Magic Color Gain;6;0;Create;True;0;0;False;0;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;885.819,-536.7266;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;525.5939,-699.9531;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;713.8134,-385.2078;Inherit;False;Property;_MagicEmission;Magic Emission;3;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;460.4083,-214.1834;Inherit;False;Property;_Metallic;Metallic;1;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;518.8742,-73.05251;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;878.0433,-661.1385;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;1081.116,-438.4368;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1312.013,-475.4013;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Magic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;52;0
WireConnection;54;1;55;0
WireConnection;16;0;18;0
WireConnection;16;1;54;0
WireConnection;19;0;21;0
WireConnection;19;1;54;0
WireConnection;7;0;8;0
WireConnection;7;1;54;0
WireConnection;6;0;7;0
WireConnection;6;1;25;0
WireConnection;20;0;19;0
WireConnection;20;1;25;0
WireConnection;17;0;16;0
WireConnection;17;1;25;0
WireConnection;15;0;6;0
WireConnection;15;1;17;0
WireConnection;15;2;20;0
WireConnection;47;1;15;0
WireConnection;47;0;48;0
WireConnection;34;0;47;0
WireConnection;36;0;34;0
WireConnection;36;1;47;0
WireConnection;36;2;37;0
WireConnection;90;0;34;0
WireConnection;90;1;47;0
WireConnection;90;2;89;0
WireConnection;88;0;57;0
WireConnection;88;1;90;0
WireConnection;39;0;36;0
WireConnection;39;1;38;0
WireConnection;87;0;57;0
WireConnection;87;1;39;0
WireConnection;24;0;88;0
WireConnection;24;1;23;0
WireConnection;0;0;87;0
WireConnection;0;2;24;0
WireConnection;0;3;26;0
WireConnection;0;4;27;0
ASEEND*/
//CHKSM=8AFE6B15D3F37BCF06000CF059B7A9F9CF009F1C