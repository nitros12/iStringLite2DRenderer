using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace iStringLite2DRenderer.Effects
{
    public class AnimatedGifEffect : BitmapEffect
    {
        private Image _gif;
        private FrameDimension _frames;
        private int _frameCount;
        private int _frameIndex;
        
        public AnimatedGifEffect(string imageLocation) : base(imageLocation)
        {
            LoadGif(imageLocation);
        }

        public AnimatedGifEffect(string imageLocation, int positionX, int positionY) : base(imageLocation, positionX, positionY)
        {
            LoadGif(imageLocation);
        }

        public AnimatedGifEffect(string imageLocation, int positionX, int positionY, VideoBuffer videoBuffer) : base(imageLocation, positionX, positionY, videoBuffer)
        {
            LoadGif(imageLocation);
        }

        public AnimatedGifEffect(string imageLocation, int positionX, int positionY, int width, int height) : base(imageLocation, positionX, positionY, width, height)
        {
            LoadGif(imageLocation);
        }

        public void LoadGif(string imageLocation)
        {
            _gif = Image.FromFile(imageLocation);
            _frames = new FrameDimension(_gif.FrameDimensionsList[0]);
            _frameCount = _gif.GetFrameCount(_frames);

            Console.WriteLine("\tGif has {0} frames", _frameCount);
            
            PropertyItem item = _gif.GetPropertyItem (0x5100); // frameDelay in libgdiplus
            Interval = (item.Value [0] + item.Value [1] * 256) * 10; // sets interval to framerate delay of gif
            play(); // start the animation
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            if (!intervalReached()) return;
            
            //Console.WriteLine("Frame: {0}/{1}", (_frameIndex + 1), _frameCount);
            
            // load current frame
            _gif.SelectActiveFrame(_frames, _frameIndex);
            _image = new Bitmap(_gif, new Size(_image.Width, _image.Height));
            
            // increment frame
            _frameIndex++;
            if(_frameIndex >= _frameCount)
                _frameIndex = 0;
            
            // have the BitmapEffect code render the frame
            base.update(ref videoBuffer);
            Timer.Restart();
        }

        public override void reset()
        {
            _frameIndex = 0; // reset current frame index back to 0
            base.reset();
        }
    }
}