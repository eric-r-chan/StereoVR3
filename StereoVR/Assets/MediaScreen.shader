﻿// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


/// Used to render Media Screens for the Media App Template
/// Supports Mono, LeftRight, and TopBottom stereo modes.
Shader "MediaAppTemplate/MediaScreen" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _LeftRight ("Render Stereo Left/Right", Int) = 0
    _TopBottom ("Render Stereo Top/Bottom", Int) = 0
  }

  SubShader {
  Tags { "RenderType"="Opaque" }
  LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "Assets/GoogleVR/Shaders/GvrUnityCompatibility.cginc"
      #include "UnityCG.cginc"

      struct v2f {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _LeftRight;
      float _TopBottom;
      float _Inside;
      float _InverseX;
      float _XCoeff;

      v2f vert(appdata_base v) {
        v2f o;
        o.vertex = GvrUnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
        o.uv.x *= 1.0 - (0.5 * _LeftRight);
        o.uv.y *= 1.0 - (0.5 * _TopBottom);
        o.uv.y += 0.5f * _TopBottom * (1.0 - unity_StereoEyeIndex);
        o.uv.x += 0.5f * _LeftRight * unity_StereoEyeIndex;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv);
        return col;
      }
      ENDCG
    }
  }
}