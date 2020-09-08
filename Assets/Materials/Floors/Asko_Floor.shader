// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify/Asko_Floor"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_TextureSample2("Texture Sample 2", 2D) = "bump" {}
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample2;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _TextureSample3;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord6 = i.uv_texcoord * float2( 7,4.5 ) + float2( 0.85,0 );
			o.Normal = UnpackNormal( tex2D( _TextureSample2, uv_TexCoord6 ) );
			float2 uv_TexCoord7 = i.uv_texcoord * float2( 5,3 ) + float2( 0.9,0 );
			float4 blendOpSrc5 = tex2D( _TextureSample1, uv_TexCoord7 );
			float4 blendOpDest5 = tex2D( _TextureSample0, uv_TexCoord6 );
			o.Albedo = ( saturate( (( blendOpDest5 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest5 ) * ( 1.0 - blendOpSrc5 ) ) : ( 2.0 * blendOpDest5 * blendOpSrc5 ) ) )).rgb;
			float4 tex2DNode4 = tex2D( _TextureSample3, uv_TexCoord6 );
			o.Metallic = tex2DNode4.r;
			o.Smoothness = ( tex2DNode4.a * 1.77 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18301
3072;368.2;1920;1018;1274;270;1;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1094,-206;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;7,4.5;False;1;FLOAT2;0.85,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1040,115;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,3;False;1;FLOAT2;0.9,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-657,-9;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;False;-1;81e4311f6ae57354583fb298fc1a610e;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-659,-193;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;bbbe9ca50283e2846b1d074e9a03087a;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-530,378;Inherit;True;Property;_TextureSample3;Texture Sample 3;3;0;Create;True;0;0;False;0;False;-1;6506e838ccdcef642b3529b5bd3f615e;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-191,490;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;False;1.77;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-548,185;Inherit;True;Property;_TextureSample2;Texture Sample 2;2;0;Create;True;0;0;False;0;False;-1;28b1b55bd9609c74d9fa83e8b965dda5;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;5;-249,-50;Inherit;False;Overlay;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-210,239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89,-23;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Amplify/Asko_Floor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;1;7;0
WireConnection;1;1;6;0
WireConnection;4;1;6;0
WireConnection;3;1;6;0
WireConnection;5;0;2;0
WireConnection;5;1;1;0
WireConnection;9;0;4;4
WireConnection;9;1;8;0
WireConnection;0;0;5;0
WireConnection;0;1;3;0
WireConnection;0;3;4;0
WireConnection;0;4;9;0
ASEEND*/
//CHKSM=F197817AA8D15CFC6CC0846AD4FB7E2806CA57AB