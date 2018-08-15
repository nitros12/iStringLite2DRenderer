using System.Drawing;

namespace iStringLite2DRenderer.Effects
{
    public class ScrollingTextEffect : TextEffect
    {
        public ScrollingTextEffect(string text, int positionX, int positionY, int textSize, int interval, Brush colour, VideoBuffer videoBuffer) : base(text, positionX, positionY, textSize, colour, videoBuffer)
        {
            Interval = interval;
            play();
        }
        
        public ScrollingTextEffect(string text, int positionX, int positionY, int textSize, int interval, Brush colour, Brush shadow, VideoBuffer videoBuffer) : base(text, positionX, positionY, textSize, colour, shadow, videoBuffer)
        {
            Interval = interval;
            play();
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            base.update(ref videoBuffer);
            if(!intervalReached()) return;
            _textPositionX--;
            
            Timer.Restart();
        }

        public override void reset()
        {
            _textPositionX = 0;
            Timer.Restart();
        }
    }
}