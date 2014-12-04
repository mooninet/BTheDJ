using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BTheDJ
{
    /// <summary>
    /// 이것은 IUpdateable을 구현하는 게임 구성 요소입니다.
    /// </summary>
    public class NoteEffect : Microsoft.Xna.Framework.DrawableGameComponent
    {
        int index;
        int keyboard;
        Game1 game;
        SpriteBatch spriteBatch;

        public NoteEffect(Game1 game, int keyboard)
            : base(game)
        {
            // TODO: 모든 파생 구성 요소를 이곳에서 생성하십시오.
            this.spriteBatch = game.spriteBatch;
            this.game = game;
            this.index = 0;
            this.keyboard = keyboard;
        }

        /// <summary>
        /// 게임 구성 요소를 실행하기 전에 게임 구성 요소에 필요한 모든 초기화를 실시할 수
        /// 있습니다. 여기서 필요한 서비스를 질의(쿼리)할 수 있으며 콘텐츠 또한 불러올 수 있습니다.
        /// </summary>
        public override void Initialize()
        {
            // TODO: 여기에 초기화 코드를 추가하십시오.

            base.Initialize();
        }

        /// <summary>
        /// 이렇게 하면 게임 구성 요소가 스스로 자신을 업데이트할 수 있습니다.
        /// </summary>
        /// <param name="gameTime">타이밍 값의 스냅샷을 제공합니다.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: 여기에 업데이트 코드를 추가하십시오.
            if (index <= 5)
                index++;
            if (index == 6)
                this.Dispose(true);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();
            spriteBatch.Draw(game.mPlayer.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(18 + (keyboard * 37), 410), new Rectangle(1+(index*96), 679, 96, 96), new Color(new Vector4(1, 1, 1, 0.1f)));
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            
            base.Dispose(disposing);
        }
    }
}
