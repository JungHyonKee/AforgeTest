using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace AforgeTest
{
    public partial class Form1 : Form
    {
        public Bitmap Orignal;
        public Bitmap aaa;
        Bitmap Template;
        CameraDistortionCorretion CDC = new CameraDistortionCorretion(0.6);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if(open.ShowDialog() == DialogResult.OK)
            {
                string Path = open.FileName;
                Orignal = new Bitmap(Path);
                pictureBox1.Image = Orignal;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Orignal = ConvertTo24bpp( CDC.Apply( Orignal, 0.6 ) );


            if( Template.PixelFormat != PixelFormat.Format8bppIndexed ) {
                Template = Grayscale.CommonAlgorithms.BT709.Apply( Template );
            }
            if( Orignal.PixelFormat != PixelFormat.Format8bppIndexed ) {
                Orignal = Grayscale.CommonAlgorithms.BT709.Apply( Orignal );
            }

            BradleyLocalThresholding FilterBrad = new BradleyLocalThresholding();
            FilterBrad.WindowSize = 100;
            FilterBrad.PixelBrightnessDifferenceLimit = 0.2f;
            Template = FilterBrad.Apply( Template );
            Orignal = FilterBrad.Apply( Orignal );
            //TemplatImage.Save( @"D:\Temp.jpg" );
            //SourceImage.Save( @"D:\src.jpg" );
            if( Template.PixelFormat != PixelFormat.Format24bppRgb ) {
                GrayscaleToRGB FilterRGB = new GrayscaleToRGB();
                Template = FilterRGB.Apply( Template );
            }
            if( Orignal.PixelFormat != PixelFormat.Format24bppRgb ) {
                GrayscaleToRGB FilterRGB = new GrayscaleToRGB();
                Orignal = FilterRGB.Apply( Orignal );
            }

            int divisor = int.Parse(textBox1.Text);
            int CellsizeLength = int.Parse(textBox2.Text);
            ExhaustiveTemplateMatching etm = new ExhaustiveTemplateMatching(0.1f);
            if (Orignal.PixelFormat != PixelFormat.Format24bppRgb)
            {
                GrayscaleToRGB FilterRGB = new GrayscaleToRGB();
                Orignal = FilterRGB.Apply(Orignal);
            }

            ResizeNearestNeighbor Resize_filter2 = new ResizeNearestNeighbor(Orignal.Width / divisor, Orignal.Height / divisor);
            Bitmap Resize_Org_Image = Resize_filter2.Apply(AForge.Imaging.Image.Clone(Orignal)); 



            ResizeNearestNeighbor Resize_filter3 = new ResizeNearestNeighbor(Template.Width / divisor, Template.Height / divisor);
            Bitmap Resize_Template = Resize_filter3.Apply(AForge.Imaging.Image.Clone(Template));


            TemplateMatch[] tm = etm.ProcessImage(Resize_Org_Image, Resize_Template);

            if (tm.Length>0)
            {
                List<IntPoint> cornersRect = new List<IntPoint>
                {
                    new IntPoint(tm[0].Rectangle.X * divisor - CellsizeLength, tm[0].Rectangle.Y * divisor - CellsizeLength),
                    new IntPoint((tm[0].Rectangle.X * divisor) + (tm[0].Rectangle.Width * divisor) + CellsizeLength, tm[0].Rectangle.Y * divisor - CellsizeLength),
                    new IntPoint((tm[0].Rectangle.X * divisor) + (tm[0].Rectangle.Width * divisor) + CellsizeLength, (tm[0].Rectangle.Y * divisor) + (tm[0].Rectangle.Height * divisor) + CellsizeLength),
                    new IntPoint(tm[0].Rectangle.X * divisor - CellsizeLength, (tm[0].Rectangle.Y * divisor) + (tm[0].Rectangle.Height * divisor) + CellsizeLength)
                };
                SimpleQuadrilateralTransformation squadtran = new SimpleQuadrilateralTransformation(cornersRect, Orignal.Width + CellsizeLength * 2, Orignal.Height + CellsizeLength * 2)
                {
                    AutomaticSizeCalculaton = true
                };
                Bitmap ExhaustiveTemplate24bit = squadtran.Apply(AForge.Imaging.Image.Clone(Orignal));
                ExhaustiveTemplate24bit = new ResizeNearestNeighbor(Template.Width, Template.Height).Apply(ExhaustiveTemplate24bit);
                pictureBox3.Image = ExhaustiveTemplate24bit;
            }
            GC.Collect();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                string Path = open.FileName;
                Template = new Bitmap(Path);
                pictureBox2.Image = Template;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox3.Image = null;
        }

        private void button5_Click( object sender, EventArgs e ) {
            var Result = ConvertTo24bpp(CDC.Apply( Orignal,double.Parse(textBox2.Text) ));
            pictureBox3.Image = Result;
            Result.Save( @"D:\새 폴더\CameraDistortionCorretion.bmp" );
        }
        public static Bitmap ConvertTo24bpp( Bitmap img ) {
            var bmp = new Bitmap( img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
            using( var gr = Graphics.FromImage( bmp ) )
                gr.DrawImage( img, new Rectangle( 0, 0, img.Width, img.Height ) );
            return bmp;
        }
        private void button6_Click( object sender, EventArgs e ) {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var a = 111;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var b = 111;
        }
    }
}
