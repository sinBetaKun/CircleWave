using System.Runtime.InteropServices;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CircleWave.VideoEffect.CustomEffect
{
    internal class RadialWaveCustomEffect : D2D1CustomShaderEffectBase
    {
        public float Amp
        {
            set => SetValue((int)EffectImpl.Properties.Amp, value);
            get => GetFloatValue((int)EffectImpl.Properties.Amp);
        }

        public int Count
        {
            set => SetValue((int)EffectImpl.Properties.Count, value);
            get => GetIntValue((int)EffectImpl.Properties.Count);
        }
        public float Angle
        {
            set => SetValue((int)EffectImpl.Properties.Angle, value);
            get => GetFloatValue((int)EffectImpl.Properties.Angle);
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

        public RadialWaveCustomEffect(IGraphicsDevicesAndContext devices) : base(Create<EffectImpl>(devices))
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

            [CustomEffectProperty(PropertyType.Int32, (int)Properties.Count)]
            public int Count
            {
                get => constantBuffer.Count;
                set
                {
                    constantBuffer.Count = value;
                    UpdateConstants();
                }
            }

            [CustomEffectProperty(PropertyType.Float, (int)Properties.Angle)]
            public float Angle
            {
                get => constantBuffer.Angle;
                set
                {
                    constantBuffer.Angle = value;
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

            public EffectImpl() : base(ShaderResourceLoader.GetShaderResource("RadialWave.cso")/*ここでシェーダーのbyte列を渡す*/)
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

                outputRect = new(
                    (int)Math.Floor(inputRects[0].Left - Math.Abs((inputRects[0].Left - X) * Amp / 100)),
                    (int)Math.Floor(inputRects[0].Top - Math.Abs((inputRects[0].Top - Y) * Amp / 100)),
                    (int)Math.Ceiling(inputRects[0].Right + Math.Abs((inputRects[0].Right - X) * Amp / 100)),
                    (int)Math.Ceiling(inputRects[0].Bottom + Math.Abs((inputRects[0].Bottom - Y) * Amp / 100)));
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
                inputRects[0] = new(
                    (int)Math.Floor(outputRect.Left - Math.Abs((outputRect.Left - X) * Amp / 50)),
                    (int)Math.Floor(outputRect.Top - Math.Abs((outputRect.Top - Y) * Amp / 50)),
                    (int)Math.Ceiling(outputRect.Right + Math.Abs((outputRect.Right - X) * Amp / 50)),
                    (int)Math.Ceiling(outputRect.Bottom + Math.Abs((outputRect.Bottom - Y) * Amp / 50)));
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Amp;
                public int Count;
                public float Angle;
                public float X;
                public float Y;
            }

            public enum Properties
            {
                Amp = 0,
                Count = 1,
                Angle = 2,
                X = 3,
                Y = 4,
            }
        }
    }
}
