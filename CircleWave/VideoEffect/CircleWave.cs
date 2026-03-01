using CircleWave.VideoEffect.Parameter;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace CircleWave.VideoEffect
{
    [VideoEffect("波打ち（極座標）", ["アニメーション"], ["渦", "spiral", "プラグイン", "plugin"])]
    internal class CircleWave : VideoEffectBase
    {
        public override string Label => "波打ち（極座標）";

        [Display(Name = "波の種類")]
        [EnumComboBox]
        public WaveType WaveType { get => _waveType; set => Set(ref _waveType, value); }
        private WaveType _waveType = WaveType.Circle;

        [Display(AutoGenerateField = true)]
        public CircleWaveArgBase CircleWaveArg { get => _circleWaveArg; set => Set(ref _circleWaveArg, value); }
        private CircleWaveArgBase _circleWaveArg = new CircleWaveParameter();

        public Animation Amp
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.AmpAngle.CopyFrom(value);
            }
        }

        public Animation Wlen
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Wlen.CopyFrom(value);
            }
        }

        public Animation Phase
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Phase.CopyFrom(value);
            }
        }

        public Animation Speed
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Speed.CopyFrom(value);
            }
        }

        public Animation Offset
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Offset.CopyFrom(value);
            }
        }

        public Animation Strd
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Strd.CopyFrom(value);
            }
        }

        public Animation Cmpl
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Cmpl.CopyFrom(value);
            }
        }

        public Animation X
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.X.CopyFrom(value);
            }
        }

        public Animation Y
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Y.CopyFrom(value);
            }
        }

        public bool Mode
        {
            set
            {
                if (CircleWaveArg is CircleWaveParameter cp)
                    cp.Mode = value;
            }
        }

        public override ValueTask EndEditAsync()
        {
            CircleWaveArg = WaveType.Convert(CircleWaveArg);
            return base.EndEditAsync();
        }

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
            => [];

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new CircleWaveProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [CircleWaveArg];
    }
}
