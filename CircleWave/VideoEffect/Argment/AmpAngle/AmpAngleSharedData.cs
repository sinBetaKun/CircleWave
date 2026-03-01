using YukkuriMovieMaker.Commons;

namespace CircleWave.VideoEffect.Argment.AmpAngle
{
    internal class AmpAngleSharedData
    {
        public Animation AmpAngle { get; } = new(10, -360, 360);

        public AmpAngleSharedData()
        {
        }

        public AmpAngleSharedData(IAmpAngleParameter parameter)
        {
            AmpAngle.CopyFrom(parameter.AmpAngle);
        }

        public void CopyTo(IAmpAngleParameter parameter)
        {
            parameter.AmpAngle.CopyFrom(AmpAngle);
        }
    }
}
