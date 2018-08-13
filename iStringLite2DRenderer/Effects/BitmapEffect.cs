using System;
using System.Drawing;
using iStringLite_2DRenderer;

namespace iStringLite_Stuff.Effects
{
    public class BitmapEffect : Effect
    {
	    private readonly Bitmap _image;
        private int _positionX;
        private int _positionY;

        private bool _fullscreen;

        /// <summary>
        /// Used when creating a fullscreen, scaled bitmap that fills the entire VideoBuffer.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        public BitmapEffect(string imageLocation)
        {
            this._image = new Bitmap(imageLocation);
            this._positionX = 0;
            this._positionY = 0;
            this._fullscreen = false;
        }

        /// <summary>
        /// Used when creating a positional bitmap that does not resize.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        /// <param name="positionX">The X position to start rendering the image from</param>
        /// <param name="positionY">The Y position to start rendering the image from</param>
        public BitmapEffect(string imageLocation, int positionX, int positionY)
        {
            this._image = new Bitmap(imageLocation);
            this._positionX = positionX;
            this._positionY = positionY;
            this._fullscreen = false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Used when creating a positional bitmap that does not resize.
        /// </summary>
        /// <param name="imageLocation">Location of the image to be rendered</param>
        /// <param name="positionX">The X position to start rendering the image from</param>
        /// <param name="positionY">The Y position to start rendering the image from</param>
        /// <param name="fullscreen">Whether or not the image is scaled to fit the VideoBuffer dimensions</param>
        /// <param name="videoBuffer">VideoBuffer to be used for scaling if fullscreen is true, can be null otherwise</param>
        public BitmapEffect(string imageLocation, int positionX, int positionY, bool fullscreen, VideoBuffer videoBuffer)
        {
            this._image = new Bitmap(imageLocation);
            this._positionX = positionX;
            this._positionY = positionY;
            this._fullscreen = fullscreen;

            if (fullscreen && videoBuffer != null)
            {
                this._image = new Bitmap(_image, new Size(videoBuffer.Width, videoBuffer.Height)); // resize the image to the video buffer
                //TODO: if fullscreen and vbuffer is null, throw exception?
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
	        this._image = new Bitmap(imageLocation);
            this._image = new Bitmap(_image, new Size(width, height)); // resize the image
            
            this._positionX = positionX;
            this._positionY = positionY;
            _fullscreen = false;
        }
        
        public override void update(ref VideoBuffer videoBuffer)
        {
            try
            {
                // loops through image and copied it into the buffer
                for (int y = _positionY; y < _image.Height; y++)
                {
                    for (int x = _positionX; x < _image.Width; x++)
                    {
                        videoBuffer.Buffer[y, x] = colorToRGB(_image.GetPixel(x, y));
                    }
                }
                
                /*// loops through video buffer and 
                for (int y = 0; y < videoBuffer.Buffer.GetLength(0); y++)
                {
                    for (int x = 0; x < videoBuffer.Buffer.GetLength(1); x++)
                    {
                        videoBuffer.Buffer[y, x] = colorToRGB(_image.GetPixel(x, y));
                        //Console.WriteLine("R: {0}, G: {1}, B: {2} RGB: {3}", image.GetPixel(x, y).R, image.GetPixel(x, y).G, image.GetPixel(x, y).B, colorToRGB(image.GetPixel(x, y)));
                    }
                }*/
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
            return (uint) (color.R << 8 | color.G << 8 | color.B);
        }
    }
}