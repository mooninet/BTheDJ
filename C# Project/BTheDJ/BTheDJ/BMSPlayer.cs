using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace BTheDJ
{
    public class BMSPlayer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // 
        public SpriteBatch spriteBatch;
        //GraphicsDeviceManager graphics;
        public SpriteFont spriteFont;
        ContentManager Content;
        public Game1 game;

        public bool Playing = false; // 진행상태 변수
        public bool autoMode = false; // 자동 연주 모드
        public bool debugMode = false; // 디버그모드
        public int Player = 1; // 플레이어(1P, 2P);
        public string GENRE = "none"; // 장르
        public string TITLE = "none"; // 곡명
        public string ARTIST = "none";
        public float BPM = 60; // 분당 비트 수
        public int PLAYLEVEL = 0; // 난이도
        public int RANK = 0; // 판정난이도
        public float BarHeight = 1000; // 마디 높이
        public float SPEED = 1;
        public int Combo = 0; // 콤보 수
        public int Score = 0; // 점수
        //public bool isCombo = true; // 콤보 가능한 상태를 나타내는 변수
        public int frames; // 총 진행된 프레임 수
        public double playtime; // 한 곡당 진행한 시간.
        //public BMSData.BitmapList bgBitmapList = new BMSData.BitmapList();
        public BMSData.BMSParser bmsParser; // 파서
        public BMSData.WavList objWavList; // 웨이브리스트
        public BMSData.BitmapList objBitMapList; // 비트맵리스트
        public BMSData.Bitmap bgBitmap; // 한 장 짜리 비트맵변수
        public BMSData.BarMgr objBarMgr; // 바 매니저
        public BMSResource.Images objBitmapResource_bmsPlayer; // 이미지 리소스 자원
        public List<BMSData.Block> BlockList; // 노트리스트
        public NoteEffect objNoteEffect1;
        InputDelayManager delayFunctionKey;
        InputDelayManager delayKeyboard;

        public BMSPlayer(Game1 game)
            :base(game)
        {
            /*
             * 플레이어를 생성하면 맴버변수에 데이터를 집어넣는다..
             */
            //NoteNum = 3000;
            //objWavList = new BMSData.WavList(NoteNum);
            this.spriteBatch = game.spriteBatch;
            //this.graphics = graphics;
            this.spriteFont = game.font;
            this.Content = game.Content;
            this.game = game;
            bmsParser = new BMSData.BMSParser(this);
            objWavList = new BMSData.WavList(Content);
            objBarMgr = new BMSData.BarMgr(this);
            BlockList = new List<BMSData.Block> { };
            objBitmapResource_bmsPlayer = new BMSResource.Images(Content, "bmsPlayer");
            objBitMapList = new BMSData.BitmapList(objBitmapResource_bmsPlayer);
            delayFunctionKey = new InputDelayManager(new Keys[] { Keys.F1, Keys.F3, Keys.F4, Keys.P, Keys.F11, Keys.Escape });
            delayKeyboard = new InputDelayManager(new Keys[] { Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L }, 20);
        }

        protected override void LoadContent()
        {

            base.LoadContent();
        }

        public void AddBmp(int bitmapIndex, string filename)
        {
            // 비트맵리스트에 비트맵을 추가하는 루틴 추가
            //objBitMapList.AddBitmap(filename);
        }

        public void AddWav(int index, string filepath)
        {
            objWavList.AddWav(index, filepath);
        }

        public void AddBar(int BarNum, int ChannelFirst, int ChannelSecond, string data)
        {
            // BarMgr오브젝트의 AddBar메소드로 바를 추가함
            objBarMgr.AddBar(BarNum, ChannelFirst, ChannelSecond, data);
        }

        public void PlayWav(int Index)
        {
            objWavList.PlayWav(Index);
        }

        public void ApplybgBitmap(int Index)
        {
            bgBitmap = objBitMapList.bitmapList[Index];
        }

        public void AddBpmBlock(int bpm, float y) 
        {
           
        }
        public void AddBitmapBlock(int bitmapnum, float y)
        {
            // bitmapblock클래스는 block클래스를 상속받았기 때문에 가능
            BMSData.Block block = new BMSData.BitmapBlock(this);
            
            // 생성된 block 오브젝트의 index에 입력받은 bitmap인덱스를 넣어준다
            ((BMSData.BitmapBlock)block).Index = bitmapnum;

            // 그리고 시작 좌표를 입력해준다.
            block.X = 250;
            block.Y = y;
            // 그리고 BMSPlayer의 block타입 리스트에 추가.
            BlockList.Add(block);
        }
        public void AddWaveBlock(int wavNum, float y)
        {
            // 배경음 노트(건반)을 추가한다(사용자가 칠 수 없는 노트)
            BMSData.Block block = new BMSData.BgmBlock(this);
            // keyboard 맴버는 필요없다. 사용자로부터 입력받아 처리하는 부분이 아니기때문.
            ((BMSData.BgmBlock)block).Index = wavNum;
            block.X = 400;
            block.Y = y;
            BlockList.Add(block);
        }
        public void AddKeyboardBlock(int keyboard, int wavNum, float y) 
            // keyboard = 패널넘버(사용자채널), wavNum = 웨이브리스트 인덱스, y = 노트 좌표
        {
            //BMSData.Block block = new BMSData.KeyboardBlock(this);
            BMSData.Block block = new BMSData.KeyboardBlock(this);
            //BMSData.KeyboardBlock kblock = new BMSData.KeyboardBlock();
            //BMSData.Block block = kblock;

            ((BMSData.KeyboardBlock)block).Index = wavNum;
            ((BMSData.KeyboardBlock)block).Keyboard = keyboard;
            //int a = kblock.Index;
            switch (keyboard)
            {
                case 1:
                    block.X = 18;
                    ((BMSData.KeyboardBlock)block).SetWhite();
                    break;
                case 2:
                    block.X = 55;
                    ((BMSData.KeyboardBlock)block).SetBlue();
                    break;
                case 3:
                    block.X = 92;
                    ((BMSData.KeyboardBlock)block).SetWhite();
                    break;
                case 4:
                    block.X = 136;
                    ((BMSData.KeyboardBlock)block).SetWhite();
                    break;
                case 5:
                    block.X = 173;
                    ((BMSData.KeyboardBlock)block).SetBlue();
                    break;
                case 6:
                    block.X = 210;
                    ((BMSData.KeyboardBlock)block).SetWhite();
                    break;
            }
            block.Y = y;
            BlockList.Add(block); // BMSPlayer의 블럭리스트에 등록   
        }
        public void PlayMusic(GameTime gameTime)
        {
            // 노래 재생만 하는 함수.(곡선택 화면에서 샘플곡 재생을 위한 함수)
            double BarPerSecond = BPM / (4 * 60); // 초당 마디 수
            double dyt = (BarHeight * BarPerSecond) * gameTime.ElapsedGameTime.TotalSeconds; // 프레임당 움직일 바의 y좌표 = 노트가 움직일 거리

            if (this.autoMode)
            {
                for (int i = 0; i < this.BlockList.Count; i++)
                {
                    if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                        ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                }
            }

            for (int i = 0; i < BlockList.Count; i++)
            {
                // 블럭의 좌표를 내리고 업데이트.
                BlockList[i].MoveDown((float)dyt);
                BlockList[i].Update();
            }
        }

        // Update메소드 구현.
        public override void Update(GameTime gameTime)
        {
            // 키보드 입력 처리 부분
            //----------------------------------------------------------------------------------------------------
            // 현재 부분은 원터치로 작동하는 키보드 입력처리 부분
            //----------------------------------------------------------------------------------------------------
            if (!delayFunctionKey.isDelay())
            {
                Keys[] keys = Keyboard.GetState().GetPressedKeys();
                for (int i = 0; i < keys.Length; i++)
                {
                    switch (keys[i])
                    {
                        // 알고리즘상 오토기능은 미구현.-> 구현완료.
                        case Keys.F1:
                            this.autoMode = !this.autoMode;
                            break;
                        case Keys.F4:
                            //mPlayer.BarHeight = 2400; // 배속은 구현 미구현.
                            this.ChangeSpeedUp();
                            break;
                        case Keys.F3:
                            //mPlayer.BarHeight = 600;
                            this.ChangeSpeedDown();
                            break;
                        // 일시정지
                        case Keys.P:
                            this.Playing = !this.Playing;
                            break;
                        case Keys.F11:
                            this.debugMode = !this.debugMode;
                            break;
                        case Keys.Escape:
                            game.state = Game1.Focus.Scene_Select;
                            this.Dispose();
                            break;
                    }
                }
            }
            //--------------------------------------------------------------------------------------------------
            // 현재 부분은 지속적인 키입력을 받는 것도 처리 가능
            //---------------------------------------------------------------------------------------------------
            // 일정 키보드값을 입력 받았을 때 블럭리스트 전체를 검사해서 해당 블럭의 타입이 키보드블럭인 경우
            // keyboard변수값(사용자채널값)을 비교한다음 bplay값을 true(동작가능한상태)로 바꾸어준다.

            // 키를 떼면 다시 원래상태(false)로 돌려놓아야만 한다.
            // 오토모드가 아닐 경우
            // 한번 동작되고나면 자동으로 동작불가능으로 바꾸어 여러번 동작하는 것을 방지.
            if (!this.autoMode)
            {
                for (int i = 0; i < this.BlockList.Count; i++)
                {
                    if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                        ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = false;
                }
            }
            else if (this.autoMode)
            {
                for (int i = 0; i < this.BlockList.Count; i++)
                {
                    if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                        ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                }
            }
            if (!delayKeyboard.isDelay())
            {
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {

                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 1)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 2)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.F))
                {
                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 3)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.J))
                {
                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 4)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 5)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.L))
                {
                    for (int i = 0; i < this.BlockList.Count; i++)
                    {
                        if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                            if (((BMSData.KeyboardBlock)this.BlockList[i]).Keyboard == 6)
                                ((BMSData.KeyboardBlock)this.BlockList[i]).bplay = true;
                    }
                }
            }

            //-----------------------------------------------------------------------------------------------------
            // 블럭&마디선 좌표 업데이트부분
            if (Playing)
            {
                // 게임 진행시간
                playtime += gameTime.ElapsedGameTime.TotalSeconds;
                // BPM를 이용해서 배속을 구현한다.
                //BarHeight = BarHeight * SPEED;
                double BarPerSecond = BPM / (4 * 60); // 초당 마디 수
                double dyt = (BarHeight * BarPerSecond) * gameTime.ElapsedGameTime.TotalSeconds; // 프레임당 움직일 바의 y좌표 = 노트가 움직일 거리

                for (int i = 0; i < BlockList.Count; i++)
                {
                    // 블럭의 좌표를 내리고 업데이트.
                    BlockList[i].MoveDown((float)dyt);
                    BlockList[i].Update();
                }
                for (int i = 0; i < objBarMgr.BarList.Count; i++)
                {
                    // 마디선 좌표를 내린다
                    objBarMgr.BarList[i].MoveDown((float)dyt);
                }
            }
        }
        // Draw 메소드 구현
        public override void Draw(GameTime gameTime)
        {
            // 랜더링 할 때 마다 frames를 증가시켜 몇프레임 진행됬는지 체크
            if (Playing)
                frames++;
            //playtime += gameTime.ElapsedGameTime.TotalSeconds;
            // 블럭들을 그린다
            for (int i = 0; i < this.BlockList.Count; i++)
            {
                // 배경음 노트는 그리지않게한다
                if (!debugMode)
                    if (this.BlockList[i].GetType().ToString() == "BTheDJ.BMSData.BgmBlock")
                        continue;
                this.BlockList[i].Render();
            }
            // 마디선 출력
            objBarMgr.RenderLine();

            spriteBatch.Begin();
            // 판정선 출력.
            if (debugMode)
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("white"), new Vector2(15, 410), new Rectangle(5, 3, 5, 6), Color.Red, 0, Vector2.Zero, new Vector2(100, 1), SpriteEffects.None, 0);
            else
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("white"), new Vector2(15, 410), new Rectangle(5, 3, 5, 6), Color.Red, 0, Vector2.Zero, new Vector2(46, 1), SpriteEffects.None, 0);
            // 기어 출력
            spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(0, 430), new Rectangle(0, 0, 260, 120), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            // 에너지바 출력
            spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(0, 0), new Rectangle(891, 0, 15, 430), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(245, 0), new Rectangle(909, 0, 15, 430), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            
            // 키보드입력값에 따른 출력 
            // 알파블랜드를 적용시켜 투명하게 만들어보자
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(15, 430), new Rectangle(0, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(15, 15), new Rectangle(726, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(52, 430), new Rectangle(37, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(52, 15), new Rectangle(767, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            } 
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(89, 430), new Rectangle(0, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(89, 15), new Rectangle(726, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            } 
            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(133, 430), new Rectangle(0, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(133, 15), new Rectangle(726, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            } 
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(170, 430), new Rectangle(37, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(170, 15), new Rectangle(767, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            } 
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(207, 430), new Rectangle(0, 295, 35, 35), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(207, 15), new Rectangle(726, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 1);
            }
            // 오토모드일 경우에는 AUTO이미지 출력
            if (this.autoMode)
            {
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(15, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(52, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(89, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(133, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(170, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(this.objBitmapResource_bmsPlayer.getTexture2DByAssetName("GD_Platform"), new Vector2(207, 0), new Rectangle(850, 0, 38, 415), new Color(new Vector4(1, 1, 1, 0.1f)), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            //spriteBatch.DrawString(spriteFont, "FirstBarLineY = " + this.objBarMgr.BarList[0].BarY.ToString(), new Vector2(100, 240), Color.White);
            // if (((BMSData.KeyboardBlock)mPlayer.Blocks[mPlayer.Blocks.Count - 1]).bplay == false)
            // {
            /*
            for (int i = 0; i < mPlayer.Blocks.Count; i++)
            {
                mPlayer.Blocks[i].Render();
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "PLAYING", new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(font, "bplay = " + mPlayer.Playing.ToString(), new Vector2(0, 40), Color.White);
            spriteBatch.End();
            */
            // }
            // else
            // {
            //     spriteBatch.Begin();
            //      spriteBatch.DrawString(font, "FINISHED", new Vector2(0, 20), Color.White);
            //     spriteBatch.End();
            // }
            //spriteBatch.DrawString(spriteFont, "PLAYING", new Vector2(0, 0), Color.White);
            if (debugMode)
            {
                
                spriteBatch.DrawString(spriteFont, "Playing = " + this.Playing.ToString(), new Vector2(30, 0), Color.White);
                spriteBatch.DrawString(spriteFont, "AutoPlay = " + this.autoMode.ToString(), new Vector2(30, 15), Color.White);
                //for (int i = 0; i < bmsFiles.Count; i++)
                //{
                //    spriteBatch.DrawString(font, bmsFiles[i], pos * i * 3, Color.White);
                //}
                //spriteBatch.DrawString(font, Convert.ToString("objWavList.Count = " + mPlayer.objWavList.Count), pos * 5, Color.White);
                //spriteBatch.DrawString(font, Convert.ToString("ExtHexToInt(#WAV1O 7C#) = " + BMSUtil.ExtHexToInt("1O")), new Vector2(0,0), Color.White);
                //spriteBatch.DrawString(font, "mPlayer.Blocks = " + Convert.ToString(mPlayer.Blocks.Count), pos * 15, Color.White);
                //spriteBatch.DrawString(font, "mPlayer.Bars = " + Convert.ToString(mPlayer.objBarMgr.BarList.Count), pos * 20, Color.White);
                //spriteBatch.DrawString(font, "mPlayer.Blocks[0].index = " + Convert.ToString(mPlayer.Blocks[0]), new Vector2(0,0), Color.White);
                //for (int j = 0; j < mPlayer.objWavList.wavList.Count; j++)
                //    spriteBatch.DrawString(font, j + "=" + mPlayer.objWavList.wavList[j].Filepath, new Vector2(pos.X, j * (pos.Y) * 3), Color.White);
                spriteBatch.DrawString(spriteFont, "TotalFrames = " + frames.ToString(), new Vector2(300, 80), Color.White);
                //spriteBatch.DrawString(font, "FPS = " + ((frames / (gameTime.TotalGameTime.TotalSeconds / gameTime.ElapsedGameTime.TotalSeconds)) * 60).ToString(), new Vector2(100, 80), Color.White);
                spriteBatch.DrawString(spriteFont, "SPF = " + gameTime.ElapsedGameTime.TotalSeconds.ToString(), new Vector2(300, 120), Color.White);
                //spriteBatch.DrawString(spriteFont, "PlayTime = " + gameTime.TotalGameTime.TotalSeconds.ToString(), new Vector2(300, 100), Color.White);
                spriteBatch.DrawString(spriteFont, "PlayTime = " + playtime.ToString(), new Vector2(300, 100), Color.White);
                spriteBatch.DrawString(spriteFont, "TITLE = " + this.TITLE, new Vector2(300, 140), Color.White);
                spriteBatch.DrawString(spriteFont, "BPM = " + this.BPM.ToString(), new Vector2(300, 160), Color.White);
                spriteBatch.DrawString(spriteFont, "Blocks = " + this.BlockList.Count.ToString(), new Vector2(300, 180), Color.White);
                spriteBatch.DrawString(spriteFont, "WavList = " + this.objWavList.Count.ToString(), new Vector2(300, 200), Color.White);
                spriteBatch.DrawString(spriteFont, "BarMgr = " + this.objBarMgr.Count.ToString(), new Vector2(300, 220), Color.White);
                //spriteBatch.DrawString(font, mPlayer.Blocks[0].GetType().ToString(), new Vector2(300, 240), Color.White);
            }
            // 디버그정보가 아닌 기본정보 출력
            spriteBatch.DrawString(spriteFont, "COMBO = " + this.Combo.ToString(), new Vector2(80, 220), Color.White);
            spriteBatch.DrawString(spriteFont, "SCORE = " + this.Score.ToString(), new Vector2(80, 240), Color.White);
            spriteBatch.End();
        }
        // 재생되는 곡의 배속을 올린다
        /*
         * 배속처리를 할 때는 초당프레임수와 키를 입력받을때 당시의 상황을 고려해야한다...
         * 신중하게 처리해야함. 그렇지않으면 해당 메소드가 여러번 호출되어버린다
         *
        public void ChangeSpeedUp()
        {
            if (SPEED <= 5.0f)
                SPEED += 1.5f;
        }
        // 배속을 내림
        public void ChangeSpeedDown()
        {
            if (SPEED >= 1.5f)
                SPEED -= 1.5f;
        }
         */
        public void ChangeSpeedUp()
        {
            if (SPEED <= 5.0f)
                SPEED += 0.5f;
            BarHeight *= 1.5f;
            for (int i = 0; i < BlockList.Count - 1; i++)
            {
                if (BlockList[i].Y < 400)
                {
                    float a = BlockList[i].Y * 1.5f;
                    do { BlockList[i].Y += 0.01f; }
                    while (BlockList[i].Y < a);
                }
            }

        }
        // 배속을 내림
        public void ChangeSpeedDown()
        {
            if (SPEED >= 1.0f)
                SPEED -= 0.5f;
            BarHeight *= 0.5f;
            for (int i = 0; i < BlockList.Count - 1; i++)
            {
                if (BlockList[i].Y < 400)
                {
                    float a = BlockList[i].Y * 0.5f;
                    do { BlockList[i].Y -= 0.01f; }
                    while (BlockList[i].Y > a);
                }
            }
        }
        /*
        public BMSData.Block GetFirstNote(int keyboard)
        {
            // 키보드넘버를 매개변수로 받아서 첫번째 노트인지 알려주는 노트
            for(int i=0; i<this.Blocks.Count; i++)
            {
                if(this.Blocks[i].GetType().ToString() == "BTheDJ.BMSData.KeyboardBlock")
                    if(((BMSData.KeyboardBlock)this.Blocks[i]).Keyboard == keyboard)
                        if(((BMSData.KeyboardBlock)this.Blocks[i]).Y <= 400)

                        
            }
        }
         */
        protected override void Dispose(bool disposing)
        {
            // 해당 클래스가 소멸될 때 자동으로 이 함수가 호출이 된다...
            // bmsPlayer를 초기화한다..
            bmsParser = new BMSData.BMSParser(this);
            objWavList = new BMSData.WavList(Content);
            objBarMgr = new BMSData.BarMgr(this);
            BlockList = new List<BMSData.Block> { };
            // 게임을 종료됬을 때도 자동으로 호출이 되는데 그 때는 디바이스도 같이 소멸되는데 로드를 하면 안된다
            //objBitmapResource_bmsPlayer = new BMSResource.Images(Content, "bmsPlayer");
            //objBitMapList = new BMSData.BitmapList(objBitmapResource_bmsPlayer);
            autoMode = false;
            //Combo = 0;
            //Score = 0;
            frames = 0;
            playtime = 0;
            //this.Dispose(true);
            //objWavList.StopWavs();
            // 초기화 한 후에는 가비지컬렉터를 호출한다.
            base.Dispose(disposing);
            //GC.Collect();
        }
    }
}
