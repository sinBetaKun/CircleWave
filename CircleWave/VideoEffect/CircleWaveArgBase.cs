using YukkuriMovieMaker.Project;

namespace CircleWave.VideoEffect
{
    internal abstract class CircleWaveArgBase : SharedParameterBase
    {
        public CircleWaveArgBase()
        {
        }

        public CircleWaveArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyFrom(CircleWaveArgBase? origin);
    }
}
