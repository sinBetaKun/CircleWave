using CircleWave.VideoEffect.Argment.AmpRate;
using CircleWave.VideoEffect.Argment.XY;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace CircleWave.VideoEffect.Parameter
{
    internal class RadialWaveParameter : CircleWaveArgBase, IAmpRateParameter, IXYParameter
    {
        [Display(Name = "振幅", Description = "振幅")]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation AmpRate { get; } = new(10, 0, 100);

        [Display(Name = "波の個数", Description = "波の個数")]
        [AnimationSlider("F0", "", 0, 20)]
        public Animation Count { get; } = new(10, 0, 500);
        
        [Display(Name = "回転角", Description = "回転角")]
        [AnimationSlider("F0", "°", 0, 360)]
        public Animation Angle { get; } = new(0, -36000, 36000);

        [Display(Name = "X", Description = "中心のX座標")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new Animation(0, -100000, 100000);

        [Display(Name = "Y", Description = "中心のY座標")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new Animation(0, -100000, 100000);

        public RadialWaveParameter()
        {
        }

        public RadialWaveParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CircleWaveArgBase? origin)
        {
            if (origin is IAmpRateParameter ampParameter)
                AmpRate.CopyFrom(ampParameter.AmpRate);
            if (origin is IXYParameter xyParameter)
            {
                X.CopyFrom(xyParameter.X);
                Y.CopyFrom(xyParameter.Y);
            }
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [AmpRate, Count, Angle, X, Y];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AmpRateSharedData(this));
            store.Save(new XYSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AmpRateSharedData>() is AmpRateSharedData ampSharedData)
                AmpRate.CopyFrom(ampSharedData.AmpRate);
            if (store.Load<XYSharedData>() is XYSharedData xySharedData)
            {
                X.CopyFrom(xySharedData.X);
                Y.CopyFrom(xySharedData.Y);
            }
        }
    }
}
