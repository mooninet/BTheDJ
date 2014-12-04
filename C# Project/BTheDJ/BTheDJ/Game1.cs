using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
/*
 * 14.09.30 9:35 AM
 * 현재 신텍스 에러는 발생하지 않는 것을 확인. 클래스 설계도에 따른 클래스의 전체 구성은 완성하였음.
 * 아직 미완성된 부분이 2~30%정도 된다.
 * UI부분은 거의 만들지않았음.
 * 창 핸들 생성에 문제가 생겼댄다 이걸 어떻한다
 *
 * 14.10.01 00:51 AM
 * 메모리부분(프로세스user개체)의 문제를 인지하고 출력부분을 조정을 해야됨.
 * 성공적으로 BMSPlayer의 block리스트에 등록까지는 된다. 출력만하면 이미지를 클라이언트에 찍을 수 있을 것이다.
 * 1.블럭의 맴버를 조절을 해서 메모리문제를 해결하자
 * -> Content를 넘겨줄 것이 아니라 Game.1에서 로드를 다 한다음 그 데이터만 넘겨주자(객체로)
 * 2.사운드 출력을 SoundEffect로 수정해야 게임이 가능하다. 이 클래스를 쓰도록 바꾸자(현재 Uri클래스)
 * 3.노래가 비정상적으로 톤이 높다..인덱스가 잘못된 듯 하다
 * 
 * 14.10.02 04:57 AM
 * 클래스를 직접 만들어 사용했었는데 drawablegamecomponent클래스를 상속받으면 또 그릴 수 있나본데...
 * 컴포넌트를 만들어보자.
 * 
 * 14.10.06 02:22 AM
 * 곡 진행 시 음정이 이상한 문제(불협화음)를 해결하였다. 웨이브리스트에 적재하는 중간에 문제가 있었다.
 * 원래는 bms데이터 안에 wav들은 순서대로 들어가있는줄 알았는데 그게 아니었다.
 * 배열에 넣어야 할 데이터들인데 리스트에 억지로 넣다보니 실제 인덱스와 파싱해온 데이터중 웨이브파일의 인덱스가
 * 어긋나서 생긴 문제였다. C#의 딕셔너리를 활용하여 해결하였음...
 * 
 * 배속시스템을 구현하였다. bpm과 업데이트되는 시간주기 등을 이용하여 처리함. bmsplayer의 speed변수만 수정해주면
 * 알아서 바의 높이도 조절한다.-> 실패. 다시해야함. 1배속만 가능
 * 
 * 오버헤드 문제를 해결하던 중 BGMBlock의 생성에 문제가 생긴 듯 하다.. 배경음이 나오지 않는다.
 * 노트자체도 만들어지지않고 소리자체도 안나온다. 등록할 때 문제가 생긴것같다.
 * 
 * 블럭을 생성할 때 BMSPlayer를 매개변수로 넘겨받으면 구지 스프라이트종류나 컨텐츠매니저를 넘겨줄 필요가 없을듯하다.
 * 수정하자. => 오버헤드를 발생시킴.
 * 
 * 현재 BGMBlock생성문제를 해결중
 * -> 02:45 AM 현재 해결완료. 리소스 로드문제였음. 스트링 하나가 잘못되어있었다. 
 *    이것으로 알게 된 것은 리소스가 로드되지않으면 block리스트에 등록자체가 되지 않는다는 것이다.
 *    
 * 한 곡 연주가 끝나면 어떻게 할 것인지 처리해야한다.(마지막 노트가 동작했다면 끝난게 아닐까?)
 * 
 * 14.10.08
 * 컨텐츠매니저나 스트라이트들을 일일이 매개변수로 넘겨줄 것이 아니라 BMSPlayer안에 모두 맴버로 갖고있게 하고
 * 플레이어 자체를 넘겨주면 매개변수의 수를 줄일 수 있다.
 * 
 * 14.10.09 09:31 AM
 * 이미지파일들만 리소스클래스로 분류를 했는데 사운드파일들도 리소스클래스로 따로 분류를 해야할 듯 하다..
 * 
 * 14.10.10 09:59 AM
 * 1. 바의 가로선을 그리자 -> BMSPlayer가 그려야 한다
 * 2. 키보드 입력 기능을 구현하자
 * 3. 키보드 중복입력을 방지하는 기능을 구현하자
 *    (업데이트가 초당 60번씩 되어지는데 이에 맞춰 입력할 수 없기 때문. 게다가 꾹 누르고 있어버리면 모두 동작해버린다)
 * 
 * 키보드 입력 부분을 구현중. 어느정도는 되었으나 동작절차의 오버헤드가 크기때문에 제대로 작동하지않는다...
 * 키보드 입력 부분 코드를 간략화 시켜야 한다..리팩토링??
 * 작동은 하나 다른 느낌의 게임이 되어간다. 디제이맥스와는 다른.
 * 
 * 14.10.11
 * 1. 키보드입력을 받는 구조를 바꿔볼까. 현재는 노트의 bplay변수를 조정해서 해당 좌표에 도달하면 소리가 나게 되어있는데
 *    키를 입력받을 때 바로 소리가 나게 만들면 좀더 DJMAX스러워질 수 있을 것 같은데...
 *    일정 좌표에 도달한 노트의 인덱스를 가지고와서 노트에서 소리가 날 게 아니라 키에서 소리가 나게 해보자
 * 2. 키보드입력을 한번누르면 1번만 적용되게 해야한다
 * 
 * 14.10.12
 * 1. 키입력에 소리가 나게 할 것. 블럭클래스에서의 Playwav메소드를 BMSPlayer로 옮기자
 * 2. 노트를 받아치면 받아쳤는지 표시를 하자.
 * 3. 배속을 변경할 때 곡 재생을 방해해선 안되기때문에 처음으로 받아쳐야할 노트 기준으로 좌표를 변경해야한다
 *    -> 미루자...어렵다.
 *    
 * 14.10.15
 * 1. 오토모드를 구현한다.
 * 2. 플레이어 스킨을 출력한다(플레이어 이미지만..)
 * 3. 마디선을 그린다. -> 완료
 * 4. 키보드 인풋 딜레이 매니저를 추가한다.
 * 5. 발표자료 제작,.
 * 
 * 14.10.16
 * 1. 키보드에 임시로 딜레이변수를 넣어서 컨트롤한다.(수정해야함)
 * 2. 블럭마다 플래그변수를 넣어서 연주되었는지 되지않은상태인지 나타낸다.
 * 
 * 14.10.18
 * 1. 이미지 리소스 클래스를 생성할 때 폴더별로 로드가 가능하게 만듬.
 *    (클래스별로 서로 다른 폴더내의 리소스파일을 로드 가능)
 * 2. 노트를 받아치면 판정선밑으로는 출력되지않고 못받아치면 끝까지 떨어진다.
 * 3. 오토모드 구현 완료.
 * 4. 디버그모드 구현.(디버그 모드일 때만 스테이터스 출력)
 * 5. UI구현 때는 각 화면 클래스별로 Update()와 Render()를 구분해서 정리해놓도록하자..헛갈린다
 * 6. 왠지 모르지만 갑자기 리소스로딩 문제가 해결되었다...노트가 6천개도 로드가 된다?****
 *    곡 추가를 몇개 했었고...파일몇개 더 넣었을 뿐인데..? 아마 비트맵블럭 클래스를 지워버려서 그런 듯 하다...
 *    비트맵클럭 클래스는 리소스로딩부분을 수정하지 않았었다 아마도.
 *    
 * 14.10.19
 * 1. DrawableGameComponent를 상속받으면 각자 클래스마다 화면을 출력하게 할 수 있다.
 *    그리고 Game클래스를 매개변수로 받아서 생성을 하게 되는데 이 프로젝트에서는 Game1이 주가 되므로
 *    Game1을 넘겨주면 되겠다. DrawableGameComponent는 인터페이스로, Game1이 갖고있는 Sprite들이나
 *    Content들을 쓸 수 있게 해주는 것이다. 기존에는 이 인터페이스를 상속받지 않아서 매개변수로써
 *    필요한 것들을 일일이 넘겨주었어야 했다. 지금은 Game1만 넘겨주면 모든게 처리가능하다.
 *    (물론 Game1이 기준점일 때)
 * 2. 곡 선택화면 클래스 UI를 만들고 있다...
 * 3. UI부분 구현중...키보드 입력에 따라 여러 화면을 그리게 만들어야한다.
 * 4. 어느정도는 완료했는데 접근지시자 부분이나 클래스 생성부분이 지나치게 정돈되지못해서
 *    재생이 제대로 되지 않는다(소리가 뭍힌다 -> 한번에 재생할 수 있는 음원제한을 넘은 것 같다)
 *    예전엔 이렇게 않았다. -> 오버헤드가 커져서 판정선에 도달했을 때 정확히 재생을 할 수 없던 문제였다.
 * 5. 콤보 끊기 기능 구현완료. 곡 선택 화면 컴포넌트 구현 완료. 
 * 6. 노트 이펙트 컴포넌트를 구현중이다.
 * 7. 플레이어를 생성하고 컨트롤 할 때 매개변수를 뭘로 줄 것인지가 정말 중요하다...
 *    곡 선택 화면에서 샘플곡 재생은 parser안에 있는 플레이어로 재생을 하는데
 *    엔터키를 입력받아 플레이 화면으로 넘어갈 때는 game1객체의 mPlayer에 액세스해야한다...
 *    
 * 14.10.23
 * 1. 웨이브리스트에서 keyboard값에 분류하고 Y값이 400미만의 가장 아래쪽에 위치한 노트의 인덱스를
 *    즉시 반환하는 함수를 만들고 키보드입력값에 따라 리턴값으로 효과음을 내도록 만들자
 * 2. 판정을 여러개로 나누고 좀 더 쉽게 만들자
 * 3. 애니메이션 효과를 넣자. 
 * 
 * 14.11.08
 * 1. 에디터컴포넌트를 구현중. 윈폼연동은 성공. 그러니 키보드입력처리부분이 까다로울 듯 하다.
 *    애시당초 XNA는 콘솔게임개발을 위한 프레임워크이기때문에 윈폼같이 PC용 키보드입력처리가 되어있지않다
 *    그래서 XNA에 윈폼을 연동시키는게 아니라 윈폼에 XNA를 연동시키는 방식을 이용하는 듯 하다...
 * 2. 화면전환시 사운드재생을 중지시키기위해 SoundEffectInstance클래스를 이용하였는데 동시재생이 
 *    원활하지않다 그래서 음이 뭍히는 경우가 많이 생기는 듯 하다...
 *    => 웨이브리스트의 재생메소드를 수정하여 처리함. 웨이브객체는 soundEffect만 갖고있고
 *       재생할 때 인스턴스를 생성하여 재생하는걸로 수정(결론적으로는 소리를 바로 멈추게 할 순 없다)
 * 3. 윈폼에 XNA를 연동시키면서 현재 개발방향을 유지하는 방법을 생각해보자...
 */


namespace BTheDJ
{
    /// <summary>
    /// 이것은 사용자 게임의 주 형식입니다.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /*
         * 프로젝트(실행파일)의 절대경로 사이에 한글이나 띄워쓰기가 들어가있으면 절대 안된다...절대!!
         * Wav파일을 로드하는 부분에 있어서 XNA 파이프라인을 이용하지않고 절대경로를 활용하기때문에 인식이 불가능하다..
         * 그래서 그냥 바탕화면에 두면 속편하다. 프로젝트폴더 안에도 넣지 못한다
         * => SoundEffect클래스를 활용하도록 수정을 하였음.. 경로문제는 해결. 그대신 모든 음원파일을 파이프라인에
         *    포함시켜야 한다..
         */
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public List<string> bmsFiles = new List<string> { }; // bms파일들 이름을 넣는 리스트
        public SpriteFont font;
        public BMSPlayer mPlayer;
        public Scene_Select mSelect;
        public Scene_Editor mEdit;
        public GameTime gameTime;
        // 현재 포커싱중인 화면을 나타내는 변수
        public enum Focus { Scene_Select, Scene_Play, Scene_Result, Scene_Edit }
        //public Focus state = Focus.Scene_Select;
        public Focus state = Focus.Scene_Select;
        //BMSData.BMSParser mParser;
        //BMSData.BitmapList mBitmapList;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 550;
        }
        // BMS파일들을 가져오는 함수
        public void GetBmsFiles()
        {
            // 현재 위치 폴더 안의 파일들중 확장자가 bms인 파일들의 이름을 스트링타입 리스트에 저장하는 루틴 
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            foreach (DirectoryInfo d in dir.GetDirectories())
                if (d.Name == "Content")
                    foreach (DirectoryInfo d1 in d.GetDirectories())
                        if (d1.Name == "BMSFiles")
                        {
                            foreach (FileInfo f in d1.GetFiles())
                                //if (f.Extension == ".bms" || f.Extension == ".bml" || f.Extension == ".bme")
                                if (f.Extension == ".bms")
                                    bmsFiles.Add(f.Name);
                        }
        }
        /// <summary>
        /// 게임이 실행되기 전에 처리해야 할 모든 초기화를 실시할 수 있습니다.
        /// 게임은 여기서 필요한 서비스를 질의(쿼리)할 수 있으며 비그래픽
        /// 관련 콘텐츠 또한 로드할 수 있습니다. base.Initialize를 호출하면 모든 구성 요소를 열거함과 동시에
        /// 모두 초기화합니다.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: 여기에 사용자의 초기화 논리 추가
            // 이 프로젝트의 파이프라인에 추가한 bms파일들의 이름을 bmsFiles에 저장한다.
            /*
            string rootDir = "";
            string filename = "";
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                if (d.Name == "Content")
                    foreach (DirectoryInfo d1 in d.GetDirectories())
                        if (d1.Name == "BMSFiles")
                        {
                            rootDir = "";
                            rootDir += d1.Name + "/";
                            char[] sep = new char[1] { '.' };
                            string[] FileNames;
                            foreach (FileInfo f in d1.GetFiles())
                            {
                                FileNames = f.Name.Split(sep);
                                filename = rootDir + FileNames[0];
                                bmsFiles.Add(f.Name);
                                filename = "";
                            }
                        }
            }
             */
            GetBmsFiles();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent는 매 게임별로 한 번씩 호출되며 이 메서드에서
        /// 모든 사용자 콘텐츠를 로드합니다.
        /// </summary>
        protected override void LoadContent()
        {
            // 텍스쳐를 출력하는 데 사용할 수 있는 새 SpriteBatch를 생성하십시오.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: 여기서 this.Content를 이용해서 사용자 콘텐츠를 로드
            font = this.Content.Load<SpriteFont>("SpriteFont1");
            // mPlayer 생성
            mPlayer = new BMSPlayer(this);
            // mSelect 생성
            mSelect = new Scene_Select(this);
            mEdit = new Scene_Editor(this);
            // bmsFiles리스트중 n번째 파일을 파싱
            //bmsParser.Parse(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[1]);
            
            //mPlayer.bmsParser.Parse(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + bmsFiles[0]);
        }

        /// <summary>
        /// UnloadContent는 매 게임별로 한 번씩 호출되며 이 메서드에서
        /// 모든 사용자 콘텐츠를 언로드합니다.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: 여기서 모든 비ContentManager 콘텐츠를 언로드
            // 프로그램 종료 시 임시 덤프파일도 함께 삭제해준다
            FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
            if (tempfile.Exists)
                tempfile.Delete();
        }

        /// <summary>
        /// 게임이 레벨 업데이트, 충돌 감지, 입력 감지 및 오디오 재생과
        /// 같은 게임 논리 코드를 실행할 수 있습니다.
        /// </summary>
        /// <param name="gameTime">타이밍 값의 스냅샷을 제공합니다.</param>
        protected override void Update(GameTime gameTime)
        {
            // 게임이 종료할 수 있게 함
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: 여기에 업데이트 논리 추가

            this.gameTime = gameTime;
            //mPlayer.PlayWav(30);

            // 노트들의 y좌표 조정(Update)
            //if (((BMSData.KeyboardBlock)mPlayer.Blocks[mPlayer.Blocks.Count - 1]).bplay == false)
           // {
           //     mPlayer.Update(gameTime);
           // }            
            switch (state)
            {
                case Focus.Scene_Select:
                    mSelect.Update(gameTime);
                    break;
                case Focus.Scene_Play:
                    mPlayer.Update(gameTime);
                    break;  
                case Focus.Scene_Edit:
                    mEdit.Update(gameTime);
                    break;
            }
            //mSelect.Update(gameTime);
            //mPlayer.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// 게임이 스스로 갱신해야 할 때 이 메서드를 호출합니다.
        /// </summary>
        /// <param name="gameTime">타이밍 값의 스냅샷을 제공합니다.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: 여기에 그래픽 출력 코드를 추가하십시오.
            
            switch (state)
            {
                case Focus.Scene_Select:
                    mSelect.Draw(gameTime);
                    break;
                case Focus.Scene_Play:
                    mPlayer.Draw(gameTime);
                    break;
                case Focus.Scene_Edit:
                    mEdit.Draw(gameTime);
                    break;
            }
             
            //mSelect.Draw(gameTime);
            //mPlayer.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
