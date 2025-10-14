using YukkuriMovieMaker.Commons;

namespace CircleWave.VideoEffect
{
    internal class FrameAndLength(int frame, int length)
    {
        public int Frame { get; init; } = frame;
        public int Length { get; init; } = length;

        public double GetValue(Animation animation, int fps) =>
            animation.GetValue(Frame, Length, fps);
    }
}
