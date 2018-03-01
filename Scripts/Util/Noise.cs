using Godot;
using System;
using System.Collections.Generic;

public class Noise
{
    private Random random = new Random();
    private Dictionary<int, Vector2> randGrads = new Dictionary<int, Vector2>();

    private static int IntToNat(int z)
    {
        if (z < 0)
            return -2 * z - 1;
        return 2 * z;
    }

    private static int NatToInt(int n)
    {
        if (n % 2 == 0)
            return n / 2;
        return -(n + 1) / 2;
    }

    private static int CantorPairNats(int x, int y)
    {
        return (int) (0.5 * (x + y) * (x + y + 1) + y);
    }

    private static int[] DeCantorPair(int z)
    {
        int t = (int) (Math.Floor((Math.Sqrt(8 * z + 1) - 1) / 2));
        int x = t * (t + 3) / 2 - z;
        int y = z - t * (t + 1) / 2;
        return new int[] {x,y};
    }

    private Vector2 GetGrad(int x, int y)
    {
        int nx = IntToNat(x);
        int ny = IntToNat(y);

        int z = CantorPairNats(nx,ny);

        if(randGrads.ContainsKey(z))
            return randGrads[z];

        float xx = (float) random.NextDouble() * 2 - 1;
        float yy = (float) random.NextDouble() * 2 - 1;
        Vector2 v = new Vector2(xx, yy);
        v = v.Normalized();
        //randGrads.Add(z, v);
        randGrads[z] = v;
        return v;
    }

    private int FastFloor(float x)
	{
		return x > 0 ? (int) x : (int) x - 1;
	}

	private float Weigh(float x)
	{
		return 6 * x * x * x * x * x - 15 * x * x * x * x + 10 * x * x * x;
	}

	private float Lerp(float w, float a, float b)
	{
		return a + w * (b - a);
	}

    public float Sample(float x, float y)
	{
		int gx0 = FastFloor(x);
		int gy0 = FastFloor(y);
		int gx1 = gx0 + 1;
		int gy1 = gy0 + 1;

		float dx0 = x - gx0;
		float dy0 = y - gy0;
		float dx1 = x - gx1;
		float dy1 = y - gy1;

		Vector2 vals0 = GetGrad(gx0, gy0);
		Vector2 vals1 = GetGrad(gx0, gy1);
		Vector2 vals2 = GetGrad(gx1, gy0);
		Vector2 vals3 = GetGrad(gx1, gy1);

		float w0 = (new Vector2(dx0,dy0)).Dot(vals0);
		float w1 = (new Vector2(dx0,dy1)).Dot(vals1);
		float w2 = (new Vector2(dx1,dy0)).Dot(vals2);
		float w3 = (new Vector2(dx1,dy1)).Dot(vals3);

		float sx = Weigh(dx0);
		float sy = Weigh(dy0);

		float a = Lerp(sy, w0, w1);
		float b = Lerp(sy, w2, w3);
		float h = Lerp(sx, a, b);

        //Failsafe clipping
		if (h < -1.0f)
			h = -1.0f;
		if (h > 1.0f)
			h = 1.0f;

		return h;
	}
}