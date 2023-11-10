using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono_labo_1.Klas_Frames
{
    internal class AnimationRow
    {
        public List<AnimationFrame> frames { get; set; }

        public AnimationRow()
        {
            frames = new List<AnimationFrame>();
        }
    }
}
