/*
 * 
 * XNA특성상 해당 클래스는 불필요하다. 삭제할 예정
 *
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace BTheDJ.BMSData
{
    // 비트맵파일의 패스를 저장함과 동시에 로드하는 기본적인 비트맵클래스
    public class Bitmap : Microsoft.Xna.Framework.Game
    {
        //protected ContentManager Content;
        //private string Filename;
        private Texture2D texture;
        public Texture2D TEXTURE
        {
            get { return texture; }
            set { TEXTURE = value; }
        }
        // 비트맵클래스를 생성할 때 생성할 때 마다 로드할게 아니라 images클래스안의 로드된 데이터를 갖고오자.
        //public Bitmap(string filename, BMSResource.Images imageResources)
        public Bitmap(string filename, BMSResource.Images resources)
        {
            //this.Content = ContentManager;
            //Filename = filename;
            if (filename == null)
                return;
            //texture = imageResources.ImageList[filename];
            //texture = contentManager.Load<Texture2D>(filename);
            texture = resources.getTexture2DByAssetName(filename);
            
            //ContentManager.Dispose();
        }
        /*
        public string getName()
        {
            return Filename;
        }
        */
        //public void setBitmap(string filename)
        //{
        //    texture = Content.Load<Texture2D>(filename);
        //}
    }
}
