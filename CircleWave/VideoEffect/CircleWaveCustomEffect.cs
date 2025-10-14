using System.Numerics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CircleWave.VideoEffect
{
    internal class CircleWaveCustomEffect : D2D1CustomShaderEffectBase
    {
        public float Amp
        {
            set => SetValue((int)EffectImpl.Properties.Amp, value);
            get => GetFloatValue((int)EffectImpl.Properties.Amp);
        }
        
        public float Wlen
        {
            set => SetValue((int)EffectImpl.Properties.Wlen, value);
            get => GetFloatValue((int)EffectImpl.Properties.Wlen);
        }
        
        public float Phase
        {
            set => SetValue((int)EffectImpl.Properties.Phase, value);
            get => GetFloatValue((int)EffectImpl.Properties.Phase);
        }
        
        public float Offset
        {
            set => SetValue((int)EffectImpl.Properties.Offset, value);
            get => GetFloatValue((int)EffectImpl.Properties.Offset);
        }
        
        public float Strd
        {
            set => SetValue((int)EffectImpl.Properties.Strd, value);
            get => GetFloatValue((int)EffectImpl.Properties.Strd);
        }
        
        public float Cmpl
        {
            set => SetValue((int)EffectImpl.Properties.Cmpl, value);
            get => GetFloatValue((int)EffectImpl.Properties.Cmpl);
        }
        
        public float X
        {
            set => SetValue((int)EffectImpl.Properties.X, value);
            get => GetFloatValue((int)EffectImpl.Properties.X);
        }
        
        public float Y
        {
            set => SetValue((int)EffectImpl.Properties.Y, value);
            get => GetFloatValue((int)EffectImpl.Properties.Y);
        }
        
        public bool Mode
        {
            set => SetValue((int)EffectImpl.Properties.Mode, value);
            get => GetBoolValue((int)EffectImpl.Properties.Mode);
        }
        
        public float Time
        {
            set => SetValue((int)EffectImpl.Properties.Time, value);
            get => GetFloatValue((int)EffectImpl.Properties.Time);
        }

        public CircleWaveCustomEffect(IGraphicsDevicesAndContext devices) : base(Create<EffectImpl>(devices))
        {   
        }

        [CustomEffect(1)]
        class EffectImpl : D2D1CustomShaderEffectImplBase<EffectImpl>
        {
            ConstantBuffer constantBuffer;

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Amp)]
            public float Amp
            {
                get => constantBuffer.Amp;
                set
                {
                    constantBuffer.Amp = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Wlen)]
            public float Wlen
            {
                get => constantBuffer.Wlen;
                set
                {
                    constantBuffer.Wlen = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Phase)]
            public float Phase
            {
                get => constantBuffer.Phase;
                set
                {
                    constantBuffer.Phase = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Offset)]
            public float Offset
            {
                get => constantBuffer.Offset;
                set
                {
                    constantBuffer.Offset = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Strd)]
            public float Strd
            {
                get => constantBuffer.Strd;
                set
                {
                    constantBuffer.Strd = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Cmpl)]
            public float Cmpl
            {
                get => constantBuffer.Cmpl;
                set
                {
                    constantBuffer.Cmpl = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.X)]
            public float X
            {
                get => constantBuffer.X;
                set
                {
                    constantBuffer.X = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Y)]
            public float Y
            {
                get => constantBuffer.Y;
                set
                {
                    constantBuffer.Y = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Bool, (int)Properties.Mode)]
            public bool Mode
            {
                get => constantBuffer.Mode;
                set
                {
                    constantBuffer.Mode = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Time)]
            public float Time
            {
                get => constantBuffer.Time;
                set
                {
                    constantBuffer.Time = value;
                    UpdateConstants();
                }
            }

            public EffectImpl() : base(ShaderResourceLoader.GetShaderResource("CircleWave.cso")/*ここでシェーダーのbyte列を渡す*/)
            {
            }

            protected override void UpdateConstants()
            {
                drawInformation?.SetPixelShaderConstantBuffer(constantBuffer);
            }

            /// <summary>
            /// 入力画像の範囲から出力画像の範囲を計算する
            /// 例:
            /// 画像に対して10pxの縁取りエフェクトを掛ける場合、outputRectをinputRectsの範囲から10px大きくする
            /// 画像に対して10pxのモザイクエフェクトをかける場合、出力範囲は変わらないのでinputRects[0]をそのままoutputRectに設定する
            /// </summary>
            /// <param name="inputRects">入力画像の範囲。inputの数だけ渡される。最適化のため、入力画像の範囲がそのまま渡されるわけではなく、分割されることもある。</param>
            /// <param name="inputOpaqueSubRects">入力画像の不透明な部分の範囲。最適化のため、入力画像の範囲がそのまま渡されるわけではなく、分割されることもある。</param>
            /// <param name="outputRect">入力画像をもとに計算した出力画像の範囲。</param>
            /// <param name="outputOpaqueSubRect">入力画像を元に計算した出力画像の不透明な部分</param>
            public override void MapInputRectsToOutputRect(Vortice.RawRect[] inputRects, Vortice.RawRect[] inputOpaqueSubRects, out Vortice.RawRect outputRect, out Vortice.RawRect outputOpaqueSubRect)
            {
                outputOpaqueSubRect = default;

                Vortice.RawRect input = inputRects[0];

                RawRect[] ranges = [
                    CalcRange(input.Left, input.Top),
                    CalcRange(input.Right, input.Top),
                    CalcRange(input.Left, input.Bottom),
                    CalcRange(input.Right, input.Bottom),
                    ];

                outputRect = new RawRect(
                    ranges.Select(rc => rc.Left).Min(),
                    ranges.Select(rc => rc.Top).Min(),
                    ranges.Select(rc => rc.Right).Max(),
                    ranges.Select(rc => rc.Bottom).Max());
            }

            /// <summary>
            /// 出力画像を生成するために入力する必要のある入力画像の範囲を計算する
            /// 例:
            /// 画像に対して10pxの縁取りエフェクトを掛ける場合、縁取りの計算に周囲10pxの画像が必要なのでinputRects[0]をoutputRectから10px大きくしたものに設定する
            /// 画像に対して10pxのモザイクエフェクトを掛ける場合、モザイクの計算に周囲10pxの画像が必要なのでinputRects[0]をoutputRectから10px大きくしたものに設定する
            /// </summary>
            /// <param name="outputRect">出力画像の範囲。最適化のため、出力画像の範囲がそのまま渡されるわけではなく、分割されることもある。</param>
            /// <param name="inputRects">出力画像を生成するために入力する必要のある入力画像の範囲。</param>
            public override void MapOutputRectToInputRects(Vortice.RawRect outputRect, Vortice.RawRect[] inputRects)
            {
                var radius =
                    new[]
                    {
                        new Vector2(outputRect.Left, outputRect.Top),
                        new Vector2(outputRect.Right, outputRect.Top),
                        new Vector2(outputRect.Left, outputRect.Bottom),
                        new Vector2(outputRect.Right, outputRect.Bottom)
                    }
                    .Select(x => x.Length())
                    .Select(x => (int)MathF.Ceiling(x))
                    .Max();
                radius = Math.Min(radius, 2048);
                inputRects[0] = new RawRect(-radius, -radius, radius, radius);
            }

            private RawRect CalcRange(int x, int y)
            {
                float dx = x - X;
                float dy = y - Y;
                double t = Math.Atan2(dy, dx);
                double maxT_x, minT_x, maxT_y, minT_y;
                double dt = Math.Abs(Math.PI * Amp / (Mode ? 180 : 90));

                if (t < 0)
                {
                    if (t - dt < -Math.PI)
                        minT_x = -Math.PI;
                    else
                        minT_x = t - dt;

                    if (t + dt > 0)
                        maxT_x = 0;
                    else
                        maxT_x = t + dt;
                }
                else
                {
                    if (t - dt < 0)
                        minT_x = 0;
                    else
                        minT_x = t - dt;

                    if (t + dt > Math.PI)
                        maxT_x = Math.PI;
                    else
                        maxT_x = t + dt;
                }

                if (t < -Math.PI / 2)
                {
                    if (t - dt < -Math.PI * 3 / 2)
                        maxT_y = Math.PI / 2;
                    else
                        maxT_y = t - dt;

                    if (t + dt > -Math.PI / 2)
                        minT_y = -Math.PI / 2;
                    else
                        minT_y = t + dt;
                }
                else if (t > Math.PI / 2)
                {
                    if (t - dt < Math.PI / 2)
                        maxT_y = Math.PI / 2;
                    else
                        maxT_y = t - dt;

                    if (t + dt > Math.PI * 3 / 2)
                        minT_y = -Math.PI / 2;
                    else
                        minT_y = t + dt;
                }
                else
                {
                    if (t - dt < -Math.PI / 2)
                        minT_y = 0;
                    else
                        minT_y = t - dt;

                    if (t + dt > Math.PI / 2)
                        maxT_y = Math.PI;
                    else
                        maxT_y = t + dt;
                }

                double r = Math.Sqrt(dx * dx + dy * dy);

                return new(
                    (int)Math.Floor(X + r * Math.Cos(minT_x)),
                    (int)Math.Floor(Y + r * Math.Sin(minT_y)),
                    (int)Math.Ceiling(X + r * Math.Cos(maxT_x)),
                    (int)Math.Ceiling(Y + r * Math.Sin(maxT_y)));
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Amp;
                public float Wlen;
                public float Phase;
                public float Offset;
                public float Strd;
                public float Cmpl;
                public float X;
                public float Y;
                public bool Mode;
                public float Time;
            }

            public enum Properties
            {
                Amp = 0,
                Wlen = 1,
                Phase = 2,
                Offset = 3,
                Strd = 4,
                Cmpl = 5,
                X = 6,
                Y = 7,
                Mode = 8,
                Time = 9,
            }
        }
    }
}
