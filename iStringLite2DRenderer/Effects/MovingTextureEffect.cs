using System;

namespace iStringLite2DRenderer.Effects
{
    public class MovingTextureEffect : BitmapEffect
    {
        public MovingTextureEffect(string imageLocation) : base(imageLocation)
        {
        }

        public MovingTextureEffect(string imageLocation, int positionX, int positionY) : base(imageLocation, positionX, positionY)
        {
        }

        public MovingTextureEffect(string imageLocation, int positionX, int positionY, VideoBuffer videoBuffer) : base(imageLocation, positionX, positionY, videoBuffer)
        {
        }

        public MovingTextureEffect(string imageLocation, int positionX, int positionY, int width, int height) : base(imageLocation, positionX, positionY, width, height)
        {
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            //TODO: Very basic movement at the minute for testing. Needs a wrap function too.
            _positionX++;
            _positionY++;
            base.update(ref videoBuffer);
        }
    }
}