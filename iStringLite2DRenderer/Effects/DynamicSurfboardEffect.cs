using System;
using System.Collections;

namespace iStringLite2DRenderer.Effects
{
    public class DynamicSurfboardEffect : Effect
    {
        public DynamicSurfboardEffect()
        {
            this.Interval = 100;
            play();
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            if (!intervalReached()) return;
            //for(int y = 0; videoBuffer)
            Timer.Restart();
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }
    }
}