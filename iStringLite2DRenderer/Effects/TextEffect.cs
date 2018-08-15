using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace iStringLite2DRenderer.Effects
{
    public class TextEffect : BitmapEffect
    {
        private string _text;
        protected int _textPositionX;
        private int _textPositionY;
        private int _textSize;
        protected RectangleF _rectf;
        private Graphics _g;
        private Brush _brush;
        private Brush _shadow;

        public static readonly string FONT = "Arial";
        public static readonly int SHADOW_THICKNESS = 1;
        private static int START_POSITION_X;
        private static int START_POSITION_Y;


        public TextEffect(string text, int positionX, int positionY, int textSize, Brush colour, VideoBuffer videoBuffer) : base()
        {
            //TODO: Incomplete
            _text = text;
            _positionX = 0;
            _positionY = 0;
            _textPositionX = positionX;
            _textPositionY = positionY;
            START_POSITION_X = _textPositionX;
            START_POSITION_Y = _textPositionY;
            _textSize = textSize;
            _brush = colour;
            
            _image = new Bitmap(videoBuffer.Width, videoBuffer.Height);
            _rectf = new RectangleF(0, 0, _image.Width, _image.Height);
            _g = Graphics.FromImage(_image);
                        
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            _g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            _g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }
        
        public TextEffect(string text, int positionX, int positionY, int textSize, Brush colour, Brush shadow, VideoBuffer videoBuffer) :  this(text, positionX, positionY, textSize, colour, videoBuffer)
        {
            _shadow = shadow;
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            _g.Clear(Color.Transparent);

            if (isComplete) return;
            
            // draw shadow
            if(_shadow != null)
                _g.DrawString(_text, new Font(FONT, _textSize), _shadow, new PointF(_textPositionX + SHADOW_THICKNESS, _textPositionY + (SHADOW_THICKNESS * 2)));
            
            // draw string
            _g.DrawString(_text, new Font(FONT, _textSize), _brush, new PointF(_textPositionX, _textPositionY));
            _g.Flush();
            
            //TODO: Check when 
            
            base.update(ref videoBuffer);
        }

        public override void reset()
        {
            Timer.Restart();
            _textPositionX = START_POSITION_X;
            _textPositionY = START_POSITION_Y;
        }

        public override bool isComplete { get; set; }
    }
}