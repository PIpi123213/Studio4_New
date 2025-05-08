Shader "Unlit/Unlit Transparent Color" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
    Fog {Mode Off}
      Stencil {
            Ref 1
            Comp notEqual
            Pass keep
        }


    ZTest Always
    
    //ZWrite Off        // ��д����Ȼ���
    //ZTest LEqual      // �����������

    Blend SrcAlpha OneMinusSrcAlpha
    Color [_Color]

    Pass {}
}
}
