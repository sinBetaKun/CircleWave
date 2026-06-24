using System.Numerics;
using System.Runtime.InteropServices;
using Vortice;
using Vortice.Direct2D1;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace CircleWave.VideoEffect.CustomEffect
{
    internal class CircleWave2CustomEffect : D2D1CustomShaderEffectBase
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

        public float E
        {
            set => SetValue((int)EffectImpl.Properties.E, value);
            get => GetFloatValue((int)EffectImpl.Properties.E);
        }

        public CircleWave2CustomEffect(IGraphicsDevicesAndContext devices) : base(Create<EffectImpl>(devices))
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

            [CustomEffectProperty(PropertyType.Float, (int)Properties.E)]
            public float E
            {
                get => constantBuffer.E;
                set
                {
                    constantBuffer.E = value;
                    UpdateConstants();
                }
            }

            public EffectImpl() : base(ShaderResourceLoader.GetShaderResource("CircleWave2.cso")/*ここでシェーダーのbyte列を渡す*/)
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
            public override void MapInputRectsToOutputRect(RawRect[] inputRects, RawRect[] inputOpaqueSubRects, out RawRect outputRect, out RawRect outputOpaqueSubRect)
            {
                outputOpaqueSubRect = default;

                RawRect input = inputRects[0];
                var globalBounds = new Bounds();
                object lockObj = new();

                Parallel.For(
                    input.Left,
                    input.Right,
                    () => new Bounds(),   // スレッドごとのBounds,
                    (x, state, local) =>
                    {
                        local.Add(Transform(x, input.Top));
                        local.Add(Transform(x, input.Bottom));
                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            globalBounds.Merge(local);
                        }
                    });

                Parallel.For(
                    input.Top + 1,
                    input.Bottom - 1,
                    () => new Bounds(),   // スレッドごとのBounds,
                    (y, state, local) =>
                    {
                        local.Add(Transform(input.Left, y));
                        local.Add(Transform(input.Right, y));
                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            globalBounds.Merge(local);
                        }
                    });

                outputRect = new RawRect(
                    (int)MathF.Floor(globalBounds.MinX),
                    (int)MathF.Floor(globalBounds.MinY),
                    (int)MathF.Ceiling(globalBounds.MaxX),
                    (int)MathF.Ceiling(globalBounds.MaxY));
            }

            /// <summary>
            /// 出力画像を生成するために入力する必要のある入力画像の範囲を計算する
            /// 例:
            /// 画像に対して10pxの縁取りエフェクトを掛ける場合、縁取りの計算に周囲10pxの画像が必要なのでinputRects[0]をoutputRectから10px大きくしたものに設定する
            /// 画像に対して10pxのモザイクエフェクトを掛ける場合、モザイクの計算に周囲10pxの画像が必要なのでinputRects[0]をoutputRectから10px大きくしたものに設定する
            /// </summary>
            /// <param name="outputRect">出力画像の範囲。最適化のため、出力画像の範囲がそのまま渡されるわけではなく、分割されることもある。</param>
            /// <param name="inputRects">出力画像を生成するために入力する必要のある入力画像の範囲。</param>
            public override void MapOutputRectToInputRects(RawRect outputRect, RawRect[] inputRects)
            {
                var globalBounds = new Bounds();
                object lockObj = new();

                Parallel.For(
                    outputRect.Left,
                    outputRect.Right,
                    () => new Bounds(),   // スレッドごとのBounds,
                    (x, state, local) =>
                    {
                        local.Add(InvTransform(x, outputRect.Top));
                        local.Add(InvTransform(x, outputRect.Bottom));
                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            globalBounds.Merge(local);
                        }
                    });

                Parallel.For(
                    outputRect.Top + 1,
                    outputRect.Bottom - 1,
                    () => new Bounds(),   // スレッドごとのBounds,
                    (y, state, local) =>
                    {
                        local.Add(InvTransform(outputRect.Left, y));
                        local.Add(InvTransform(outputRect.Right, y));
                        return local;
                    },
                    local =>
                    {
                        lock (lockObj)
                        {
                            globalBounds.Merge(local);
                        }
                    });

                inputRects[0] = new RawRect(
                    (int)MathF.Floor(globalBounds.MinX),
                    (int)MathF.Floor(globalBounds.MinY),
                    (int)MathF.Ceiling(globalBounds.MaxX),
                    (int)MathF.Ceiling(globalBounds.MaxY));
            }

            private Vector2 Transform(float x0, float y0)
            {
                float x1 = x0 - X;
                float y1 = y0 - Y;
                float f0 = MathF.Sqrt(x1 * x1 + y1 * y1);
                float f1 = MathF.Max(0, f0 - Strd);
                float f2 = f1 * Amp * MathF.Sin((Offset - MathF.Pow(f1 / Wlen + 1.0f, E)) * MathF.Tau);
                float f3 = f2 / f0;
                float s = MathF.Sin(-f3);
                float c = MathF.Cos(f3); // = cos(-f3)
                return new Vector2(X + c * x1 - s * y1, Y + s * x1 + c * y1);
            }

            private Vector2 InvTransform(float x0, float y0)
            {
                float x1 = x0 - X;
                float y1 = y0 - Y;
                float f0 = MathF.Sqrt(x1 * x1 + y1 * y1);
                float f1 = MathF.Max(0, f0 - Strd);
                float f2 = f1 * Amp * MathF.Sin((Offset - MathF.Pow(f1 / Wlen + 1.0f, E)) * MathF.Tau);
                float f3 = f2 / f0;
                float s = MathF.Sin(f3);
                float c = MathF.Cos(f3);
                return new Vector2(X + c * x1 - s * y1, Y + s * x1 + c * y1);
            }

            private struct Bounds
            {
                public float MinX;
                public float MinY;
                public float MaxX;
                public float MaxY;

                public Bounds()
                {
                    MinX = float.MaxValue;
                    MinY = float.MaxValue;
                    MaxX = float.MinValue;
                    MaxY = float.MinValue;
                }

                public void Add(Vector2 p)
                {
                    MinX = MathF.Min(MinX, p.X);
                    MinY = MathF.Min(MinY, p.Y);

                    MaxX = MathF.Max(MaxX, p.X);
                    MaxY = MathF.Max(MaxY, p.Y);
                }

                public void Merge(Bounds other)
                {
                    MinX = MathF.Min(MinX, other.MinX);
                    MinY = MathF.Min(MinY, other.MinY);

                    MaxX = MathF.Max(MaxX, other.MaxX);
                    MaxY = MathF.Max(MaxY, other.MaxY);
                }

                public readonly float Width => MaxX - MinX;
                public readonly float Height => MaxY - MinY;
            }

            [StructLayout(LayoutKind.Sequential)]
            struct ConstantBuffer
            {
                public float Amp;
                public float Wlen;
                public float Offset;
                public float Strd;
                public float X;
                public float Y;
                public float E;
            }

            public enum Properties
            {
                Amp = 0,
                Wlen = 1,
                Offset = 2,
                Strd = 3,
                X = 4,
                Y = 5,
                E = 6,
            }
        }
    }
}
