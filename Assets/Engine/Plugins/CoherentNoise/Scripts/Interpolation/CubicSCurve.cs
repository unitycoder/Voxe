namespace Assets.Engine.Plugins.CoherentNoise.Scripts.Interpolation
{
	internal class CubicSCurve: SCurve
	{
		#region Overrides of Interpolator

		public override float Interpolate(float t)
		{
			return t*t*(3f - 2f*t);
		}

		#endregion
	}
}