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
        private double amp, wlen, phase, strd, x, y, time;
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

            double amp = item.Amp.GetValue(frame, length, fps);
            double wlen = item.Amp.GetValue(frame, length, fps);
            double phase = item.Amp.GetValue(frame, length, fps);
            double strd = item.Amp.GetValue(frame, length, fps);
            double x = item.Amp.GetValue(frame, length, fps);
            double y = item.Amp.GetValue(frame, length, fps);
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
                effect.Strd = (float)strd;
                effect.X = (float)x;
                effect.Y = (float)y;
                effect.Mode = mode;
                effect.Time = (float)time;

                this.amp = amp;
                this.wlen = wlen;
                this.phase = phase;
                this.strd = strd;
                this.x = x;
                this.y = y;
                this.mode = mode;
                this.time = time;

                isFirst = false;
            }

            return effectDescription.DrawDescription;
        }

        public void Dispose()
        {
            output?.Dispose();
            effect?.SetInput(0, null, true);
            effect?.Dispose();
        }
    }
}
