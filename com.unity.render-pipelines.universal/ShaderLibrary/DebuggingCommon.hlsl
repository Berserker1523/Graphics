
#ifndef DEBUGGING_COMMON_INCLUDED
#define DEBUGGING_COMMON_INCLUDED

#define DEBUG_MATERIAL_NONE 0
#define DEBUG_MATERIAL_UNLIT 1
#define DEBUG_MATERIAL_DIFFUSE 2
#define DEBUG_MATERIAL_SPECULAR 3
#define DEBUG_MATERIAL_ALPHA 4
#define DEBUG_MATERIAL_SMOOTHNESS 5
#define DEBUG_MATERIAL_OCCLUSION 6
#define DEBUG_MATERIAL_EMISSION 7
#define DEBUG_MATERIAL_NORMAL_WORLD_SPACE 8
#define DEBUG_MATERIAL_NORMAL_TANGENT_SPACE 9
#define DEBUG_MATERIAL_LIGHTING_COMPLEXITY 10
#define DEBUG_MATERIAL_LOD 11
#define DEBUG_MATERIAL_METALLIC 12
int _DebugMaterialIndex;

#define DEBUG_LIGHTING_NONE 0
#define DEBUG_LIGHTING_SHADOW_CASCADES 1
#define DEBUG_LIGHTING_LIGHT_ONLY 2
#define DEBUG_LIGHTING_LIGHT_DETAIL 3
#define DEBUG_LIGHTING_REFLECTIONS 4
#define DEBUG_LIGHTING_REFLECTIONS_WITH_SMOOTHNESS 5
int _DebugLightingIndex;

#define DEBUG_ATTRIBUTE_TEXCOORD0 1
#define DEBUG_ATTRIBUTE_TEXCOORD1 2
#define DEBUG_ATTRIBUTE_TEXCOORD2 3
#define DEBUG_ATTRIBUTE_TEXCOORD3 4
#define DEBUG_ATTRIBUTE_COLOR     5
#define DEBUG_ATTRIBUTE_TANGENT   6
#define DEBUG_ATTRIBUTE_NORMAL    7
int _DebugAttributesIndex;

#define DEBUG_LIGHTING_FEATURE_GI 0
#define DEBUG_LIGHTING_FEATURE_MAIN_LIGHT 1
#define DEBUG_LIGHTING_FEATURE_ADDITIONAL_LIGHTS 2
#define DEBUG_LIGHTING_FEATURE_VERTEX_LIGHTING 3
#define DEBUG_LIGHTING_FEATURE_EMISSION 4
int _DebugLightingFeatureMask;

#define DEBUG_MIPMAPMODE_NONE 0
#define DEBUG_MIPMAPMODE_MIP_LEVEL 1
#define DEBUG_MIPMAPMODE_MIP_COUNT 2
#define DEBUG_MIPMAPMODE_MIP_COUNT_REDUCTION 3
//#define DEBUG_MIPMAPMODE_STREAMING_MIP_BUDGET 4
//#define DEBUG_MIPMAPMODE_STREAMING_MIP 5
int _DebugMipIndex;

sampler2D _DebugNumberTexture;

#define DEBUG_VALIDATION_NONE 0
//#define DEBUG_VALIDATION_HIGHLIGHT_NAN_INFINITE_NEGATIVE 1
//#define DEBUG_VALIDATION_HIGHLIGHT_OUTSIDE_RANGE 2
#define DEBUG_VALIDATION_ALBEDO 3
#define DEBUG_VALIDATION_METAL 4
int _DebugValidationIndex;

// Set of colors that should still provide contrast for the Color-blind
#define kPurpleColor float4(156.0 / 255.0, 79.0 / 255.0, 255.0 / 255.0, 1.0) // #9C4FFF
#define kRedColor float4(203.0 / 255.0, 48.0 / 255.0, 34.0 / 255.0, 1.0) // #CB3022
#define kGreenColor float4(8.0 / 255.0, 215.0 / 255.0, 139.0 / 255.0, 1.0) // #08D78B
#define kYellowGreenColor float4(151.0 / 255.0, 209.0 / 255.0, 61.0 / 255.0, 1.0) // #97D13D
#define kBlueColor float4(75.0 / 255.0, 146.0 / 255.0, 243.0 / 255.0, 1.0) // #4B92F3
#define kOrangeBrownColor float4(219.0 / 255.0, 119.0 / 255.0, 59.0 / 255.0, 1.0) // #4B92F3
#define kGrayColor float4(174.0 / 255.0, 174.0 / 255.0, 174.0 / 255.0, 1.0) // #AEAEAE

#endif
