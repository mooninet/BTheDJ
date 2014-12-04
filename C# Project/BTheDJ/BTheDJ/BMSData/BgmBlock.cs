using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BTheDJ.BMSData
{
    public class BgmBlock : Block
    {
        enum BLOCK_TYPE
        {
            Red
        };
        
        //SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;
        //ContentManager Content;
        SpriteFont font;
        private BLOCK_TYPE blockType = BLOCK_TYPE.Red;
        //private Bitmap ImgWhite;
        //private Bitmap ImgBlue;
        //private Bitmap ImgRed;
        //private Texture2D ImgWhite;
        private int WavNumber;
        //private int keyboard; // 이벤트 채널 노트기때문에 키보드넘버는 필요없다.
        public int Index
        {
            get { return WavNumber; }
            set { WavNumber = value; }
        }
        //public int Keyboard
        //{
        //    get { return keyboard; }
        //    set { keyboard = value; }
        //}
        //public void SetWhite() { blockType = BLOCK_TYPE.White; }
        //public void SetBlue() { blockType = BLOCK_TYPE.Blue; }
        public void SetRed() { blockType = BLOCK_TYPE.Red; }

        //public KeyboardBlock(BMSPlayer player)
        //public BgmBlock(BMSPlayer player, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, ContentManager contentManager)
        public BgmBlock(BMSPlayer player)
            :base(player)
            //:base(spriteBatch, graphics)
        {
            refBMSPlayer = player;
            this.font = player.spriteFont;
            //this.spriteBatch = spriteBatch;
            //this.graphics = graphics;
            //this.Content = contentManager;
            //refBMSPlayer = player; // Block클래스를 상속받기때문에 구지 BMS플레이어를 매개변수로 넘겨받을 필요없음.
            // 노트에 키보드이미지를 등록한다.
            // Bitmap클래스 타입의 데이터
            // 문제발생 리소스 사용불가.
            // ContentManager를 통째로 매개변수로 받아 사용하자.
            //ImgWhite = new Bitmap("Image/white", refBMSPlayer.objBitmapResource);
            //ImgWhite = refBMSPlayer.objBitmapResource.getTexture2DByAssetName("Image/bmsPlayer/white");
            //ImgBlue = new Bitmap("Image/blue", contentManager);
        }

        public override void Update() // block 클래스를 상속받았기 때문에 override
        {
            // 2.0f만큼씩 노트들을 내린다. 나중에 배속조절을 할때 수정해야함
            //base.MoveDown(3.75f);
            // bplay가 false이면
            if (!bplay) // 배경음은 항상 재생되어야하게때문에 false인 경우는 재생
            {
                //if (y <= 400 + refBMSPlayer.RANK && y >= 400 - refBMSPlayer.RANK) // 해당 위치에 도달하면 노트가 가지고 있는 Index를 참조하여 wav리스트안의 음원을 재생한다
                if(y >= 430)
                {
                    bplay = true;
                    // 음원 출력
                    refBMSPlayer.PlayWav(Index);
                    // 노트 연주시 출력 할 기타 이펙트도 추가해준다

                }
           }
        }

        public override void Render()
        {
            if (y >= 0 && y <= 420) // 0이상 400이하(구동기의 높이)일 경우만 렌더링
            {
                Vector2 pos = new Vector2(base.X, base.Y-10);
                Rectangle size = new Rectangle((int)pos.X, (int)pos.Y, 20, 10);
                switch (blockType)
                {
                    //case BLOCK_TYPE.White:
                        // 흰색 노트를 출력
                        //spriteBatch.Begin();
                        //spriteBatch.Draw(ImgWhite.TEXTURE, rect, null, Color.White, 0, Vector2.Zero, 0.45f, SpriteEffects.None, 1);
                        //spriteBatch.End();
                        //break;
                    //case BLOCK_TYPE.Blue:
                        // 파란 노트를 출력
                        //spriteBatch.Begin();
                        //spriteBatch.Draw(ImgBlue.TEXTURE, rect, null, Color.SkyBlue, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                        //spriteBatch.End();
                        //break;
                        
                    case BLOCK_TYPE.Red:
                        // 빨간 노트를 출력
                        spriteBatch.Begin();
                        //spriteBatch.Draw(ImgWhite.TEXTURE, pos, null, Color.Pink, 0, Vector2.Zero, 0.45f, SpriteEffects.None, 1);
                        spriteBatch.Draw(refBMSPlayer.objBitmapResource_bmsPlayer.getTexture2DByAssetName("white"), pos, null, Color.Pink, 0, Vector2.Zero, new Vector2(0.5f, 0.7f), SpriteEffects.None, 1);
                        spriteBatch.DrawString(font, Convert.ToString(Index), new Vector2(X + 30, Y), Color.White);
                        spriteBatch.DrawString(font, this.bplay.ToString(), new Vector2(X + 30, Y + 15), Color.White);
                        spriteBatch.DrawString(font, this.isPlayed.ToString(), new Vector2(X + 30, Y + 30), Color.White);
                        spriteBatch.End();
                        //spriteBatch.Dispose();
                        break;
                         
                }
            }
        }
    }
}
