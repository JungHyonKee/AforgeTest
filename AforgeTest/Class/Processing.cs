using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace AforgeTest {

    public static class Processing {

        public unsafe static Bitmap Process( this Image source, Action<Pixel> work ) {
            Pixel bmp2 = new Pixel( source );
            work( bmp2 );
            return bmp2.Result();
        }

        public unsafe static Bitmap Process( this Image source, Image source2, Action<Pixel, Pixel> work)
        {
            Pixel bmp1 = new Pixel( source );
            Pixel bmp2 = new Pixel( source2 );

            work( bmp1, bmp2 );
            return bmp1.Result();
        }

        public unsafe class Pixel {
            struct PixelData { internal byte R, G, B, A; }

            int Length = 0;
            BitmapData SrcData = null, DstData = null;
            byte* SrcPtr = null, DstPtr = null; 

            Bitmap src, dst;
            public Pixel( Image bmp ) {
                src = new Bitmap( bmp );
                dst = new Bitmap( src.Width, src.Height );

                Rectangle LockRct = new Rectangle( Point.Empty, src.Size );

                Length = LockRct.Width * sizeof( PixelData );
                if( Length % 4 != 0 ) Length = 4 * ( Length / 4 + 1 );

                SrcData = src.LockBits( LockRct, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
                SrcPtr = (byte*)SrcData.Scan0.ToPointer();

                DstData = dst.LockBits( LockRct, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb );
                DstPtr = (byte*)DstData.Scan0.ToPointer();
            }

            public Color this[ int X, int Y ] {
                get {
                    PixelData* PixelData = (PixelData*)( SrcPtr + Y * Length + X * sizeof( PixelData ) );
                    return Color.FromArgb( PixelData->A, PixelData->B, PixelData->G, PixelData->R );
                }
                set {
                    PixelData* PixelData = (PixelData*)( DstPtr + Y * Length + X * sizeof( PixelData ) );
                    PixelData->A = value.A;
                    PixelData->B = value.R;
                    PixelData->G = value.G;
                    PixelData->R = value.B;
                }
            }

            public Bitmap Result() {
                src.UnlockBits( SrcData );
                SrcData = null;
                SrcPtr = null;

                dst.UnlockBits( DstData );
                DstData = null;
                DstPtr = null;

                return dst;
            }
        }
    }
}