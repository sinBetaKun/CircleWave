using CircleWave.VideoEffect.Parameter;
using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CircleWave.VideoEffect
{
    internal class CircleWaveProcessor : IVideoEffectProcessor
    {
        private readonly CircleWave item;
        private bool isFirst = true;
        private bool isEnable = false;
        private double amp, wlen, phase, speed, offset, strd, cmpl, x, y, time, count, angle;
        private bool mode;
        private WaveType waveType;

        private readonly CircleWaveCustomEffect? effect1;
        private readonly RadialWaveCustomEffect? effect2;
        private readonly AffineTransform2D? trans;
        private ID2D1Image? input;

        public ID2D1Image Output => trans?.Output ?? throw new NullReferenceException();

        public CircleWaveProcessor(IGraphicsDevicesAndContext devices, CircleWave item)
        {
            this.item = item;
            effect1 = new CircleWaveCustomEffect(devices);
            effect2 = new RadialWaveCustomEffect(devices);
            trans = new AffineTransform2D(devices.DeviceContext);

            if (!effect1.IsEnabled)
            {
                effect1.Dispose();
                effect1 = null;
            }
            else if (item.WaveType == WaveType.Circle)
            {
                trans.SetInput(0, effect1.Output, true);
            }

            if (!effect2.IsEnabled)
            {
                effect2.Dispose();
                effect2 = null;
            }
            else if (item.WaveType == WaveType.Radial)
            {
                trans.SetInput(0, effect2.Output, true);
            }
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;

            switch (waveType)
            {
                case WaveType.Circle:
                    effect1?.SetInput(0, input, true);
                    effect2?.SetInput(0, null, true);
                    trans?.SetInput(0, effect1?.Output, true);
                    break;

                case WaveType.Radial:
                    effect1?.SetInput(0, null, true);
                    effect2?.SetInput(0, input, true);
                    trans?.SetInput(0, effect2?.Output, true);
                    break;
            }
        }

        public void ClearInput()
        {
            trans?.SetInput(0, null, true);
            effect1?.SetInput(0, null, true);
            effect2?.SetInput(0, null, true);
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            bool typeOld = false;

            if (item.WaveType != waveType)
            {
                typeOld = true;
                waveType = item.WaveType;

                switch (waveType)
                {
                    case WaveType.Circle:
                        effect1?.SetInput(0, input, true);
                        effect2?.SetInput(0, null, true);
                        trans?.SetInput(0, effect1?.Output, true);
                        break;

                    case WaveType.Radial:
                        effect1?.SetInput(0, null, true);
                        effect2?.SetInput(0, input, true);
                        trans?.SetInput(0, effect2?.Output, true);
                        break;
                }
            }

            int frame = effectDescription.ItemPosition.Frame;
            int length = effectDescription.ItemDuration.Frame;
            int fps = effectDescription.FPS;
            FrameAndLength fl = new(frame, length);

            if (item.CircleWaveArg is CircleWaveParameter cp)
            {
                if (effect1 is null)
                    return effectDescription.DrawDescription;

                double ampAngle = fl.GetValue(cp.AmpAngle, fps);
                double wlen = fl.GetValue(cp.Wlen, fps);
                double phase = fl.GetValue(cp.Phase, fps);
                double speed = fl.GetValue(cp.Speed, fps);
                double offset = fl.GetValue(cp.Offset, fps);
                double strd = fl.GetValue(cp.Strd, fps);
                double cmpl = fl.GetValue(cp.Cmpl, fps);
                double x1 = fl.GetValue(cp.X, fps);
                double y1 = fl.GetValue(cp.Y, fps);
                bool mode = cp.Mode;
                double time = (double)frame / fps;

                if ((float)phase == 0)
                {
                    isEnable = false;
                    return effectDescription.DrawDescription;
                }

                if (isFirst
                    || typeOld
                    || amp != ampAngle
                    || this.wlen != wlen
                    || this.phase != phase
                    || this.speed != speed
                    || this.offset != offset
                    || this.strd != strd
                    || this.cmpl != cmpl
                    || x != x1
                    || y != y1
                    || this.mode != mode
                    || this.time != time)
                {
                    isEnable = true;

                    if (!isEnable)
                    {
                        isEnable = true;
                        trans?.SetInput(0, effect1.Output, true);
                    }

                    effect1.Amp = (float)ampAngle;
                    effect1.Wlen = (float)wlen;
                    effect1.Phase = (float)(phase == 0 ? 1 : phase);
                    effect1.Offset = (float)offset;
                    effect1.Strd = (float)strd;
                    effect1.Cmpl = (float)cmpl;
                    effect1.X = (float)x1;
                    effect1.Y = (float)y1;
                    effect1.Mode = mode;
                    effect1.Time = (float)(phase == 0 ? 0 : time * speed / 100);

                    amp = ampAngle;
                    this.wlen = wlen;
                    this.phase = phase;
                    this.speed = speed;
                    this.offset = offset;
                    this.strd = strd;
                    this.cmpl = cmpl;
                    x = x1;
                    y = y1;
                    this.mode = mode;
                    this.time = time;

                    isFirst = false;
                }

                return effectDescription.DrawDescription with
                {
                    Controllers = [
                        new VideoEffectController(item, [
                                new ControllerPoint(
                                    new Vector3((float)x1, (float)y1, 0),
                                    (a) => {
                                        cp.X.AddToEachValues(a.Delta.X);
                                        cp.Y.AddToEachValues(a.Delta.Y);
                                    })])]
                };
            }
            else if (item.CircleWaveArg is RadialWaveParameter rp)
            {
                if (effect2 is null)
                    return effectDescription.DrawDescription;

                double ampRate = fl.GetValue(rp.AmpRate, fps);
                double count = fl.GetValue(rp.Count, fps);
                double angle = fl.GetValue(rp.Angle, fps);
                double x2 = fl.GetValue(rp.X, fps);
                double y2 = fl.GetValue(rp.Y, fps);

                if (count == 0)
                {
                    if (isEnable)
                    {
                        isEnable = false;
                        trans?.SetInput(0, input, true);
                    }

                    return effectDescription.DrawDescription;
                }

                if (isFirst
                    || typeOld
                    || amp != ampRate
                    || this.count != count
                    || this.angle != angle
                    || x != x2
                    || y != y2)
                {
                    isEnable = true;

                    if (!isEnable)
                    {
                        isEnable = true;
                        trans?.SetInput(0, effect2.Output, true);
                    }

                    effect2.Amp = (float)ampRate;
                    effect2.Count = (int)count;
                    effect2.Angle = (float)angle;
                    effect2.X = (float)x2;
                    effect2.Y = (float)y2;

                    amp = ampRate;
                    this.count = count;
                    this.angle = angle;
                    x = x2;
                    y = y2;

                    isFirst = false;
                }

                return effectDescription.DrawDescription with
                {
                    Controllers = [
                        new VideoEffectController(item, [
                                new ControllerPoint(
                                    new Vector3((float)x2, (float)y2, 0),
                                    (a) => {
                                        rp.X.AddToEachValues(a.Delta.X);
                                        rp.Y.AddToEachValues(a.Delta.Y);
                                    })])]
                };
            }

            throw new NotSupportedException();

        }

        public void Dispose()
        {
            trans?.SetInput(0, null, true);
            trans?.Dispose();
            effect1?.SetInput(0, null, true);
            effect1?.Dispose();
            effect2?.SetInput(0, null, true);
            effect2?.Dispose();
        }
    }
}
