#version 330
in vec2 fragTexCoord;
in vec4 fragColor;
out vec4 finalColor;
uniform sampler2D texture0;
uniform vec2 resolution;
uniform vec2 worldOrigin;
uniform float clock;
uniform float theme; // 0 forest, 1 dungeon, 2 village
uniform float strength;
uniform float hurt;
uniform vec4 pulse; // screen xy, age, intensity
uniform vec4 lights[8]; // screen xy, radius, color family (warm / cool)

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float noise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    return mix(mix(hash(i),hash(i+vec2(1,0)),f.x),
               mix(hash(i+vec2(0,1)),hash(i+vec2(1,1)),f.x),f.y);
}
vec3 bright(vec2 uv) {
    vec3 c = texture(texture0, clamp(uv, vec2(0.001), vec2(0.999))).rgb;
    return c * smoothstep(0.60, 0.95, max(c.r, max(c.g,c.b)));
}
void main() {
    vec2 pixel = vec2(fragTexCoord.x, 1.0-fragTexCoord.y)*resolution;
    vec2 world = pixel + worldOrigin;
    vec3 original = texture(texture0,fragTexCoord).rgb;
    vec3 color = original;
    float luminance = dot(color,vec3(0.2126,0.7152,0.0722));
    color = mix(vec3(luminance),color,theme==1.0 ? 0.83 : 0.88);
    // A small, thresholded bloom kernel leaves the base pixel-art sample sharp.
    vec3 bloom = vec3(0);
    for (int y=-1;y<=1;y++) for (int x=-1;x<=1;x++)
        bloom += bright(fragTexCoord+vec2(x,y)*3.0/resolution);
    color += bloom * 0.020;

    if (theme==1.0) {
        vec3 illumination=vec3(0.26,0.31,0.43);
        for(int i=0;i<8;i++) {
            float radius=lights[i].z;
            if(radius<=0.0) continue;
            float d=length(pixel-lights[i].xy)/radius;
            float falloff=exp(-d*d*2.2);
            float flicker=0.96+0.04*sin(clock*6.0+float(i)*4.1);
            vec3 tint=lights[i].w>0.5 ? vec3(0.25,0.85,0.90) : vec3(1.10,0.74,0.35);
            illumination+=tint*falloff*flicker;
            color+=tint*pow(falloff,8.0)*0.045;
        }
        color*=min(illumination,vec3(1.3));
    } else {
        // Moving canopy shadows and diagonal shafts, anchored to the world.
        float cloud=noise(world*0.003+vec2(clock*0.018,0));
        float diagonal=world.x+world.y*0.60;
        float rays=pow(0.5+0.5*sin(diagonal*0.012+sin(world.y*0.003)+clock*0.12),12.0);
        rays*=0.45+0.55*noise(world*0.004+clock*0.015);
        color*=mix(0.69,1.07,smoothstep(0.15,0.85,cloud));
        vec3 sunlight=theme==2.0 ? vec3(1.0,0.43,0.15) : vec3(1.0,0.83,0.43);
        color*=theme==2.0 ? vec3(1.12,0.91,0.80) : vec3(1.04,1.00,0.86);
        color+=sunlight*rays*(theme==2.0 ? 0.17 : 0.14);
    }
    // Slow ground mist, with gaps so silhouettes and attack cues remain legible.
    float mist=noise(world*0.007+vec2(clock*0.022,-clock*0.008));
    mist*=noise(world*0.002-vec2(clock*0.01,0));
    vec3 fog=theme==1.0 ? vec3(0.21,0.32,0.43) : theme==2.0 ? vec3(0.64,0.38,0.25) : vec3(0.44,0.55,0.41);
    color=mix(color,fog,mist*0.22);
    // Sparse drifting motes (pollen / dungeon dust / warm village embers).
    vec2 motes=world+vec2(clock*7.0+sin(clock*0.5)*8.0,-clock*9.0);
    vec2 cell=floor(motes/90.0);
    vec2 local=mod(motes,90.0);
    vec2 center=vec2(hash(cell),hash(cell+11.0))*70.0+10.0;
    float speck=1.0-smoothstep(0.8,2.7,length(local-center));
    float twinkle=0.3+0.7*pow(0.5+0.5*sin(clock*1.4+hash(cell)*30.0),2.0);
    color+=vec3(0.85,0.70,0.34)*speck*twinkle*step(0.4,hash(cell+3.0));
    // Parry and wolf hits send a short luminous ring through the world only.
    float ring=exp(-pow((length(pixel-pulse.xy)-pulse.z*360.0)/9.0,2.0));
    color+=vec3(0.35,0.85,0.75)*ring*pulse.w*max(0.0,1.0-pulse.z/0.65)*0.45;
    vec2 edge=(pixel/resolution-0.5)*vec2(1.0,0.85);
    float vignette=smoothstep(0.25,0.68,length(edge));
    color*=1.0-vignette*0.24;
    color=mix(color,vec3(0.65,0.08,0.05),hurt*vignette*0.38);
    finalColor=vec4(mix(original,clamp(color,0.0,1.0),strength),1.0)*fragColor;
}
