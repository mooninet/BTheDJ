using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BTheDJ.BMSData
{
    public class BarMgr
    {
        public BMSPlayer GetBMSPlayer;
        public List<Bar> BarList = new List<Bar> { };
        public BarMgr(Game1 game)
        {
            GetBMSPlayer = game.mPlayer;
        }
        public BarMgr(BMSPlayer player)
        {
            GetBMSPlayer = player;
        }
        public int Count
        {
            get { return BarList.Count; }
            set { }
        }
        public void AddBar(int barNum, int channelFirst, int channelSecond, string data)
        {
            // 바가 이미 있을 경우는 키만을 추가한다
            for(int i=0; i<BarList.Count; i++)
            {
                if (BarList[i].BarNumber == barNum)
                {
                    // 바에 키를 추가.
                    BarList[i].AddKey(channelFirst, channelSecond, data);
                    return;
                }
            }
            // 바가 없다면 바를 추가하고 키를 추가한다
            Bar bar = new Bar(this);
            bar.BarNumber = barNum;
            // 마디선의 좌표를 지정한다
            if (bar.BarNumber == 0)
                bar.BarY = -GetBMSPlayer.BarHeight;
            else
                bar.BarY = -GetBMSPlayer.BarHeight * bar.BarNumber;
            // 바에 키를 추가한다.
            bar.AddKey(channelFirst, channelSecond, data);
            // 생성된 바를 바 리스트에 등록.
            BarList.Add(bar);
        }
        public void RenderLine()
        {
            for (int i = 0; i <this.BarList.Count; i++)
            {
                // 0이상 400 이하만 출력
                if (BarList[i].BarY >= 0 && BarList[i].BarY <= 450)
                {
                    Vector2 startVector = new Vector2(15, BarList[i].BarY);
                    Rectangle rect = new Rectangle(5, 3, 5, 4);
                    // 마디선 출력.
                    GetBMSPlayer.spriteBatch.Begin();
                    GetBMSPlayer.spriteBatch.Draw(GetBMSPlayer.objBitmapResource_bmsPlayer.getTexture2DByAssetName("white"), startVector, rect, Color.White, 0, Vector2.Zero, new Vector2(47, 0.3f), SpriteEffects.None, 0);
                    GetBMSPlayer.spriteBatch.End();
                }
            }
        }
    }
}
