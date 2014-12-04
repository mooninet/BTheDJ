/*
 * 완전 미완성 클래스...아예.
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
namespace BTheDJ.BMSData
{
    //bitmap들을 담아두는 리스트를 생성하는 클래스.
    public class BitmapList : Bitmap
    {
        //static string Filename = "Image/white";
        BMSResource.Images Resources;
        public List<Bitmap> bitmapList = new List<Bitmap> { };
        //ContentManager Content;

        public BitmapList(BMSResource.Images Resources)
            :base(null, Resources)
        {
            //this.Content = contentManager;
            this.Resources = Resources;
        }

        public void AddBitmap(string filename)
        {
            Bitmap Bitmap = new Bitmap(filename, Resources);
            bitmapList.Add(Bitmap);
        }

        public void AddBitmap(Bitmap bitmap)
        {
            bitmapList.Add(bitmap);
        }

        public Bitmap getBitmap(int bitNumber)
        {
            return bitmapList[bitNumber];
        }
    }
}
