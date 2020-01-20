using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AforgeTest {
    public unsafe class CFastBitmap
    {
        //TODO:lockbits 여부를 확인할 수 있는 프로퍼티 추가
        private Bitmap originalImage;
        private Bitmap dstImage;

        private BitmapData SrcData, DstData;

        private byte* SrcPtr = null, DstPtr = null;
        Rectangle LockRct;

        struct PixelData { internal byte R, G, B, A; }

        public CFastBitmap(Bitmap src)
        {
            originalImage = new Bitmap(src);
            dstImage = new Bitmap(originalImage.Width, originalImage.Height, originalImage.PixelFormat);

            LockRct = new Rectangle(Point.Empty, src.Size);

            SrcData = originalImage.LockBits(LockRct, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            SrcPtr = (byte*) SrcData.Scan0.ToPointer();

            DstData = dstImage.LockBits(LockRct, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            DstPtr = (byte*)DstData.Scan0.ToPointer();
            

        }
    
        public Color this [int X, int Y]
        {
            get
            {
                PixelData* PixelData = (PixelData*)(SrcPtr + Y * SrcData.Stride + X * sizeof(PixelData));
                return Color.FromArgb(PixelData->A, PixelData->B, PixelData->G, PixelData->R);
            }
            set
            {
                PixelData* PixelData = (PixelData*)(DstPtr + Y * DstData.Stride + X * sizeof(PixelData));
                PixelData->A = value.A;
                PixelData->B = value.R;
                PixelData->G = value.G;
                PixelData->R = value.B;
            }
        }

        public void FromBitmap(Bitmap src)
        {
            originalImage = new Bitmap(src);

            Lockbits();
        }

        public void Lockbits()
        {
            LockRct = new Rectangle(Point.Empty, originalImage.Size);

            SrcData = originalImage.LockBits(LockRct, ImageLockMode.ReadOnly, originalImage.PixelFormat);
            SrcPtr = (byte*)SrcData.Scan0.ToPointer();

            DstData = dstImage.LockBits(LockRct, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            DstPtr = (byte*)DstData.Scan0.ToPointer();
        }

        public void Unlockbits()
        {
            originalImage.UnlockBits(SrcData);
            //SrcData = null;
            //SrcPtr = null;

            dstImage.UnlockBits(DstData);
            //DstData = null;
            //DstPtr = null;

        }

        public Bitmap Result()
        {
            Unlockbits();

            return dstImage;
        }
        
        public Bitmap Process(Action<CFastBitmap> work)
        {
            work(this);
            return Result();
        }

        public Bitmap Process(Action<CFastBitmap, CFastBitmap> work, CFastBitmap temp)
        {
            work(this, temp);
            temp.Unlockbits();
            return Result();
        }
        //public Bitmap Process(Action<CFastBitmap> work)
        //{
        //    work(this);
                        
        //    return Result();
        //}
    }
}
