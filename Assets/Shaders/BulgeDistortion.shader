Shader "Custom/BulgeDistortion"
{
    Properties
    {
        [HideInInspector][NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [Header(Head)]
        _Enable_Head ("Enable Head", Int) = 1
        _DistortionTex_Head ("Distortion Mask Head (PNG)", 2D) = "black" {}
        _Center_Head ("Center Head (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_Head ("Angle Head", Float) = 0.0
        _CenterOffset_Head ("Center Offset Head XY", Vector) = (0, 0, 0, 0)
        _Radius_Head ("Radius Head", Range(0.01, 1.0)) = 0.3
        _Scale_Head ("Scale Head XY", Vector) = (1, 1, 1, 1)
        _Strength_Head ("Strength Head", Range(-5.0, 5.0)) = 0.8
        _Softness_Head ("Softness Head", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_Head ("Feather Distance Head", Range(0.0, 1.0)) = 0.2
        _DepthFactor_Head ("Depth Factor Head", Float) = 1.0
        [Header(Body)]
        _Enable_Body ("Enable Body", Int) = 1
        _DistortionTex_Body ("Distortion Mask Body (PNG)", 2D) = "black" {}
        _Center_Body ("Center Body (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_Body ("Angle Body", Float) = 0.0
        _CenterOffset_Body ("Center Offset Body XY", Vector) = (0, 0, 0, 0)
        _Radius_Body ("Radius Body", Range(0.01, 1.0)) = 0.3
        _Scale_Body ("Scale Body XY", Vector) = (1, 1, 1, 1)
        _Strength_Body ("Strength Body", Range(-5.0, 5.0)) = 0.8
        _Softness_Body ("Softness Body", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_Body ("Feather Distance Body", Range(0.0, 1.0)) = 0.2
        _DepthFactor_Body ("Depth Factor Body", Float) = 1.0
        [Header(Left Hand)]
        _DistortionTex_LeftHand ("Distortion Mask Left Hand (PNG)", 2D) = "black" {}
        _Center_LeftHand ("Center Left Hand (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_LeftHand ("Angle Left Hand", Float) = 0.0
        _CenterOffset_LeftHand ("Center Offset Left Hand XY", Vector) = (0, 0, 0, 0)
        _Radius_LeftHand ("Radius Left Hand", Range(0.01, 1.0)) = 0.3
        _Scale_LeftHand ("Scale Left Hand XY", Vector) = (1, 1, 1, 1)
        _Strength_LeftHand ("Strength Left Hand", Range(-5.0, 5.0)) = 0.8
        _Softness_LeftHand ("Softness Left Hand", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_LeftHand ("Feather Distance Left Hand", Range(0.0, 1.0)) = 0.2
        _DepthFactor_LeftHand ("Depth Factor Left Hand", Float) = 1.0
        [Header(Right Hand)]
        _DistortionTex_RightHand ("Distortion Mask Right Hand (PNG)", 2D) = "black" {}
        _Center_RightHand ("Center Right Hand (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_RightHand ("Angle Right Hand", Float) = 0.0
        _CenterOffset_RightHand ("Center Offset Right Hand XY", Vector) = (0, 0, 0, 0)
        _Radius_RightHand ("Radius Right Hand", Range(0.01, 1.0)) = 0.3
        _Scale_RightHand ("Scale Right Hand XY", Vector) = (1, 1, 1, 1)
        _Strength_RightHand ("Strength Right Hand", Range(-5.0, 5.0)) = 0.8
        _Softness_RightHand ("Softness Right Hand", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_RightHand ("Feather Distance Right Hand", Range(0.0, 1.0)) = 0.2
        _DepthFactor_RightHand ("Depth Factor Right Hand", Float) = 1.0
        [Header(Left Arm)]
        _Enable_LeftArm ("Enable Left Arm", Int) = 1
        _DistortionTex_LeftArm ("Distortion Mask Left Arm (PNG)", 2D) = "black" {}
        _CenterOffset_LeftArm ("Center Offset Left Arm XY", Vector) = (0, 0, 0, 0)
        _Radius_LeftArm ("Radius Left Arm", Range(0.01, 1.0)) = 0.3
        _Strength_LeftArm ("Strength Left Arm", Range(-5.0, 5.0)) = 0.8
        _Softness_LeftArm ("Softness Left Arm", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_LeftArm ("Feather Distance Left Arm", Range(0.0, 1.0)) = 0.2
        _DepthFactor_LeftArm ("Depth Factor Left Arm", Float) = 1.0
        _Center_LeftArm1 ("Center Left Arm 1 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_LeftArm1 ("Angle Left Arm 1", Float) = 0.0
        _Scale_LeftArm1 ("Scale Left Arm 1 XY", Vector) = (1, 1, 1, 1)
        _Center_LeftArm2 ("Center Left Arm 2 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_LeftArm2 ("Angle Left Arm 2", Float) = 0.0
        _Scale_LeftArm2 ("Scale Left Arm 2 XY", Vector) = (1, 1, 1, 1)
        [Header(Right Arm)]
        _Enable_RightArm ("Enable Right Arm", Int) = 1
        _DistortionTex_RightArm ("Distortion Mask Right Arm (PNG)", 2D) = "black" {}
        _CenterOffset_RightArm ("Center Offset Right Arm XY", Vector) = (0, 0, 0, 0)
        _Radius_RightArm ("Radius Right Arm", Range(0.01, 1.0)) = 0.3
        _Strength_RightArm ("Strength Right Arm", Range(-5.0, 5.0)) = 0.8
        _Softness_RightArm ("Softness Right Arm", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_RightArm ("Feather Distance Right Arm", Range(0.0, 1.0)) = 0.2
        _DepthFactor_RightArm ("Depth Factor Right Arm", Float) = 1.0
        _Center_RightArm1 ("Center Right Arm 1 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_RightArm1 ("Angle Right Arm 1", Float) = 0.0
        _Scale_RightArm1 ("Scale Right Arm 1 XY", Vector) = (1, 1, 1, 1)
        _Center_RightArm2 ("Center Right Arm 2 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_RightArm2 ("Angle Right Arm 2", Float) = 0.0
        _Scale_RightArm2 ("Scale Right Arm 2 XY", Vector) = (1, 1, 1, 1)
        [Header(Left Leg)]
        _Enable_LeftLeg ("Enable Left Leg", Int) = 1
        _DistortionTex_LeftLeg ("Distortion Mask Left Leg (PNG)", 2D) = "black" {}
        _CenterOffset_LeftLeg ("Center Offset Left Leg XY", Vector) = (0, 0, 0, 0)
        _Radius_LeftLeg ("Radius Left Leg", Range(0.01, 1.0)) = 0.3
        _Strength_LeftLeg ("Strength Left Leg", Range(-5.0, 5.0)) = 0.8
        _Softness_LeftLeg ("Softness Left Leg", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_LeftLeg ("Feather Distance Left Leg", Range(0.0, 1.0)) = 0.2
        _DepthFactor_LeftLeg ("Depth Factor Left Leg", Float) = 1.0
        _Asymmetry_LeftLeg ("Asymmetry Left Leg (-1 fix end/pull start .. 1 fix start/pull end)", Range(-1.0, 1.0)) = 0.0
        _Center_LeftLeg1 ("Center Left Leg 1 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_LeftLeg1 ("Angle Left Leg 1", Float) = 0.0
        _Scale_LeftLeg1 ("Scale Left Leg 1 XY", Vector) = (1, 1, 1, 1)
        _Center_LeftLeg2 ("Center Left Leg 2 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_LeftLeg2 ("Angle Left Leg 2", Float) = 0.0
        _Scale_LeftLeg2 ("Scale Left Leg 2 XY", Vector) = (1, 1, 1, 1)
        [Header(Right Leg)]
        _Enable_RightLeg ("Enable Right Leg", Int) = 1
        _DistortionTex_RightLeg ("Distortion Mask Right Leg (PNG)", 2D) = "black" {}
        _CenterOffset_RightLeg ("Center Offset Right Leg XY", Vector) = (0, 0, 0, 0)
        _Radius_RightLeg ("Radius Right Leg", Range(0.01, 1.0)) = 0.3
        _Strength_RightLeg ("Strength Right Leg", Range(-5.0, 5.0)) = 0.8
        _Softness_RightLeg ("Softness Right Leg", Range(0.0, 1.0)) = 0.0
        _FeatherDistance_RightLeg ("Feather Distance Right Leg", Range(0.0, 1.0)) = 0.2
        _DepthFactor_RightLeg ("Depth Factor Right Leg", Float) = 1.0
        _Asymmetry_RightLeg ("Asymmetry Right Leg (-1 fix end/pull start .. 1 fix start/pull end)", Range(-1.0, 1.0)) = 0.0
        _Center_RightLeg1 ("Center Right Leg 1 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_RightLeg1 ("Angle Right Leg 1", Float) = 0.0
        _Scale_RightLeg1 ("Scale Right Leg 1 XY", Vector) = (1, 1, 1, 1)
        _Center_RightLeg2 ("Center Right Leg 2 (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Angle_RightLeg2 ("Angle Right Leg 2", Float) = 0.0
        _Scale_RightLeg2 ("Scale Right Leg 2 XY", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend Off
        Cull Off
        ZWrite Off
        ZTest Always
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
                float4 vertex : SV_POSITION;
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            // Head
            int _Enable_Head;
            sampler2D _DistortionTex_Head;
            float4 _DistortionTex_Head_TexelSize;
            float2 _Center_Head;
            float _Angle_Head;
            float2 _CenterOffset_Head;
            float _Radius_Head;
            float2 _Scale_Head;
            float _Strength_Head;
            float _Softness_Head;
            float _FeatherDistance_Head;
            float _DepthFactor_Head;
            // Body
            int _Enable_Body;
            sampler2D _DistortionTex_Body;
            float4 _DistortionTex_Body_TexelSize;
            float2 _Center_Body;
            float _Angle_Body;
            float2 _CenterOffset_Body;
            float _Radius_Body;
            float2 _Scale_Body;
            float _Strength_Body;
            float _Softness_Body;
            float _FeatherDistance_Body;
            float _DepthFactor_Body;
            // Left Hand
            sampler2D _DistortionTex_LeftHand;
            float4 _DistortionTex_LeftHand_TexelSize;
            float2 _Center_LeftHand;
            float _Angle_LeftHand;
            float2 _CenterOffset_LeftHand;
            float _Radius_LeftHand;
            float2 _Scale_LeftHand;
            float _Strength_LeftHand;
            float _Softness_LeftHand;
            float _FeatherDistance_LeftHand;
            float _DepthFactor_LeftHand;
            // Right Hand
            sampler2D _DistortionTex_RightHand;
            float4 _DistortionTex_RightHand_TexelSize;
            float2 _Center_RightHand;
            float _Angle_RightHand;
            float2 _CenterOffset_RightHand;
            float _Radius_RightHand;
            float2 _Scale_RightHand;
            float _Strength_RightHand;
            float _Softness_RightHand;
            float _FeatherDistance_RightHand;
            float _DepthFactor_RightHand;
            // Left Arm
            int _Enable_LeftArm;
            sampler2D _DistortionTex_LeftArm;
            float4 _DistortionTex_LeftArm_TexelSize;
            float2 _CenterOffset_LeftArm;
            float _Radius_LeftArm;
            float _Strength_LeftArm;
            float _Softness_LeftArm;
            float _FeatherDistance_LeftArm;
            float _DepthFactor_LeftArm;
            float2 _Center_LeftArm1;
            float _Angle_LeftArm1;
            float2 _Scale_LeftArm1;
            float2 _Center_LeftArm2;
            float _Angle_LeftArm2;
            float2 _Scale_LeftArm2;
            // Right Arm
            int _Enable_RightArm;
            sampler2D _DistortionTex_RightArm;
            float4 _DistortionTex_RightArm_TexelSize;
            float2 _CenterOffset_RightArm;
            float _Radius_RightArm;
            float _Strength_RightArm;
            float _Softness_RightArm;
            float _FeatherDistance_RightArm;
            float _DepthFactor_RightArm;
            float2 _Center_RightArm1;
            float _Angle_RightArm1;
            float2 _Scale_RightArm1;
            float2 _Center_RightArm2;
            float _Angle_RightArm2;
            float2 _Scale_RightArm2;
            // Left Leg
            int _Enable_LeftLeg;
            sampler2D _DistortionTex_LeftLeg;
            float4 _DistortionTex_LeftLeg_TexelSize;
            float2 _CenterOffset_LeftLeg;
            float _Radius_LeftLeg;
            float _Strength_LeftLeg;
            float _Softness_LeftLeg;
            float _FeatherDistance_LeftLeg;
            float _DepthFactor_LeftLeg;
            float _Asymmetry_LeftLeg;
            float2 _Center_LeftLeg1;
            float _Angle_LeftLeg1;
            float2 _Scale_LeftLeg1;
            float2 _Center_LeftLeg2;
            float _Angle_LeftLeg2;
            float2 _Scale_LeftLeg2;
            // Right Leg
            int _Enable_RightLeg;
            sampler2D _DistortionTex_RightLeg;
            float4 _DistortionTex_RightLeg_TexelSize;
            float2 _CenterOffset_RightLeg;
            float _Radius_RightLeg;
            float _Strength_RightLeg;
            float _Softness_RightLeg;
            float _FeatherDistance_RightLeg;
            float _DepthFactor_RightLeg;
            float _Asymmetry_RightLeg;
            float2 _Center_RightLeg1;
            float _Angle_RightLeg1;
            float2 _Scale_RightLeg1;
            float2 _Center_RightLeg2;
            float _Angle_RightLeg2;
            float2 _Scale_RightLeg2;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float2 ApplyBulge(float2 uv, float2 centerParam, float angle, float2 centerOffset, float radius, float2 scaleParam, float strength, float softness, float featherDistance, sampler2D distTex, float4 distTexSize, float depthFactor, float asymmetry)
            {
                float2 center = centerParam;
                float2 effectiveCenter = center + centerOffset;
                float c = cos(angle);
                float s = sin(angle);
                float2x2 invRot = float2x2(c, s, -s, c);
                float2x2 rot = float2x2(c, -s, s, c);
                // Shift the center along the segment direction based on asymmetry
                float effectiveRadius = radius * depthFactor;
                float halfLocalLength = effectiveRadius * scaleParam.y;
                float shiftAmount = asymmetry * halfLocalLength;
                float2 segmentDir = mul(rot, float2(0, 1)); // unit vector along positive local y
                effectiveCenter += segmentDir * shiftAmount;
                float2 offset = uv - effectiveCenter;
                float2 localOffset = mul(invRot, offset);
                float2 scale = scaleParam;
                float2 localScaled = localOffset / scale;
                float dist = length(localScaled) / effectiveRadius;
                bool hasDistortionTex = (distTexSize.z > 2);
                float mask = 0.0;
                if (hasDistortionTex)
                {
                    if (dist < 1.0)
                    {
                        float2 norm = localScaled / effectiveRadius;
                        float2 maskUV = 0.5 + norm * 0.5;
                        if (softness < 0.001)
                        {
                            float4 maskTex = tex2D(distTex, maskUV);
                            mask = maskTex.r * maskTex.a;
                        }
                        else
                        {
                            float blur = softness * featherDistance * 2.5;
                            mask += tex2D(distTex, maskUV).r * tex2D(distTex, maskUV).a * 2.0;
                            mask += tex2D(distTex, maskUV + float2(0, blur)).r * tex2D(distTex, maskUV + float2(0, blur)).a;
                            mask += tex2D(distTex, maskUV + float2(0, -blur)).r * tex2D(distTex, maskUV + float2(0, -blur)).a;
                            mask += tex2D(distTex, maskUV + float2(blur, 0)).r * tex2D(distTex, maskUV + float2(blur, 0)).a;
                            mask += tex2D(distTex, maskUV + float2(-blur, 0)).r * tex2D(distTex, maskUV + float2(-blur, 0)).a;
                            mask /= 6.0;
                        }
                    }
                }
                else
                {
                    if (dist < 1.0)
                    {
                        float distPow = dist * dist;
                        mask = 1.0 / (1.0 + distPow);
                        if (softness > 0.001)
                        {
                            float edge = max(0.0, 1.0 - softness * 2.5);
                            mask *= 1.0 - smoothstep(edge, 1.0, dist);
                        }
                    }
                }
                float2 distortedLocal = localOffset * lerp(1.0, strength, mask);
                float2 distortedOffset = mul(rot, distortedLocal);
                return effectiveCenter + distortedOffset;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                if (_Enable_Head) uv = ApplyBulge(uv, _Center_Head, _Angle_Head, _CenterOffset_Head, _Radius_Head, _Scale_Head, _Strength_Head, _Softness_Head, _FeatherDistance_Head, _DistortionTex_Head, _DistortionTex_Head_TexelSize, _DepthFactor_Head, 0.0);
                if (_Enable_Body) uv = ApplyBulge(uv, _Center_Body, _Angle_Body, _CenterOffset_Body, _Radius_Body, _Scale_Body, _Strength_Body, _Softness_Body, _FeatherDistance_Body, _DistortionTex_Body, _DistortionTex_Body_TexelSize, _DepthFactor_Body, 0.0);
                if (_Enable_LeftArm)
                {
                    uv = ApplyBulge(uv, _Center_LeftHand, _Angle_LeftHand, _CenterOffset_LeftHand, _Radius_LeftHand, _Scale_LeftHand, _Strength_LeftHand, _Softness_LeftHand, _FeatherDistance_LeftHand, _DistortionTex_LeftHand, _DistortionTex_LeftHand_TexelSize, _DepthFactor_LeftHand, 0.0);
                    uv = ApplyBulge(uv, _Center_LeftArm1, _Angle_LeftArm1, _CenterOffset_LeftArm, _Radius_LeftArm, _Scale_LeftArm1, _Strength_LeftArm, _Softness_LeftArm, _FeatherDistance_LeftArm, _DistortionTex_LeftArm, _DistortionTex_LeftArm_TexelSize, _DepthFactor_LeftArm, 0.0);
                    uv = ApplyBulge(uv, _Center_LeftArm2, _Angle_LeftArm2, _CenterOffset_LeftArm, _Radius_LeftArm, _Scale_LeftArm2, _Strength_LeftArm, _Softness_LeftArm, _FeatherDistance_LeftArm, _DistortionTex_LeftArm, _DistortionTex_LeftArm_TexelSize, _DepthFactor_LeftArm, 0.0);
                }
                if (_Enable_RightArm)
                {
                    uv = ApplyBulge(uv, _Center_RightHand, _Angle_RightHand, _CenterOffset_RightHand, _Radius_RightHand, _Scale_RightHand, _Strength_RightHand, _Softness_RightHand, _FeatherDistance_RightHand, _DistortionTex_RightHand, _DistortionTex_RightHand_TexelSize, _DepthFactor_RightHand, 0.0);
                    uv = ApplyBulge(uv, _Center_RightArm1, _Angle_RightArm1, _CenterOffset_RightArm, _Radius_RightArm, _Scale_RightArm1, _Strength_RightArm, _Softness_RightArm, _FeatherDistance_RightArm, _DistortionTex_RightArm, _DistortionTex_RightArm_TexelSize, _DepthFactor_RightArm, 0.0);
                    uv = ApplyBulge(uv, _Center_RightArm2, _Angle_RightArm2, _CenterOffset_RightArm, _Radius_RightArm, _Scale_RightArm2, _Strength_RightArm, _Softness_RightArm, _FeatherDistance_RightArm, _DistortionTex_RightArm, _DistortionTex_RightArm_TexelSize, _DepthFactor_RightArm, 0.0);
                }
                if (_Enable_LeftLeg)
                {
                    uv = ApplyBulge(uv, _Center_LeftLeg1, _Angle_LeftLeg1, _CenterOffset_LeftLeg, _Radius_LeftLeg, _Scale_LeftLeg1, _Strength_LeftLeg, _Softness_LeftLeg, _FeatherDistance_LeftLeg, _DistortionTex_LeftLeg, _DistortionTex_LeftLeg_TexelSize, _DepthFactor_LeftLeg, _Asymmetry_LeftLeg);
                    uv = ApplyBulge(uv, _Center_LeftLeg2, _Angle_LeftLeg2, _CenterOffset_LeftLeg, _Radius_LeftLeg, _Scale_LeftLeg2, _Strength_LeftLeg, _Softness_LeftLeg, _FeatherDistance_LeftLeg, _DistortionTex_LeftLeg, _DistortionTex_LeftLeg_TexelSize, _DepthFactor_LeftLeg, _Asymmetry_LeftLeg);
                }
                if (_Enable_RightLeg)
                {
                    uv = ApplyBulge(uv, _Center_RightLeg1, _Angle_RightLeg1, _CenterOffset_RightLeg, _Radius_RightLeg, _Scale_RightLeg1, _Strength_RightLeg, _Softness_RightLeg, _FeatherDistance_RightLeg, _DistortionTex_RightLeg, _DistortionTex_RightLeg_TexelSize, _DepthFactor_RightLeg, _Asymmetry_RightLeg);
                    uv = ApplyBulge(uv, _Center_RightLeg2, _Angle_RightLeg2, _CenterOffset_RightLeg, _Radius_RightLeg, _Scale_RightLeg2, _Strength_RightLeg, _Softness_RightLeg, _FeatherDistance_RightLeg, _DistortionTex_RightLeg, _DistortionTex_RightLeg_TexelSize, _DepthFactor_RightLeg, _Asymmetry_RightLeg);
                }
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}