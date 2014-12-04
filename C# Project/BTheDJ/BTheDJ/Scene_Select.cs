using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public class Scene_Select : Microsoft.Xna.Framework.DrawableGameComponent
    {
        BTheDJ.BMSResource.Images objImageResource_Menu;
        SpriteBatch spriteBatch;
        SpriteFont font;
        ContentManager Content;
        GraphicsDeviceManager graphics;
        BMSData.BMSParser parser;
        public List<string> bmsFiles = new List<string> { }; // bms파일 이름을 넣는 리스트
        public List<string> bmsTitles = new List<string> { }; // 곡의 타이틀들을 넣는 리스트
        InputDelayManager delayUpDown;
        InputDelayManager delayEnter;
        InputDelayManager delayFunc;
        Game1 game;
        public int keyState = -1;        

        public Scene_Select(Game1 game)
            : base(game)
        {
            // TODO: 모든 파생 구성 요소를 이곳에서 생성하십시오.
            this.game = game;
            this.spriteBatch = game.spriteBatch;
            this.font = game.font;
            this.Content = game.Content;
            this.graphics = game.graphics;
            this.objImageResource_Menu = new BMSResource.Images(Content, "Menu");
            this.bmsFiles = game.bmsFiles;
            // 위아래 키보드입력에 딜레이를 부여하는 클래스.
            this.delayUpDown = new InputDelayManager(new Keys[] { Keys.Up, Keys.Down });
            this.delayEnter = new InputDelayManager(new Keys[] { Keys.Enter });
            this.delayFunc = new InputDelayManager(new Keys[] { Keys.F11 });

            this.parser = new BTheDJ.BMSData.BMSParser(new BMSPlayer(game));

            // bms파일들을 간단하게 파싱을 한 후 타이틀제목을 리스트에 넣는다.(출력을 하기 위함)
            for (int i = 0; i < bmsFiles.Count; i++)
            {
                parser.ParseHeader(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[i]);
                bmsTitles.Add(parser.refBMSPlayer.TITLE);
                // 초기화 하지 않고 같은 플레이어를 사용하면 내부 컬렉션에서 충돌이 일어나므로 꼭 초기화해야한다.
                parser.refBMSPlayer.Dispose();
            }
        }

        public void UpdateSongLists()
        {
            bmsTitles.Clear();
            game.bmsFiles.Clear();
            game.GetBmsFiles();
            bmsFiles = game.bmsFiles;
            // bms파일들을 간단하게 파싱을 한 후 타이틀제목을 리스트에 넣는다.(출력을 하기 위함)
            for (int i = 0; i < bmsFiles.Count; i++)
            {
                parser.ParseHeader(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[i]);
                bmsTitles.Add(parser.refBMSPlayer.TITLE);
                // 초기화 하지 않고 같은 플레이어를 사용하면 내부 컬렉션에서 충돌이 일어나므로 꼭 초기화해야한다.
                parser.refBMSPlayer.Dispose();
            }
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
            // 음악재생을 한다(포커스가 해당 곡으로 가 있을 경우)
            if(!delayUpDown.isDelay())
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    keyState--;
                    if (keyState < 0)
                        keyState = 0;
                    parser.refBMSPlayer.Dispose();
                    parser.Parse(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[keyState]);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    keyState++;
                    if (keyState > bmsTitles.Count - 1)
                        keyState = bmsTitles.Count - 1;
                    parser.refBMSPlayer.Dispose();
                    parser.Parse(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[keyState]);
                }
            }
            // 자동모드로 연주 시작
            parser.refBMSPlayer.autoMode = true;
            parser.refBMSPlayer.PlayMusic(gameTime);
            // 엔터키를 입력받으면 게임화면으로 넘어간다.
            if (!delayEnter.isDelay())
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (keyState > -1)
                    {
                        // game1의 포커스를 바꿔준다.
                        game.state = Game1.Focus.Scene_Play;
                        // dispose해주어야 샘플곡재생을 초기화할 수 있다.
                        parser.refBMSPlayer.Dispose();
                        // 선택곡을 파싱한다
                        game.mPlayer.bmsParser.Parse(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[keyState]);
                        game.mPlayer.Playing = true;
                    }
                }
            }
            if (!delayFunc.isDelay())
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F11))
                {
                    game.state = Game1.Focus.Scene_Edit;
                    parser.refBMSPlayer.Dispose();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // 곡 선택 화면을 출력한다.
            /*
             * parser 내부의 refBMSPlayer가 가지고 있는 정보를 바탕으로 출력을 한다
             * 그렇기 때문에 출력을 하기 전에 파싱을 먼저 해야 한다.
             */
            spriteBatch.Begin();
            // 디폴트 UI출력
            spriteBatch.Draw(objImageResource_Menu.getTexture2DByAssetName("defualt_FREE"), new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), new Rectangle(0, 0, 800, 600), Color.White);
            // 차례대로 정보를 출력한다.
            // 타이틀 출력
            for (int a = 0; a < bmsTitles.Count; a++)
            {
                spriteBatch.DrawString(font, bmsTitles[a], new Vector2(50, 105 + (a * 20)), Color.White);
            }
            // 장르 출력
            spriteBatch.DrawString(font, parser.refBMSPlayer.GENRE.ToString(), new Vector2(550, 269), Color.Black);
            // 아티스트 출력
            spriteBatch.DrawString(font, parser.refBMSPlayer.ARTIST.ToString(), new Vector2(550, 305), Color.Black);
            // BPM 출력
            spriteBatch.DrawString(font, parser.refBMSPlayer.BPM.ToString(), new Vector2(690, 375), Color.Black);
            // 노트 갯수 출력
            int notes = 0;
            for (int i = 0; i < parser.refBMSPlayer.BlockList.Count; i++)
            {
                if (parser.refBMSPlayer.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                    notes++;
            }
            spriteBatch.DrawString(font, notes.ToString(), new Vector2(690, 397), Color.Black);
            // 재생시간 출력
            float BarPerSecond = parser.refBMSPlayer.BPM / (4*60);
            int playtime = (int)(parser.refBMSPlayer.objBarMgr.Count / BarPerSecond);
            string playTime = "";
            if (playtime >= 60)
            {
                int m = playtime / 60;
                int s = playtime - (60 * m);
                playTime = m.ToString() + ":" + s.ToString();
            }
            spriteBatch.DrawString(font, playTime, new Vector2(690, 417), Color.Black);
            // 난이도 출력
            spriteBatch.DrawString(font, "LV . " + parser.refBMSPlayer.PLAYLEVEL.ToString(), new Vector2(10, 519), Color.Yellow);
            for (int j = 0; j < parser.refBMSPlayer.PLAYLEVEL; j++)
            {
                spriteBatch.Draw(this.objImageResource_Menu.getTexture2DByAssetName("defualt_FREE"), new Vector2(150 + (j * 25), 516), new Rectangle(380 + (j%7 * 30) + (j%7 * 1), 640, 29, 29), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
            // 선택 타일을 출력
            if (keyState > -1)
                spriteBatch.Draw(objImageResource_Menu.getTexture2DByAssetName("SelectedTile"), new Rectangle(40, 102 + (keyState * 20), 455, 22), null, new Color(new Vector4(1, 1, 1, 0.1f)));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
