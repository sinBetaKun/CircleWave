using System.ComponentModel.DataAnnotations;

namespace CircleWave.VideoEffect
{
    internal enum WaveType
    {
        [Display(Name = "回転（問題未解決）")]
        Circle,
        [Display(Name = "回転2")]
        Circle2,
        [Display(Name = "放射")]
        Radial,
    }
}
