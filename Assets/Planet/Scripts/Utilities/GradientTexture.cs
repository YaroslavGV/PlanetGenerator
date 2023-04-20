using System;
using System.Collections.Generic;
using UnityEngine;

public static class GradientTexture
{
    public static Texture2D GetTexture (this Gradient gradient, int width = 128, int height = 8)
    {
        if (width < 1 || height < 1)
            throw new Exception("width and height must be positive values");

        Texture2D texture = new Texture2D(width, height);
        texture.name = "Gradient Texture";
        for (int x = 0; x < width; x++)
        {
            float p = x/(float)width;
            Color color = gradient.Evaluate(p);
            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, color);
        }
        texture.Apply();
        return texture;
    }

    public static Texture2D GetTexture (this Gradient gradient, Vector2Int size) => gradient.GetTexture(size.x, size.y);

	public static string GetTextConstructor (this Gradient gradient, string name)
	{
		List<string> rows = new List<string>();
		rows.Add(name+" = new Gradient()");
		rows.Add("{");
		rows.Add("\tcolorKeys = new[]");
		rows.Add("\t{");
		for (int i = 0; i < gradient.colorKeys.Length; i++)
		{
			GradientColorKey key = gradient.colorKeys[i];
			Color color = key.color;
			float h, s, v;
			Color.RGBToHSV(color, out h, out s, out v);
			int hc = Mathf.RoundToInt(h*360f);
			float time = key.time;
			string c = i < gradient.colorKeys.Length-1 ? "," : "";
			rows.Add(string.Format("\t\tnew GradientColorKey(Color.HSVToRGB({0}/360f, {1}, {2}), {3}){4}",
				hc, FloatFormat(s), FloatFormat(v), FloatFormat(time), c));
		}
		rows.Add("\t},");
		rows.Add("\talphaKeys = new[]");
		rows.Add("\t{");
		for (int i = 0; i < gradient.alphaKeys.Length; i++)
		{
			GradientAlphaKey key = gradient.alphaKeys[i];
			float alpha = key.alpha;
			float time = key.time;
			string c = i < gradient.alphaKeys.Length-1 ? "," : "";
			rows.Add(string.Format("\t\tnew GradientAlphaKey({0}, {1}){2}", FloatFormat(alpha), FloatFormat(time), c));
		}
		rows.Add("\t}");
		rows.Add("};");
		return string.Join(Environment.NewLine, rows);
	}

	private static string FloatFormat (float value)
    {
		bool notInt = value%1 != 0;
		if (notInt)
			value = Mathf.RoundToInt(value*100f)/100f;
		return value.ToString().Replace(",", ".")+(notInt ? "f" : "");
	}
}
