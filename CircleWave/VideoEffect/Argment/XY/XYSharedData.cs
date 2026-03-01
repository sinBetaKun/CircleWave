using YukkuriMovieMaker.Commons;

namespace CircleWave.VideoEffect.Argment.XY
{
    internal class XYSharedData
    {
        public Animation X { get; } = new(0, -100000, 100000);
        public Animation Y { get; } = new(0, -100000, 100000);

        public XYSharedData()
        {
        }

        public XYSharedData(IXYParameter parameter)
        {
            X.CopyFrom(parameter.X);
            Y.CopyFrom(parameter.Y);
        }

        public void CopyTo(IXYParameter parameter)
        {
            parameter.X.CopyFrom(X);
            parameter.Y.CopyFrom(Y);
        }
    }
}
