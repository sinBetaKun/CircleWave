using System.ComponentModel.DataAnnotations;

namespace CircleWave.VideoEffect
{
    internal enum WaveType
    {
        [Display(Name = "回転")]
        Circle,
        [Display(Name = "放射(試験段階)")]
        Radial,
    }
}
