using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace CircleWave.VideoEffect
{
    [VideoEffect("波打ち（極座標）", [], [])]
    internal class CircleWave : VideoEffectBase
    {
        public override string Label => "波打ち（極座標）";

        [Display(Name = "振幅", Description = "波打つ角度")]
        [AnimationSlider("F1", "°", 0, 360)]
        public Animation Amp { get; } = new Animation(30, 0, 360);

        [Display(Name = "波長", Description = "波長")]
        [AnimationSlider("F1", "px", 0, 500)]
        public Animation Wlen { get; } = new Animation(100, 0, 100000);

        [Display(Name = "周期", Description = "周期")]
        [AnimationSlider("F2", "秒", -1, 1)]
        public Animation Phase { get; } = new Animation(0.5, -100000, 100000);

        [Display(Name = "固定部の半径", Description = "湾曲させない部分の半径")]
        [AnimationSlider("F1", "px", 0, 500)]
        public Animation Strd { get; } = new Animation(0, 0, 100000);

        [Display(Name = "X", Description = "中心のX座標")]
        [AnimationSlider("F1", "px", 0, 360)]
        public Animation X { get; } = new Animation(0, 0, 100000);

        [Display(Name = "Y", Description = "中心のY座標")]
        [AnimationSlider("F1", "px", 0, 360)]
        public Animation Y { get; } = new Animation(0, 0, 100000);

        [Display(Name = "固定部の回転", Description = "固定部の回転")]
        [ToggleSlider]
        public bool Mode { get => mode; set => Set(ref mode, value); }
        private bool mode = false;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
            => [];

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new CircleWaveProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Amp, Wlen, Phase, Strd, X, Y];
    }
}
