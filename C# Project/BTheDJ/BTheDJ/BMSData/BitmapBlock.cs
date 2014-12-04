using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace BTheDJ.BMSData
{
    public class BitmapBlock : Block
    {
        private Bitmap ImgBitmap;
        private int bitNumber = 0;
        BMSPlayer refBMSPlayer;
        SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;
        //ContentManager Content;
        public int Index
        {
            get { return bitNumber; }
            set { bitNumber = value; }
        }

        public BitmapBlock(BMSPlayer player)
            :base(player)
            //:base(spriteBatch, graphics)
        {
            //this.Content = contentManager;
            ///refBMSPlayer = player; // 필요없다. Block 클래스를 상속받았기 때문.
            // 비트맵 키보드 이미지를 넣어준다(미작업)
            //ImgBitmap = new Bitmap();
            this.spriteBatch = player.spriteBatch;
        }
        public override void Update()
        {
            // bplay가 false이면
            if (!bplay)
            {
                if (y >= 300)
                {
                    bplay = true;
                    //refBMSPlayer.ApplyBitmap(bitNumber);
                }
            }
        }

        public override void Render()
        {
            if (y > 0 && y < 400)
            {
                Vector2 rect = new Vector2(X, Y);
                spriteBatch.Begin();
                // xna에서는 game1클래스 내부에서 파이프라인을 통해서만 컨텐츠를 로드할 수 있다...
                // 그래서 ContentManager, SpriteBatch를 매개변수로 받는다.
                spriteBatch.Draw(ImgBitmap.TEXTURE, rect, Color.White);
                spriteBatch.End();
                //spriteBatch.Dispose();
            }
        }
    }
}
