// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VertexLightmap"
{
	Properties
	{
		_FilterColor("Filter Color", Color) = (1,1,1,1)
		_Gain("Gain", Range( 0 , 2)) = 1
		_Contrast("Contrast", Range( 0 , 2)) = 1
		_Saturation("Saturation", Range( 0 , 2)) = 1
		_MetallicAdd("Metallic Add", Range( -1 , 1)) = 0
		_SmoothnessAdd("Smoothness Add", Range( -1 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float4 _FilterColor;
		uniform float _Gain;
		uniform float _Contrast;
		uniform float _Saturation;
		uniform float _MetallicAdd;
		uniform float _SmoothnessAdd;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_35_0 = ( _Contrast + -1.0 );
			float3 desaturateInitialColor31 = ( ( ( ( i.vertexColor * _FilterColor ) * _Gain ) * ( temp_output_35_0 + 1.0 ) ) + ( ( temp_output_35_0 / 2.0 ) * -1.0 ) ).rgb;
			float desaturateDot31 = dot( desaturateInitialColor31, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar31 = lerp( desaturateInitialColor31, desaturateDot31.xxx, ( 1.0 - _Saturation ) );
			o.Albedo = desaturateVar31;
			float3 temp_cast_1 = (( i.vertexColor.a * 5.0 )).xxx;
			o.Emission = temp_cast_1;
			o.Metallic = ( i.uv_texcoord.x + _MetallicAdd );
			o.Smoothness = ( i.uv_texcoord.y + _SmoothnessAdd );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
619;73;1157;812;1524.684;696.6261;2.039767;True;False
Node;AmplifyShaderEditor.RangedFloatNode;23;-76.02727,-134.7759;Inherit;False;Property;_Contrast;Contrast;2;0;Create;True;0;0;False;0;1;1.39;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-662.0914,-165.6252;Inherit;False;Property;_FilterColor;Filter Color;0;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1;-740.2897,76.96768;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;35;324.0895,-165.8474;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-261.7199,10.33867;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;59.40392,139.5694;Inherit;False;Property;_Gain;Gain;1;0;Create;True;0;0;False;0;1;0.956;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;378.2604,81.52734;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;499.6698,-185.7427;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;499.4156,-71.6389;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;664.3459,-182.6155;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;662.0125,-59.0876;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;662.7357,130.8492;Inherit;False;Property;_Saturation;Saturation;3;0;Create;True;0;0;False;0;1;1.044;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;34;962.0978,131.9193;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;248.6395,619.2271;Inherit;False;Property;_SmoothnessAdd;Smoothness Add;5;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;7;310.2982,441.4619;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;241.2206,352.5972;Inherit;False;Property;_MetallicAdd;Metallic Add;4;0;Create;True;0;0;False;0;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;844.0658,-162.123;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;385.4705,210.8337;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;31;1137.122,141.6556;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;718.2598,534.6035;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;716.5059,410.5861;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1709.891,184.2711;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VertexLightmap;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;23;0
WireConnection;37;0;1;0
WireConnection;37;1;36;0
WireConnection;11;0;37;0
WireConnection;11;1;12;0
WireConnection;25;0;35;0
WireConnection;26;0;35;0
WireConnection;27;0;11;0
WireConnection;27;1;25;0
WireConnection;30;0;26;0
WireConnection;34;0;32;0
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;8;0;1;4
WireConnection;31;0;29;0
WireConnection;31;1;34;0
WireConnection;19;0;7;2
WireConnection;19;1;14;0
WireConnection;18;0;7;1
WireConnection;18;1;15;0
WireConnection;0;0;31;0
WireConnection;0;2;8;0
WireConnection;0;3;18;0
WireConnection;0;4;19;0
ASEEND*/
//CHKSM=90740E3596674AA00BABEE52880AB2631A1AEDE3