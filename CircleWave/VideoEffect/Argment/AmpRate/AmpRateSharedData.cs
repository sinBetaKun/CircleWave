using YukkuriMovieMaker.Commons;

namespace CircleWave.VideoEffect.Argment.AmpRate
{
    internal class AmpRateSharedData
    {
        public Animation AmpRate { get; } = new(10, 0, 100);

        public AmpRateSharedData()
        {
        }

        public AmpRateSharedData(IAmpRateParameter parameter)
        {
            AmpRate.CopyFrom(parameter.AmpRate);
        }

        public void CopyTo(IAmpRateParameter parameter)
        {
            parameter.AmpRate.CopyFrom(AmpRate);
        }
    }
}
