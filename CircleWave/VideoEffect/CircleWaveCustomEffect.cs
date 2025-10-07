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
        
        public float Strd
        {
            set => SetValue((int)EffectImpl.Properties.Strd, value);
            get => GetFloatValue((int)EffectImpl.Properties.Strd);
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
        
        public int Mode
        {
            set => SetValue((int)EffectImpl.Properties.Mode, value);
            get => GetIntValue((int)EffectImpl.Properties.Mode);
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

            public float Amp
            {
                get => constantBuffer.Amp;
                set
                {
                    constantBuffer.Amp = value;
                    UpdateConstants();
                }
            }

            public float Wlen
            {
                get => constantBuffer.Wlen;
                set
                {
                    constantBuffer.Wlen = value;
                    UpdateConstants();
                }
            }

            public float Phase
            {
                get => constantBuffer.Phase;
                set
                {
                    constantBuffer.Phase = value;
                    UpdateConstants();
                }
            }

            public float Strd
            {
                get => constantBuffer.Strd;
                set
                {
                    constantBuffer.Strd = value;
                    UpdateConstants();
                }
            }

            public float X
            {
                get => constantBuffer.X;
                set
                {
                    constantBuffer.X = value;
                    UpdateConstants();
                }
            }

            public float Y
            {
                get => constantBuffer.Y;
                set
                {
                    constantBuffer.Y = value;
                    UpdateConstants();
                }
            }

            public int Mode
            {
                get => constantBuffer.Mode;
                set
                {
                    constantBuffer.Mode = value;
                    UpdateConstants();
                }
            }

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
                float v = MathF.Max(MathF.Abs(input.Top - Y), MathF.Abs(input.Bottom - Y));
                float h = MathF.Max(MathF.Abs(input.Left - X), MathF.Abs(input.Right - X));
                float r = MathF.Sqrt(v * v + h * h);
                outputRect = new RawRect(
                    (int)MathF.Floor(X - r),
                    (int)MathF.Floor(Y - r),
                    (int)MathF.Ceiling(X + r),
                    (int)MathF.Ceiling(Y + r));
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Amp;
                public float Wlen;
                public float Phase;
                public float Strd;
                public float X;
                public float Y;
                public int Mode;
                public float Time;
            }

            public enum Properties
            {
                Amp = 0,
                Wlen = 1,
                Phase = 2,
                Strd = 3,
                X = 4,
                Y = 5,
                Mode = 6,
                Time = 7,
            }
        }
    }
}
