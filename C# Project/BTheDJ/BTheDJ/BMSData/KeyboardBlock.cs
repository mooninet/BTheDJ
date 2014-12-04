using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace BTheDJ.BMSData
{
    // 기본적인 노트를 생성하는 클래스
    public class KeyboardBlock : Block
    {
        enum BLOCK_TYPE
        {
            White, Blue, Red
        };
        //SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;
        //ContentManager Content;
        SpriteFont font;
        private BLOCK_TYPE blockType = BLOCK_TYPE.White;
        //private Bitmap ImgWhite;
        //private Bitmap ImgBlue;
        //private Bitmap ImgRed;
        //private Texture2D ImgWhite;
        //private Texture2D ImgBlue;
        private int WavNumber;
        private int keyboard;
        public int Index
        {
            get { return WavNumber; }
            set { WavNumber = value; }
        }
        public int Keyboard
        {
            get { return keyboard; }
            set { keyboard = value; }
        }
        public void SetWhite() { blockType = BLOCK_TYPE.White; }
        public void SetBlue() { blockType = BLOCK_TYPE.Blue; }
        public void SetRed() { blockType = BLOCK_TYPE.Red; }

        //public KeyboardBlock(BMSPlayer player)
        public KeyboardBlock(BMSPlayer player)
            //:base(spriteBatch, graphics)
            :base(player)
        {
            refBMSPlayer = player;
            //spriteBatch = spriteBatch;
            this.font = player.spriteFont;
            //this.graphics = graphics;
            //this.Content = contentManager;
            // 노트에 키보드이미지를 등록한다.
            // Bitmap클래스 타입의 데이터
            // 문제발생 리소스 사용불가.
            // ContentManager를 통째로 매개변수로 받아 사용하자.
            //ImgWhite = new Bitmap("Image/white", refBMSPlayer.objBitmapResource);
            //ImgBlue = new Bitmap("Image/blue", refBMSPlayer.objBitmapResource);
            //ImgWhite = refBMSPlayer.objBitmapResource.getTexture2DByAssetName("Image/bmsPlayer/white");
            //ImgBlue = refBMSPlayer.objBitmapResource.getTexture2DByAssetName("Image/bmsPlayer/blue");
        }

        public override void Update() // block 클래스를 상속받았기 때문에 override
        {
            // 2.0f만큼씩 노트들을 내린다. 나중에 배속조절을 할때 수정해야함
            //base.MoveDown(3.75f);
            // 판정선 위치에 도달했고 동작되지 않은 상태일 경우에
            if (isPlayed == false
                && (y <= (float)(440 + (refBMSPlayer.RANK+5)) && y >= (float)(440 - (refBMSPlayer.RANK+5)))) // 판정선의 위치->난이도를 결정
            {
                // 동작가능한 상태일 경우(키보드입력처리)
                if (bplay)
                {
                    // 해당 위치에 도달하면 노트가 가지고 있는 Index를 참조하여 wav리스트안의 음원을 재생한다
                    //bplay = false;
                    // 정상적으로 노트가 작동하면
                    isPlayed = true;
                    // 콤보상태 ON
                    //refBMSPlayer.isCombo = true;
                    // 음원 출력
                    refBMSPlayer.PlayWav(Index);
                    Vector2 pos = new Vector2(base.X, base.Y);

                    //if (!refBMSPlayer.autoMode)
                    //{
                        refBMSPlayer.Combo++;
                        if (refBMSPlayer.Combo / 10 >= 10)
                            refBMSPlayer.Score += 200;
                        else if (refBMSPlayer.Combo / 10 >= 4)
                            refBMSPlayer.Score += 100;
                        else if (refBMSPlayer.Combo / 10 >= 3)
                            refBMSPlayer.Score += 80;
                        else if (refBMSPlayer.Combo / 10 >= 2)
                            refBMSPlayer.Score += 60;
                        else
                            refBMSPlayer.Score += 40;
                    //}
                    // 노트가 연주되고나면 해당 노트객체를 삭제를 해볼까...?
                    //this.Dispose(true);
                }
            }
                // 만약 작동되지않았으면서 판정선을 넘어가게된다면 콤보상태를 끊어버린다.
            if (isPlayed == false 
                && (y <= (float)450 && y >= (float)440))
            {
                //refBMSPlayer.isCombo = false;
                refBMSPlayer.Combo = 0;
            }
        }

        public override void Render()
        {
            if ((y >= 0 && y <= 450) && isPlayed == false) // 0이상 450이하(구동기의 높이)일 경우만 렌더링. 그리고 동작하지 않은 상태여야 한다
            {
                Vector2 pos = new Vector2(X, Y-10);
                //Vector2 pos1 = new Vector2(this.keyboard * 50, 400);
                Vector2 pos1 = new Vector2(100, 100);
                //Vector2 pos2 = new Vector2(X, Y);
                //float distance = Vector2.Distance(pos, pos2);
                switch (blockType)
                {
                    case BLOCK_TYPE.White:
                        // 흰색 노트를 출력
                        spriteBatch.Begin();
                        spriteBatch.Draw(refBMSPlayer.objBitmapResource_bmsPlayer.getTexture2DByAssetName("white"), pos, null, Color.White, 0, Vector2.Zero, new Vector2(0.55f, 0.7f), SpriteEffects.None, 1);
                        if (refBMSPlayer.debugMode)
                        {
                            spriteBatch.DrawString(font, Convert.ToString(Index), new Vector2(X, Y), Color.White);
                            spriteBatch.DrawString(font, this.bplay.ToString(), new Vector2(X, Y + 15), Color.White);
                            spriteBatch.DrawString(font, this.isPlayed.ToString(), new Vector2(X, Y + 30), Color.White);
                        }
                        spriteBatch.End();
                        
                        break;
                    case BLOCK_TYPE.Blue:
                        // 파란 노트를 출력
                        spriteBatch.Begin();
                        spriteBatch.Draw(refBMSPlayer.objBitmapResource_bmsPlayer.getTexture2DByAssetName("blue"), pos, null, Color.SkyBlue, 0, Vector2.Zero, new Vector2(0.55f, 0.7f), SpriteEffects.None, 1);
                        if (refBMSPlayer.debugMode)
                        {
                            spriteBatch.DrawString(font, Convert.ToString(Index), new Vector2(X, Y), Color.White);
                            spriteBatch.DrawString(font, this.bplay.ToString(), new Vector2(X, Y + 15), Color.White);
                            spriteBatch.DrawString(font, this.isPlayed.ToString(), new Vector2(X, Y + 30), Color.White);
                        }
                        spriteBatch.End();
                        //spriteBatch.Dispose();
                        break;
                    //case BLOCK_TYPE.Red:
                        // 빨간 노트를 출력
                        //spriteBatch.Begin();
                        //spriteBatch.Draw(ImgRed.TEXTURE, pos, size, Color.Pink);
                        //spriteBatch.End();
                        //break;
                }
            }
        }
    }
}