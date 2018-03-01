using Godot;
using System;

public class OctaveNoise
{
    private int octaves;
    private Noise[] noises;

    public OctaveNoise(int oct)
    {
        octaves = oct;
        noises = new Noise[octaves];
        for (int i = 0; i < octaves; i++)
            noises[i] = new Noise();
    }

    public float Sample(float x, float y)
    {
        float amp = 1;
        int freq = 1;

        float totalSample = 0.0f;

        for (int i = 0; i < octaves; i++)
        {
            float xs = x * freq;
            float ys = y * freq;

            float sample = noises[i].Sample(xs,ys) * amp;

            totalSample += sample;
            
            amp /= 2;
            freq *= 2;
        }

        float ret = totalSample / 2;

        return ret;
    }
}