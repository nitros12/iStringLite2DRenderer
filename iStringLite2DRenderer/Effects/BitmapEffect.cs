using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace iStringLite2DRenderer.Effects
{
    public class BitmapEffect : Effect
    {
	    protected Bitmap _image;
        protected int _positionX;
        protected int _positionY;

        /// <summary>
        /// Used when creating a fullscreen, scaled bitmap that fills the entire VideoBuffer.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        public BitmapEffect(string imageLocation)
        {
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
        public BitmapEffect(string imageLocation, int positionX, int positionY)
        {
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
        public BitmapEffect(string imageLocation, int positionX, int positionY, VideoBuffer videoBuffer)
        {
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
                Console.WriteLine("Image could not be loaded from {0}", imageLocation);
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
        public BitmapEffect(string imageLocation, int positionX, int positionY, int width, int height)
        {
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