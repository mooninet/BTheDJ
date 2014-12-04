using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BTheDJ.BMSData
{
    // 하나하나의 블럭의 데이터를 갖고있는 추상클래스
    public abstract class Block
    {
        protected BMSPlayer refBMSPlayer;
        protected SpriteBatch spriteBatch;
        //protected GraphicsDeviceManager graphics;
        protected float x;
        protected float y;
        //public Block(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)  // 기본생성자
        public Block(BMSPlayer player)
        {
            this.spriteBatch = player.spriteBatch;
            //this.graphics = graphics;
            x = 0; 
        }
        //protected bool bplay = false;
        public bool bplay = false; // 해당 노트가 작동가능한 상태인지 나타내는 변수
        public bool isPlayed = false; // 노트가 작동되었는지 나타내는 변수
        public float X
        {
            get { return x; }
            set { this.x = value; }
        }

        public float Y
        {
            get { return y; }
            set { this.y = value; }
        }

        public void MoveDown(float dy)
        {
            if (y <= 500) // 플레이어의 최대Y좌표(= 구동기(출력상)의 밑바닥)이면 내리지않는다. = 쌓임
                // 노트를 500까지 내리긴 하지만 소리는 430에서 나게 한다.
            {
                y += dy;
            }
        }
        public virtual void Update() { }
        public virtual void Render() { }
    }
}
