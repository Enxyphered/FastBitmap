using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FastBitmap
{
    public unsafe class FastBitmap : IDisposable
    {
        protected Bitmap _bmp;
        protected int _pixelLength;

        protected Rectangle _rect;
        protected BitmapData _data;
        protected byte* _bufferPtr;

        public int Width { get => _bmp.Width; }
        public int Height { get => _bmp.Height; }
        public PixelFormat PixelFormat { get => _bmp.PixelFormat; }

        public FastBitmap(Bitmap bmp, ImageLockMode lockMode)
        {
            _bmp = bmp;
            _pixelLength = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            _rect = new Rectangle(0, 0, Width, Height);
            _data = bmp.LockBits(_rect, lockMode, PixelFormat);
            _bufferPtr = (byte*)_data.Scan0.ToPointer();
        }

        public Span<byte> this[int x, int y]
        {
            get
            {
                var pixel = _bufferPtr + y * _data.Stride * x * _pixelLength;
                return new Span<byte>(pixel, _pixelLength);
            }
            set
            {
                value.CopyTo(this[x, y]);
            }
        }

        public void Dispose()
        {
            _bmp.UnlockBits(_data);
        }
    }
}
