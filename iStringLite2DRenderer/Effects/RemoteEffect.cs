using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace iStringLite2DRenderer.Effects
{
    public class RemoteEffect : Effect
    {
	    protected Bitmap _image;
        protected int _positionX;
        protected int _positionY;
        private String imageLocation;

        public RemoteEffect()
        {
        }

        /// <summary>
        /// Used when creating a fullscreen, scaled bitmap that fills the entire VideoBuffer.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        public RemoteEffect(string imageLocation)
        {
            this.imageLocation = imageLocation;
            try
            {
                this._image = new Bitmap(imageLocation);
                this._positionX = 0;
                this._positionY = 0;
            }
            catch (Exception e)
            {
                //TODO: Throw a BitMapEffectInvalidLocation Exception to end program when an invalid scene was passed
                Console.WriteLine("Image could not be loaded from {0}", imageLocation);
            }
        }

        /// <summary>
        /// Used when creating a positional bitmap that does not resize.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        /// <param name="positionX">The X position to start rendering the image from</param>
        /// <param name="positionY">The Y position to start rendering the image from</param>
        public RemoteEffect(string imageLocation, int positionX, int positionY)
        {
            this.imageLocation = imageLocation;
            try
            {
                this._image = new Bitmap(imageLocation);
                this._positionX = positionX;
                this._positionY = positionY;
            }
            catch (Exception e)
            {
                Console.WriteLine("Image could not be loaded from {0}", imageLocation);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Used when creating a positional bitmap that does not resize.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        /// <param name="positionX">The X position to start rendering the image from</param>
        /// <param name="positionY">The Y position to start rendering the image from</param>
        /// <param name="videoBuffer">VideoBuffer to be used for scaling the image to</param>
        public RemoteEffect(string imageLocation, int positionX, int positionY, VideoBuffer videoBuffer)
        {
            this.imageLocation = imageLocation;
            try
            {
                this._image = new Bitmap(imageLocation);
                this._positionX = positionX;
                this._positionY = positionY;

                if (videoBuffer != null)
                {
                    this._image = new Bitmap(_image, new Size(videoBuffer.Width, videoBuffer.Height)); // resize the image to the video buffer
                    //TODO: if fullscreen and vbuffer is null, throw exception?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Image could not be loaded from {0}, {1}", imageLocation, e);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Used when creating a resizable and positionable bitmap.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        /// <param name="positionX">The X position to start rendering the image from</param>
        /// <param name="positionY">The Y position to start rendering the image from</param>
        /// <param name="width">The width of the image to be rendered</param>
        /// <param name="height">The height of the image to be rendered</param>
        public RemoteEffect(string imageLocation, int positionX, int positionY, int width, int height)
        {
            this.imageLocation = imageLocation;
            try
            {
                this._image = new Bitmap(imageLocation);
                this._image = new Bitmap(_image, new Size(width, height)); // resize the image

                this._positionX = positionX;
                this._positionY = positionY;
            }
            catch (Exception e)
            {
                Console.WriteLine("Image could not be loaded from {0}", imageLocation);
            }
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            try
            {
                this._image = new Bitmap(imageLocation);
                /*// loops through image and copied it into the buffer
                for (int y = _positionY; y < _image.Height + _positionY; y++)
                {
                    for (int x = _positionX; x < _image.Width + _positionY; x++)
                    {
                        if(x >= 0 && y >= 0 && x < videoBuffer.Width && y < videoBuffer.Height)
                            videoBuffer.Buffer[y, x] = colorToRGB(_image.GetPixel(x, y));
                    }
                }*/


                // loops through video buffer and
                for (int y = 0; y < videoBuffer.Height; y++)
                {
                    for (int x = 0; x < videoBuffer.Width; x++)
                    {
                        if(x - _positionX >= 0 && y - _positionY >= 0 && x - _positionX < _image.Width && y - _positionY < _image.Height)
                            if(_image.GetPixel(x, y).A != 0) // dont render transparrent pixels
                                videoBuffer.setPixel(x, y, colorToRGB(_image.GetPixel(x - _positionX, y - _positionY)));
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: Proper exceptions
            }
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }

        private uint colorToRGB(Color color)
        {
            int rgb = color.B << 8 | color.G;
            rgb = rgb << 8 | color.R;
            return (uint) rgb;
        }
    }
}
