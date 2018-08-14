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

        public static readonly string FONT = "Arial";

        public TextEffect(string text, int positionX, int positionY, int textSize, Brush colour, VideoBuffer videoBuffer) : base()
        {
            //TODO: Incomplete
            _text = text;
            _positionX = 0;
            _positionY = 0;
            _textPositionX = videoBuffer.Width;
            _textPositionY = positionY;
            _textSize = textSize;
            _image = new Bitmap(videoBuffer.Width, videoBuffer.Height);
            _rectf = new RectangleF(0, 0, _image.Width, _image.Height);
            _g = Graphics.FromImage(_image);
            _brush = colour;
            
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            _g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //_g.InterpolationMode = InterpolationMode.Low;
            _g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        public override void update(ref VideoBuffer videoBuffer)
        {
            _g.Clear(Color.Transparent);
            _g.DrawString(_text, new Font(FONT, _textSize), _brush, new PointF(_textPositionX, _textPositionY));
            _g.Flush();
            
            base.update(ref videoBuffer);
        }

        public override void reset()
        {
            throw new System.NotImplementedException();
        }

        public override bool isComplete { get; set; }
    }
}