#version 420 core
in vec2 texCoords;

uniform sampler2D modelTexture;
uniform bool isTextured;
uniform vec4 color;

out vec4 out_Color;

void main(void)
{
    vec4 textureColor4 = texture(modelTexture, texCoords);
    if (textureColor4.a < 0.05f) discard;
    out_Color = isTextured ? color * textureColor4 : color;
    
}