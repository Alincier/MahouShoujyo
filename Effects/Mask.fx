// 1. 创建蒙版 Shader（.fx 文件）
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float4 uShaderSpecificData;

// 像素着色器函数
float4 PixelShaderMask(float2 texCoord : TEXCOORD0) : COLOR0
{
    // 采样内容纹理    
    float4 content = tex2D(uImage0, texCoord);
    //uScreenResolution是纹理大小；uShaderSpecificData是蒙版起始点和蒙版大小；
    float texWidth = uScreenResolution.r;
    float texHeight = uScreenResolution.g;
    float maskX = uShaderSpecificData.r; 
    float maskY = uShaderSpecificData.g;
    float maskWidth = uShaderSpecificData.b;
    float maskHeight = uShaderSpecificData.a;
    //当前采样点坐标位置
    float pX = texWidth * texCoord.r;
    float pY = texHeight * texCoord.g;
    //确认蒙版采样点位置（超出范围则采样为float4(0,0,0,0);
    float4 mask;
    if (pX > maskX && pX < maskX + maskWidth && pY > maskY && pY < maskY + maskHeight)
    {
        float2 maskCoord = float2((pX - maskX) / maskWidth, (pY-maskY) / maskHeight);
        mask = tex2D(uImage1, maskCoord);
    }
    else
        mask = float4(0, 0, 0, 0);
    // 采样蒙版纹理
        

    
    float4 addColor = float4(uColor.r, uColor.g, uColor.b, 1);
    content.a *= mask.r;
    // 使用蒙版的alpha通道作为裁剪依据
    return content * addColor; //content * step(0.5, mask.a);

}
float4 TextureShaderMask(float2 texCoord : TEXCOORD0) : COLOR0
{
    /*// 采样蒙版纹理
    float4 mask = tex2D(uImage1, texCoord);
    // 采样内容纹理
    float4 content = tex2D(uImage0, texCoord);
    float4 addColor = float4(uColor.r, uColor.g, uColor.b, 1);
    content.a *= mask.r;
    // 使用蒙版的alpha通道作为裁剪依据
    return content * addColor; //content * step(0.5, mask.a);*/

    // 采样内容纹理    
    float4 content = tex2D(uImage0, texCoord);
    //uScreenResolution是纹理大小；uShaderSpecificData是蒙版起始点和蒙版大小；
    float texWidth = uScreenResolution.r;
    float texHeight = uScreenResolution.g;
    float maskX = uShaderSpecificData.r;
    float maskY = uShaderSpecificData.g;
    float maskWidth = uShaderSpecificData.b;
    float maskHeight = uShaderSpecificData.a;
    //当前采样点坐标位置
    float pX = texWidth * texCoord.r;
    float pY = texHeight * texCoord.g;
    //确认蒙版采样点位置（超出范围则采样为float4(0,0,0,0);
    float4 mask;
    if (pX > maskX && pX < maskX + maskWidth && pY > maskY && pY < maskY + maskHeight)
    {
        float2 maskCoord = float2((pX - maskX) / maskWidth, (pY - maskY) / maskHeight);
        mask = tex2D(uImage1, maskCoord);
    }
    else
        mask = float4(0, 0, 0, 0);
    // 采样蒙版纹理
        

    
    float4 addColor = float4(uColor.r, uColor.g, uColor.b, 1);
    content.a *= mask.r;
    // 使用蒙版的alpha通道作为裁剪依据
    return content * addColor; //content * step(0.5, mask.a);
}

// 技术定义
technique MaskTechnique
{
    pass PixelShaderMask
    {
        PixelShader = compile ps_2_0 PixelShaderMask();
    }
    pass TextureShaderMask
    {
        PixelShader = compile ps_2_0 TextureShaderMask();
    }
}