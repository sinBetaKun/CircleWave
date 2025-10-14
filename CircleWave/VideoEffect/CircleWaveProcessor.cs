using System.Numerics;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CircleWave.VideoEffect
{
    internal class CircleWaveProcessor : IVideoEffectProcessor
    {
        private readonly CircleWave item;
        private bool isFirst = true;
        private bool isEnable = false;
        private double amp, wlen, phase, offset, strd, x, y, time;
        private bool mode;

        private readonly CircleWaveCustomEffect? effect;
        private readonly ID2D1Image? output;
        private ID2D1Image? input;

        public ID2D1Image Output => (isEnable ? output : input) ?? input ?? throw new NullReferenceException();

        public CircleWaveProcessor(IGraphicsDevicesAndContext devices, CircleWave item)
        {
            this.item = item;
            effect = new CircleWaveCustomEffect(devices);

            if (!effect.IsEnabled)
            {
                effect.Dispose();
                effect = null;
            }
            else
            {
                output = effect.Output;
            }
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
            effect?.SetInput(0, input, true);
        }

        public void ClearInput()
        {
            effect?.SetInput(0, null, true);
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            if (effect is null)
                return effectDescription.DrawDescription;

            int frame = effectDescription.ItemPosition.Frame;
            int length = effectDescription.ItemDuration.Frame;
            int fps = effectDescription.FPS;
            FrameAndLength fl = new(frame, length);

            double amp = fl.GetValue(item.Amp, fps);
            double wlen = fl.GetValue(item.Wlen, fps);
            double phase = fl.GetValue(item.Phase, fps);
            double offset = fl.GetValue(item.Offset, fps);
            double strd = fl.GetValue(item.Strd, fps);
            double x = fl.GetValue(item.X, fps);
            double y = fl.GetValue(item.Y, fps);
            bool mode = item.Mode;
            double time = (double)frame / fps;

            if ((float)phase == 0)
            {
                isEnable = false;
                return effectDescription.DrawDescription;
            }

            if (isFirst
                || this.amp != amp
                || this.wlen != wlen
                || this.phase != phase
                || this.offset != offset
                || this.strd != strd
                || this.x != x
                || this.y != y
                || this.mode != mode
                || this.time != time)
            {
                isEnable = true;

                effect.Amp = (float)amp;
                effect.Wlen = (float)wlen;
                effect.Phase = (float)phase;
                effect.Offset = (float)offset;
                effect.Strd = (float)strd;
                effect.X = (float)x;
                effect.Y = (float)y;
                effect.Mode = mode;
                effect.Time = (float)time;

                this.amp = amp;
                this.wlen = wlen;
                this.phase = phase;
                this.offset = offset;
                this.strd = strd;
                this.x = x;
                this.y = y;
                this.mode = mode;
                this.time = time;

                isFirst = false;
            }

            return effectDescription.DrawDescription with
            {
                Controllers = [
                    new VideoEffectController(item, [
                        new ControllerPoint(
                            new Vector3((float)x, (float)y, 0),
                            (a) => {
                                item.X.AddToEachValues(a.Delta.X);
                                item.Y.AddToEachValues(a.Delta.Y);
                            })])]
            };
        }

        public void Dispose()
        {
            output?.Dispose();
            effect?.SetInput(0, null, true);
            effect?.Dispose();
        }
    }
}
