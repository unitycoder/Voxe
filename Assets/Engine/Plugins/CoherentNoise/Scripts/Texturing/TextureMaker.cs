using System;
using UnityEngine;

namespace Assets.Engine.Plugins.CoherentNoise.Scripts.Texturing
{
		/// <summary>
		/// Use methods in this class to create Unity textures with noise generators. All textures are created using 2D noise (i.e, Z coordinate is always 0), and sample source noise in
		/// [0,1]x[0,1] area.
		/// </summary>
		public static class TextureMaker
		{
				/// <summary>
				/// Generic texture-building method. Creates a texture using a fuction that transforms float coordiantes (in the range [0,1]x[0,1]) into color
				/// </summary>
				///<param name="width">Texture width.</param>
				///<param name="height">Texture height</param>
				/// <param name="colorFunc">Function mapping coordinates to color</param>
				///<param name="format">Texture format to use</param>
				///<returns></returns>
				public static Texture Make (int width, int height, Func<float, float, Color> colorFunc, TextureFormat format)
				{
						Color[] cols = new Color[width * height];
						for (int ii = 0; ii < width; ii++) {
								for (int jj = 0; jj < height; jj++) {
										cols [ii + jj * width] = colorFunc ((float)ii / width, (float)jj / height);
								}
						}
						var res = new Texture2D (width, height, format, false);
						res.SetPixels (cols, 0);
						res.Apply ();

						return res;

				}

				///<summary>
				/// Creates a texture with only alpha channel.
				///</summary>
				///<param name="width">Texture width.</param>
				///<param name="height">Texture height</param>
				///<param name="noise">Noise source</param>
				///<returns></returns>
				public static Texture AlphaTexture (int width, int height, Generator noise)
				{
						return Make (width, height, (x,y) => new Color (0, 0, 0, noise.GetValue (x, y, 0) * 0.5f + 0.5f), TextureFormat.Alpha8);
				}
				///<summary>
				/// Creates a monochrome texture.
				///</summary>
				///<param name="width">Texture width.</param>
				///<param name="height">Texture height</param>
				///<param name="noise">Noise source</param>
				///<returns></returns>
				public static Texture MonochromeTexture (int width, int height, Generator noise)
				{
						return Make (width, height, (x, y) =>
						{
								var v = noise.GetValue (x, y, 0) * 0.5f + 0.5f;
								return new Color (v, v, v, 1);
						}, TextureFormat.RGB24);
				}

				///<summary>
				/// Creates a texture using ramp of colors. Noise value (clamped to [-1,1]) is mapped to one-dimensional ramp texture to obtain final color.
				/// As there are no 1-dimensional textures in Unity, Texture2D is used, that is sampled along its top line.
				///</summary>
				///<param name="width">Texture width.</param>
				///<param name="height">Texture height</param>
				///<param name="noise">Noise source</param>
				///<param name="ramp">Ramp texture</param>
				///<returns></returns>
				public static Texture RampTexture (int width, int height, Generator noise, Texture2D ramp)
				{
						Color[] rampCols = ramp.GetPixels (0, 0, ramp.width, 1);

						return Make (width, height, (x, y) =>
						{
								var v = noise.GetValue (x, y, 0) * 0.5f + 0.5f;
								return rampCols [(int)(Mathf.Clamp01 (v) * (ramp.width - 1))];
						}, TextureFormat.RGB24);
				}
		
				///<summary>
				/// Creates a texture to use as a bump map, taking height noise as input.
				///</summary>
				///<param name="width">Texture width.</param>
				///<param name="height">Texture height</param>
				///<param name="noise">heightmap  source</param>
				///<returns></returns>
				public static Texture BumpMap (int width, int height, Generator noise)
				{
						var res = new Texture2D (width, height, TextureFormat.RGB24, false);
						for (int mip = 0; mip < res.mipmapCount; mip++) {
								Color[] cols = new Color[width * height];
								for (int ii = 0; ii < width; ii++) {
										for (int jj = 0; jj < height; jj++) {
												var left = noise.GetValue ((ii - 0.5f) / width, (float)jj / height, 0);
												var right = noise.GetValue ((ii + 0.5f) / width, (float)jj / height, 0);
												var down = noise.GetValue ((float)ii / width, (jj - 0.5f) / height, 0);
												var up = noise.GetValue ((float)ii / width, (jj + 0.5f) / height, 0);
												Vector3 normal = new Vector3 (right - left, up - down, 1).normalized;
												cols [ii + jj * width] = new Color (normal.x, normal.y, normal.z);
										}
								}
								res.SetPixels (cols, mip);
								width >>= 1;
								height >>= 1;
						}
						res.Apply (false);

						return res;
				}

		}
}