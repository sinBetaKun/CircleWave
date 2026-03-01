using CircleWave.VideoEffect.Parameter;

namespace CircleWave.VideoEffect
{
    internal static class WaveTypeEx
    {
        public static CircleWaveArgBase Convert(this WaveType type, CircleWaveArgBase current)
        {
            var store = current.GetSharedData();
            CircleWaveArgBase param = type switch
            {
                WaveType.Circle => new CircleWaveParameter(store),
                WaveType.Radial => new RadialWaveParameter(store),
                _ => throw new NotSupportedException()
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
