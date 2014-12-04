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
using System.Windows.Forms;

/*
 * 14.11.09
 * 1. 윈폼 컨트롤 동작처리를 구현하는중.
 *    XNA와 연동해서 처리를 하기때문에 키보드입력이 완벽하지않다..처리해야한다.
 * 2. 키보드입력값을 처리할 때 인풋딜레이매니저 내부의 키 배열에 들어있는 값만 리턴하게 처리해야한다
 * 3. 컨트롤들을 추가하는 중.
 * 4. 음원파일의 재생시간이 긴 것들은 다음 재생을 하여도 멈추지않아 소리가 중첩된다 중첩이되지않게 처리해야.
 * 5. 헤더부분 정보를 넣고 Accept시키는 버튼을 만들자. 
 *    => 예를들어 Bar같은 중요한 정보는 변경시킬 때 마다 큰 영향을 미치기 때문에 Accept한 다음에는
 *       더이상 정보를 입력받을 수 없게 컨트롤을 잠궈버리는게 좋은 듯 하다. 
 *       그리고 수정버튼을 누를 시 변경을 하되 Bar같은 경우는 메시지창을 띄워 정말 하겠냐는 창을 띄운다.
 * 6. 추가를 더 하자면 현재 짜여져있는 스크립트를 직접 띄워서 보여주는 스크립트 편집기도 만들면 좋을듯하다
 *    => 힘들 것 같다. 구지 한다면 만들어진 bms파일을 윈도상에서 실행시켜주는 정도?
 * 
 * 14.11.13
 * 1. 에디팅을 할 때 노트를 추가하거나 웨이브파일을 등록할 때 임시 BMS플레이어 스테이터스를 표시하자
 * 2. 텍스트박스에 키프레스 이벤트 핸들러를 추가해보았는데 일반 키는 이벤트자체가 발생하지않는다...
 *    => XNA자체적으로 윈도메시지를 중간에서 가로채서 처리하기때문에 컨트롤까지 메시지가 닿지않는다
 *    => 윈폼 컨트롤들은 XNA에 포함된부분이 아니기때문에 그런 듯 하다..
 *    
 * 14.11.18
 * 1. 마우스 좌표처리를 하였다.
 *    => 실제 클라이언트 영역 내에서의 마우스 좌표값을 받아들여서 자체 마우스커서를 저장하는 변수에 
 *       저장을 하는데, 필드 좌표평면 내에서의 절대좌표로 환산을 해서 저장을 한다.
 *       (마우스 휠값을 같이 계산한다)
 *       그래서 어떤 화면을 보고 있던지간에, 항상 필드좌표평면상에서의 마우스좌표를 얻어올 수 있게된다.
 *       또한 몇번째 박스위에 커서가 있는지도 계산이 가능하다.
 * 2. XNA와 윈폼을 연동을 하면서 네임스페이스 충돌문제가 심각하다.
 *    => XNA 자체가 닷넷환경 아래에서 만들어졌으며 SYSTEM 네임스페이스의 함수들을 재정의해서 활용하는 듯 하다...
 *       그래서 연동을 해서 작업을 진행할 때는 네임스페이스 충돌 문제를 정확하게 처리를 해야한다.
 * 3. 마우스위치추적업데이트 함수를 만들어서 커서가 필드박스(작업)영역 내에 있을때만 업데이트하는 기능을 구현해야한다
 *    => 그러려면 마우스위치정보를 갖는 구조체를 만들어야한다.
 *       현재 마우스커서위치, 몇번째 필드에 올라와있는지 기타 등등의 정보를 갖고 있어야한다.
 *    => 필드박스 영역을 관리하는 구조체나 클래스를 만들어야한다.
 *       필드박스의 갯수나 바의 갯수, 마디분할갯수 그리고 현재 보고있는 필드영역의 포커싱좌표 등등
 *    => 현재는 컴포넌트 내에서 전역변수로서 다 처리가 되어있는데 아주 더러운 코드이다..
 * 4. 필드박스 영역에서 선택된 박스의 위치를 데이터화 시키는데 콤보박스의 인덱스는 0부터 시작이다
 *    그러나 데이터화 시킬때는 1부터 시작하여야하기때문에 문제가 있었다.
 *    그리고 인덱스는 한자리수부터 계산을 하기때문에 강제로 앞자리에 0을 붙여주어야했다(스트링으로 캐스팅해서)
 * 5. 웨이브 자산을 콤보박스나 플레이어에 넣거나 뺄 때 인덱싱처리가 굉장히 까다로웠다..
 *    => 실제 스크립팅 될 때에는 웨이브인덱스가 1부터 시작해야 정상처리가 되기때문에 AddWav할때에도 인덱스를 1부터 주어서 등록을 했는데
 *       그럴 필요가 없다. 스크립팅 할때만 1부터 들어가도록 하면 된다.
 *       -> 왜냐하면 파싱을 해서 실제 플레이어에 등록이 될 때에는 알아서 스크립트의 정보대로만 저장을 하기때문에 구지 처리를 해 줄 필요가 없다.
 *          괜히 헛고생했다.
 * 6. 플레이어의 웨이브리스트에서 삭제연산을 할 경우 딕셔너리의 키값까지 조정을 해주어야한다. 그렇지않으면 인덱싱오류가 발생한다.
 * 7. 전체 노트와 마디를 삭제하는 버튼 추가. 그리고 판정선을 추가하였다.
 * 8. 오늘 처리해야할 것
 *    => 1. 중간에 마디분할을 바꿀경우 데이터를 그대로 넘겨주어야한다 -> 이부분도 처리해야...
 *       2. 스크롤바 추가 & 미니맵 추가 -> ..?
 *       3. 정지기능 추가 -> 약간 애매함..
 *       4. 스크립트로 출력기능 추가 -> 필드에 저장되어있는 값을 박스 단위로 데이터화 시켜서 스크립트로 출력한다
 *       5. 로딩기능 추가.-> 힘들 듯 하다...
 *       6. 리스트를 추가해서 선택된 박스가 어떤 음을 재생할지 정보를 갖고 있는다
 *       7. 36진수 변환기 기능을 추가한다(역으로 처리)
 *       8. PPT자료 & 매뉴얼 & UCC 제작
 * 9. 마우스 휠 컨트롤 처리를 하였다.
 * 10. 스크립트 출력 기능을 구현하였다. 그러나 아직 로딩은 되지 않는다. 세이브만 가능
 * 11. 정지를 시키면 스크립트 파싱 자체를 새로 하게 만들어볼까?
 * 12. 정지 기능을 구현.
 *     => 마우스 클릭을 통해 노트를 등록 할 때 마다 임시데이터를 루트디렉터리에 저장하고 매번 덮어써준다.
 *        플레이를 할 때 바로 그 데이터를 파싱해와서 플레이어에 등록 후 재생을 한다.
 *     => 구현완료.
 * 13. 등록된 노트의 파일명을 지정된 필드박스 위치에 출력시키자
 *     => 노트이름을 저장하는 어레이를 추가해서 노트를 등록할 때 마다 저장을 해서 출력하도록 처리하였음.
 * 14.11.20
 * 1. 버그발견. => 컨펌 버튼 뿐 아니라 플레이 버튼을 누를 때도 임시데이터 파일을 지워주어야 버그가 줄어든다
 *                 마지막 박스를 선택하면 인덱싱 오류가 발생한다.버그를 잡기위해 +1 해준 것이 문제이다
 *                 약간 치명적인 오류 발견. 임시데이터 처리에 있어서 중복의 오류가 발생...
 *                 -> dataStr을 클리어 하지 않아서 발생한 문제. 처리완료
 */
namespace BTheDJ
{
    /// <summary>
    /// 이것은 IUpdateable을 구현하는 게임 구성 요소입니다.
    /// </summary>
    public class Scene_Editor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // 출력을 위한 스프라이트와 폰트를 선언.
        SpriteBatch spriteBatch;
        SpriteFont font;
        ContentManager content;
        GraphicsDevice gd;
        GameTime gameTime;
        BMSResource.Images objImageResource_Edit; // menu
        BMSResource.Images objImageResource_Edit2; // player
        // 악보 선언
        bool[,] field;
        // 노트들의 이름을 등록하는 리스트.
        List<blockName> blockNames = new List<blockName> { };
        // 좌표와 이름을 갖는 노트이름 구조체
        struct blockName
        {
            public int x;
            public int y;
            public string name;
        }
        // 마디 선언
        int bars;
        // 마우스 휠 상태 변수
        int mouseWheel;
        int fbmouseX;
        int fbmouseY;
        // 판정선 좌표
        double judgeY = 0;
        // 저장했음을 나타내는 변수
        bool isConfirmed = false;
        // 파서 선언(임시 플레이어)
        BMSPlayer Player;
        BMSData.BarMgr oldBarMgr;
        List<BMSData.Block> oldBlockList;
        // 모든 음원자산들 보유한 웨이브리스트 선언
        BMSData.WavList objWavList;
        // 펑션키 딜레이매니저 선언
        InputDelayManager delayFunc;
        // 기본 키보드 입력 딜레이 매니저 선언
        InputDelayManager delayKeyboard; 
        // 이 컴포넌트 윈도 핸들 선언
        IntPtr handle;
        // 마디 나누기 상태
        enum BarDivState { div_4, div_8, div_16, div_32 };
        enum EditState { Playing, Pause, Stop };
        BarDivState barDivState;
        EditState state;
        // 노트데이터를 넣는 변수
        List<string> dataStr = new List<string> { };
        // 에러가 발생했음을 나타내는 플래그변수
        bool isError = false;
        // 에러메시지를 담는 변수
        string strError = "";
        // 사용자 메시지 출력
        bool isMessage = false;
        string strMessage = "";
        // 윈폼 패널 선언
        Panel pnl = new Panel();
        // 최상위 클래스 인스턴스 생성
        Game1 game;
        // 텍스트박스들을 선언한다
        TextBox tbox_Player;
        TextBox tbox_Genre;
        TextBox tbox_Title;
        TextBox tbox_Artist;
        TextBox tbox_BPM;
        TextBox tbox_PlayLevel;
        TextBox tbox_Rank;
        TextBox tbox_Bar;
        TextBox tbox_FileName;
        // 콤보박스들을 생성
        ComboBox cbox_Wav;
        ComboBox cbox_MyWav;
        // 버튼들을 생성
        Button btn_Bardiv;
        Button btn_Play;
        Button btn_Pause;
        Button btn_Stop;
        Button btn_PlayWav;
        Button btn_AddToMyWav;
        Button btn_SubMyWav;
        Button btn_PlayMyWav;
        Button btn_AllDelete;
        Button btn_Confirm;
        // 라벨들을 생성
        Label lb_FileName;
        Label lb_Player;
        Label lb_Genre;
        Label lb_Title;
        Label lb_Artist;
        Label lb_BPM;
        Label lb_PlayLevel;
        Label lb_Rank;
        Label lb_Wav;
        Label lb_Bar;
        Label lb_MyWav;
        Label lb_PlayTime;
        Label lb_HeaderLine;
        Label lb_DataLine;
        // 스크롤바 생성
        //ScrollBar scrollBar;

        public Scene_Editor(Game1 game)
            : base(game)
        {
            // TODO: 모든 파생 구성 요소를 이곳에서 생성하십시오.
            this.game = game;
            this.gd = game.GraphicsDevice;
            this.content = game.Content;
            this.objImageResource_Edit = new BMSResource.Images(content, "Menu");
            this.objImageResource_Edit2 = new BMSResource.Images(content, "bmsPlayer");
            // game 클래스에서 필요한 데이터를 가져와 이 컴포넌트에 저장한다.
            this.spriteBatch = game.spriteBatch;
            this.font = game.font;
            // 윈도 핸들을 갖고와서 저장
            this.handle = game.Window.Handle;
            // 파서와 백업용 플레이어 객체를 생성
            this.Player = new BMSPlayer(game);
            this.Player.Playing = false;
            oldBarMgr = new BMSData.BarMgr(new BMSPlayer(game));
            oldBlockList = new List<BMSData.Block> { };
            //this.oldPlayer = new BMSPlayer(game);
            //this.oldPlayer.Playing = false;
            // 임시 웨이브리스트를 생성
            this.objWavList = new BMSData.WavList(game.Content);
            // 컨트롤을 생성한다
            this.CreateControls();
            // 현재 바 나누기 상태를 지정해준다
            barDivState = BarDivState.div_4;
            state = EditState.Stop;
            bars = 10; // 디폴트는 4개의 마디
            // [9채널 + 2이벤트채널, 마디 수 * 바의 나누기 갯수] 디폴트값.
            // 한 화면에 4마디씩 출력한다
            field = new bool[11, bars * Convert.ToInt32(btn_Bardiv.Text)];
            // 윈폼 네임스페이스와 XNA 네임스페이스가 중복되서 상위클래스명까지 다 써주어야한다.
            delayFunc = new InputDelayManager
            (
                new Microsoft.Xna.Framework.Input.Keys[] 
                {
                    Microsoft.Xna.Framework.Input.Keys.F11,
                    Microsoft.Xna.Framework.Input.Keys.Escape,
                    Microsoft.Xna.Framework.Input.Keys.Enter,
                    Microsoft.Xna.Framework.Input.Keys.Space
                }
            );
            delayKeyboard = new InputDelayManager
            (
                new Microsoft.Xna.Framework.Input.Keys[]
                {
                    Microsoft.Xna.Framework.Input.Keys.Q,
                    Microsoft.Xna.Framework.Input.Keys.W,
                    Microsoft.Xna.Framework.Input.Keys.E,
                    Microsoft.Xna.Framework.Input.Keys.R,
                    Microsoft.Xna.Framework.Input.Keys.T,
                    Microsoft.Xna.Framework.Input.Keys.Y,
                    Microsoft.Xna.Framework.Input.Keys.U,
                    Microsoft.Xna.Framework.Input.Keys.I,
                    Microsoft.Xna.Framework.Input.Keys.O,
                    Microsoft.Xna.Framework.Input.Keys.P,
                    Microsoft.Xna.Framework.Input.Keys.A,
                    Microsoft.Xna.Framework.Input.Keys.S,
                    Microsoft.Xna.Framework.Input.Keys.D,
                    Microsoft.Xna.Framework.Input.Keys.F,
                    Microsoft.Xna.Framework.Input.Keys.G,
                    Microsoft.Xna.Framework.Input.Keys.H,
                    Microsoft.Xna.Framework.Input.Keys.J,
                    Microsoft.Xna.Framework.Input.Keys.K,
                    Microsoft.Xna.Framework.Input.Keys.L,
                    Microsoft.Xna.Framework.Input.Keys.Z,
                    Microsoft.Xna.Framework.Input.Keys.X,
                    Microsoft.Xna.Framework.Input.Keys.C,
                    Microsoft.Xna.Framework.Input.Keys.V,
                    Microsoft.Xna.Framework.Input.Keys.B,
                    Microsoft.Xna.Framework.Input.Keys.N,
                    Microsoft.Xna.Framework.Input.Keys.M,
                    Microsoft.Xna.Framework.Input.Keys.D0,
                    Microsoft.Xna.Framework.Input.Keys.D1,
                    Microsoft.Xna.Framework.Input.Keys.D2,
                    Microsoft.Xna.Framework.Input.Keys.D3,
                    Microsoft.Xna.Framework.Input.Keys.D4,
                    Microsoft.Xna.Framework.Input.Keys.D5,
                    Microsoft.Xna.Framework.Input.Keys.D6,
                    Microsoft.Xna.Framework.Input.Keys.D7,
                    Microsoft.Xna.Framework.Input.Keys.D8,
                    Microsoft.Xna.Framework.Input.Keys.D9,
                    Microsoft.Xna.Framework.Input.Keys.Delete,
                    Microsoft.Xna.Framework.Input.Keys.Right,
                    Microsoft.Xna.Framework.Input.Keys.Left
                }
            );
        }
        /// <summary>
        /// 게임 구성 요소를 실행하기 전에 게임 구성 요소에 필요한 모든 초기화를 실시할 수
        /// 있습니다. 여기서 필요한 서비스를 질의(쿼리)할 수 있으며 콘텐츠 또한 불러올 수 있습니다.
        /// </summary>
        public override void Initialize()
        {
            // TODO: 여기에 초기화 코드를 추가하십시오.
            // 플레이어의 진행상태를 멈춰놓아야 바로 진행되지 않고 플레이버튼을 눌렀을 때 진행된다.
            base.Initialize();
        }
        // =========================================================================================
        // 마우스 커서 위치에 따른 노트데이터를 생성해주는 함수
        // =========================================================================================
        // 현재는 전역변수로 처리되어있는데 리팩토링을 통해서 현재 마우스위치 구조체를 매개변수로 받도록 수정해야한다.
        string getDataForMouse(int fpositionX, int fpositionY)
        {
            string data = "";
            int clientx = gd.Viewport.Width;
            int clienty = gd.Viewport.Height;
            int boxheight = 30;
            int boxwidth = (clientx - 50) / 11;
            // ===================================================================================
            // 마디번호와 채널을 data에 추가.
            // ===================================================================================
            // 마디정보 추가.
            // 마디번호의 자릿수를 알아내야한다.1의자리 10의자리 100의자리인지.
            // 그리고 데이터를 쓸 때에는 36진수로 변환해서 써야한다..
            String barNum = ((fbmouseY / boxheight +1) / Convert.ToInt32(btn_Bardiv.Text)).ToString();
            switch (barNum.Length)
            {
                    // 한 자리 수 이면
                case 1:
                    data = "00" + (fbmouseY / boxheight / Convert.ToInt32(btn_Bardiv.Text)).ToString();
                    break;
                    // 두 자리 수 이면
                case 2:
                    data = "0" + (fbmouseY / boxheight / Convert.ToInt32(btn_Bardiv.Text)).ToString();
                    break;
                    // 세 자리 수 이면
                case 3:
                    data = "" + (fbmouseY / boxheight / Convert.ToInt32(btn_Bardiv.Text)).ToString();
                    break;
            }
            // 채널정보 추가.
            // 사용자채널
            if (fbmouseX / boxwidth < 9) // 필드박스 x축으로 9번째 까지이면(1~9)
                data += "1" + (fbmouseX / boxwidth + 1).ToString();
            // 배경음채널
            else if (fbmouseX / boxwidth > 8 && fbmouseX / boxwidth < 12) // 필드박스 x축으로 10번째 부터 11번째까지(10~11)
                data += "01";
            // 구분기호 ":" 추가.
            data += ":";
            // 마디위치 데이터를 data에 추가.
            for (int i = 0; i < Convert.ToInt32(btn_Bardiv.Text); i++)
            {
                // 마디분할갯수만큼 반복문을 돌리면서 인수가 해당 마디 내에서의 위치하는 박스에 도달하면
                if (i == fbmouseY / boxheight % Convert.ToInt32(btn_Bardiv.Text))
                {
                    string selectedIndex = "";
                    /*
                     * 원래는 1부터 9까지의 수를 입력받을 경우 한 자리 수로만 표현이 되는바람에
                     * 문제가 있었지만 BMSUtill의 캐스팅 함수를 이용해서 해결함.
                    if (cbox_MyWav.SelectedIndex == 0)
                        selectedIndex = "01";
                    else if (cbox_MyWav.SelectedIndex >= 1 && cbox_MyWav.SelectedIndex < 10)
                        selectedIndex = "0" + (cbox_MyWav.SelectedIndex + 1).ToString();
                    else
                     */
                        selectedIndex = BMSUtil.IntToExtHex(cbox_MyWav.SelectedIndex + 1);

                    data += selectedIndex;
                }
                else
                    data += "00";
            }
            return data;
        }
        // 컨트롤들을 생성하는 메소드
        private void CreateControls()
        {
            // 윈도 핸들을 가져와서 폼을 생성한다
            Form frm = Control.FromHandle(handle) as Form;
            // 폼에 이벤트핸들러를 통해서 이벤트함수들을 등록한다.
            // 다행히도 마우스이벤트는 정상동작을 한다.
            frm.MouseWheel += new MouseEventHandler(frm_MouseWheel);
            frm.MouseMove += new MouseEventHandler(frm_MouseMove);
            frm.MouseUp += new MouseEventHandler(frm_MouseUp);
            //frm.Paint += new PaintEventHandler(frm_Paint);
            // 패널 속성 설정
            this.pnl.Dock = DockStyle.Right;
            this.pnl.Width = 300;
            // 버튼들을 생성
            this.btn_Bardiv = new Button();
            this.btn_Bardiv.Location = new System.Drawing.Point(10, 10);
            this.btn_Bardiv.Width = 27;
            this.btn_Bardiv.Text = "4";
            this.btn_Bardiv.Click += new EventHandler(btn_Bardiv_Click);
            this.btn_Play = new Button();
            this.btn_Play.Location = new System.Drawing.Point(40, 10);
            this.btn_Play.Width = 27;
            this.btn_Play.Text = "Play";
            this.btn_Play.Click += new EventHandler(btn_Play_Click);
            this.btn_Pause = new Button();
            this.btn_Pause.Location = new System.Drawing.Point(70, 10);
            this.btn_Pause.Width = 27;
            this.btn_Pause.Text = "Pause";
            this.btn_Pause.Click += new EventHandler(btn_Pause_Click);
            this.btn_Stop = new Button();
            this.btn_Stop.Location = new System.Drawing.Point(100, 10);
            this.btn_Stop.Width = 27;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.Click += new EventHandler(btn_Stop_Click);
            this.btn_PlayWav = new Button();
            this.btn_PlayWav.Location = new System.Drawing.Point(220, 370);
            this.btn_PlayWav.Size = new System.Drawing.Size(new System.Drawing.Point(20, 20));
            this.btn_PlayWav.Text = "Play";
            this.btn_PlayWav.Click += new EventHandler(btn_PlayWav_Click);
            this.btn_AddToMyWav = new Button();
            this.btn_AddToMyWav.Location = new System.Drawing.Point(245, 370);
            this.btn_AddToMyWav.Size = new System.Drawing.Size(new System.Drawing.Point(20, 20));
            this.btn_AddToMyWav.Text = "+";
            this.btn_AddToMyWav.Click += new EventHandler(btn_AddToMyWav_Click);
            this.btn_SubMyWav = new Button();
            this.btn_SubMyWav.Size = new System.Drawing.Size(new System.Drawing.Point(20, 20));
            this.btn_SubMyWav.Text = "-";
            this.btn_SubMyWav.Location = new System.Drawing.Point(245, 400);
            //this.btn_SubMyWav.Click += new EventHandler(btn_SubMyWav_Click);
            this.btn_PlayMyWav = new Button();
            this.btn_PlayMyWav.Location = new System.Drawing.Point(220, 400);
            this.btn_PlayMyWav.Size = new System.Drawing.Size(new System.Drawing.Point(20, 20));
            this.btn_PlayMyWav.Text = "Play";
            this.btn_PlayMyWav.Click += new EventHandler(btn_PlayMyWav_Click);
            this.btn_AllDelete = new Button();
            this.btn_AllDelete.Location = new System.Drawing.Point(220, 310);
            this.btn_AllDelete.Size = new System.Drawing.Size(new System.Drawing.Point(20, 20));
            this.btn_AllDelete.Text = "D";
            this.btn_AllDelete.Click += new EventHandler(btn_AllDelete_Click);
            this.btn_Confirm = new Button();
            this.btn_Confirm.Location = new System.Drawing.Point(10, 500);
            this.btn_Confirm.Size = new System.Drawing.Size(new System.Drawing.Point(70, 30));
            this.btn_Confirm.Text = "Confirm";
            this.btn_Confirm.Click += new EventHandler(btn_Confirm_Click);
            // 라벨들을 생성
            this.lb_FileName = new Label();
            this.lb_FileName.Location = new System.Drawing.Point(10, 74);
            this.lb_FileName.Text = "FileName";
            this.lb_PlayTime = new Label();
            this.lb_PlayTime.Location = new System.Drawing.Point(140, 10);
            this.lb_PlayTime.Size = new System.Drawing.Size(60, 30);
            this.lb_PlayTime.Text = "PlayTime   " + "0:0";
            this.lb_Player = new Label();
            this.lb_Player.Location = new System.Drawing.Point(10, 104);
            this.lb_Player.Text = "Player";
            this.lb_Genre = new Label();
            this.lb_Genre.Location = new System.Drawing.Point(10, 134);
            this.lb_Genre.Text = "Genre";
            this.lb_Title = new Label();
            this.lb_Title.Location = new System.Drawing.Point(10, 164);
            this.lb_Title.Text = "Title";
            this.lb_Artist = new Label();
            this.lb_Artist.Location = new System.Drawing.Point(10, 194);
            this.lb_Artist.Text = "Artist";
            this.lb_BPM = new Label();
            this.lb_BPM.Location = new System.Drawing.Point(10, 224);
            this.lb_BPM.Text = "BPM";
            this.lb_PlayLevel = new Label();
            this.lb_PlayLevel.Location = new System.Drawing.Point(10, 254);
            this.lb_PlayLevel.Text = "PlayLevel";
            this.lb_Rank = new Label();
            this.lb_Rank.Location = new System.Drawing.Point(10, 284);
            this.lb_Rank.Text = "Rank";
            this.lb_Bar = new Label();
            this.lb_Bar.Location = new System.Drawing.Point(10, 314);
            this.lb_Bar.Text = "Bar";
            this.lb_Wav = new Label();
            this.lb_Wav.Location = new System.Drawing.Point(10, 374);
            this.lb_Wav.Text = "Wav";
            this.lb_MyWav = new Label();
            this.lb_MyWav.Location = new System.Drawing.Point(10, 404);
            this.lb_MyWav.Text = "MyWav";
            this.lb_HeaderLine = new Label();
            this.lb_HeaderLine.Location = new System.Drawing.Point(this.pnl.Width / 2 -20, 50);
            this.lb_HeaderLine.Text = "- Header -";
            this.lb_DataLine = new Label();
            this.lb_DataLine.Location = new System.Drawing.Point(this.pnl.Width / 2 - 20, 345);
            this.lb_DataLine.Text = "- Data -";
            // 텍스트박스들을 생성
            this.tbox_FileName = new TextBox();
            this.tbox_FileName.Width = 140;
            this.tbox_FileName.Location = new System.Drawing.Point(70, 70);
            this.tbox_FileName.Name = "tbFIleName";
            this.tbox_FileName.Text = "noname";
            this.tbox_Player = new TextBox();
            this.tbox_Player.Width = 140;
            this.tbox_Player.Location = new System.Drawing.Point(70, 100);
            this.tbox_Player.Name = "tbPlayer";
            this.tbox_Genre = new TextBox();
            this.tbox_Genre.Width = 140;
            this.tbox_Genre.Location = new System.Drawing.Point(70, 130);
            this.tbox_Genre.Name = "tbGenre";
            this.tbox_Title = new TextBox();
            this.tbox_Title.Width = 140;
            this.tbox_Title.Location = new System.Drawing.Point(70, 160);
            this.tbox_Title.Name = "tbTitle";
            this.tbox_Artist = new TextBox();
            this.tbox_Artist.Width = 140;
            this.tbox_Artist.Location = new System.Drawing.Point(70, 190);
            this.tbox_Artist.Name = "tbArtist";
            this.tbox_BPM = new TextBox();
            this.tbox_BPM.Width = 140;
            this.tbox_BPM.Location = new System.Drawing.Point(70, 220);
            this.tbox_BPM.Name = "tbBPM";
            this.tbox_PlayLevel = new TextBox();
            this.tbox_PlayLevel.Width = 140;
            this.tbox_PlayLevel.Location = new System.Drawing.Point(70, 250);
            this.tbox_PlayLevel.Name = "tbPlayLevel";
            this.tbox_Rank = new TextBox();
            this.tbox_Rank.Width = 140;
            this.tbox_Rank.Location = new System.Drawing.Point(70, 280);
            this.tbox_Rank.Name = "tbName";
            this.tbox_Bar = new TextBox();
            this.tbox_Bar.Width = 140;
            this.tbox_Bar.Location = new System.Drawing.Point(70, 310);
            this.tbox_Bar.Name = "tbBar";
            //this.tbox_Bar.KeyPress += new KeyPressEventHandler(tbox_Bar_KeyPress);
            // 콤보박스들을 생성
            this.cbox_Wav = new ComboBox();
            this.cbox_Wav.Width = 140;
            this.cbox_Wav.Location = new System.Drawing.Point(70, 370);
            this.cbox_MyWav = new ComboBox();
            this.cbox_MyWav.Width = 140;
            this.cbox_MyWav.Location = new System.Drawing.Point(70, 400);
            // 사운드이펙트 폴더 내의 파일(보유한 음원자산)의 갯수만큼 반복문들 돌려 
            // 현재 클래스의 임시 웨이브리스트에 등록. 그리고 콤보박스에 넣는다
            List<string> soundNames = new List<string> { };
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Content/SoundEffect/");
            // soundNames 리스트에 보유한 음원자산의 이름을 등록
            foreach (FileInfo f in d.GetFiles())
            {
                char[] seps = new char[1] { '.' };
                string[] filename = f.Name.Split(seps);
                soundNames.Add(filename[0]);
            }
            // soundNames의 데이터를 활용해 임시 웨이브리스트에 모든 음원자산을 등록
            for (int i = 0; i < soundNames.Count; i++)
            {
                objWavList.AddWav(i, "SoundEffect/" + soundNames[i]);
                // 자산의 이름을 지정
                objWavList.wavList[i].Wave.Name = soundNames[i];
                // 콤보박스에 SoundEffect자산을 등록하면 데이터는 잘 들어가는데
                // 박스를 열었을 때 데이터타입이 표시가되서 구별하기가 힘들다.
                // 어떻게 처리해야할지 고민해봐야함.
                // 1. 콤보박스를 2개 만들어서 자산이름을 넣은 콤보박스를 표시하고
                // 2. 데이터처리(샘플재생기능)은 실제 데이터(SoundEffect)를 넣은 콤보박스를 활용하는 방법.
                // 3. 콤보박스에는 음원자산 이름(스트링)만 넣고 콤보박스의 선택된 인덱스를 활용해 
                //    웨이브리스트에서 데이터를 가져와 활용하는 방법 <= 이 방법이 가장 적절해보인다.
                //cbox_Wav.Items.Add(objWavList.wavList[i].Wave);
                cbox_Wav.Items.Add(soundNames[i]);
            }
            cbox_Wav.SelectedIndex = 0;

            //tbox1.KeyPress += new KeyPressEventHandler(tbox1_KeyPress);
            // 패널에 컨트롤들을 등록
            this.pnl.Controls.Add(tbox_FileName);
            this.pnl.Controls.Add(tbox_Player);
            this.pnl.Controls.Add(tbox_Genre);
            this.pnl.Controls.Add(tbox_Title);
            this.pnl.Controls.Add(tbox_Artist);
            this.pnl.Controls.Add(tbox_BPM);
            this.pnl.Controls.Add(tbox_PlayLevel);
            this.pnl.Controls.Add(tbox_Rank);
            this.pnl.Controls.Add(tbox_Bar);
            this.pnl.Controls.Add(cbox_Wav);
            this.pnl.Controls.Add(cbox_MyWav);
            this.pnl.Controls.Add(btn_Bardiv);
            this.pnl.Controls.Add(btn_Play);
            this.pnl.Controls.Add(btn_Pause);
            this.pnl.Controls.Add(btn_Stop);
            this.pnl.Controls.Add(lb_FileName);
            this.pnl.Controls.Add(lb_PlayTime);
            this.pnl.Controls.Add(lb_Player);
            this.pnl.Controls.Add(lb_Genre);
            this.pnl.Controls.Add(lb_Title);
            this.pnl.Controls.Add(lb_Artist);
            this.pnl.Controls.Add(lb_BPM);
            this.pnl.Controls.Add(lb_PlayLevel);
            this.pnl.Controls.Add(lb_Rank);
            this.pnl.Controls.Add(lb_Wav);
            this.pnl.Controls.Add(lb_Bar);
            this.pnl.Controls.Add(lb_MyWav);
            this.pnl.Controls.Add(btn_PlayWav);
            this.pnl.Controls.Add(btn_PlayMyWav);
            this.pnl.Controls.Add(btn_AddToMyWav);
            this.pnl.Controls.Add(btn_SubMyWav);
            this.pnl.Controls.Add(btn_AllDelete);
            this.pnl.Controls.Add(btn_Confirm);
            this.pnl.Controls.Add(lb_HeaderLine);
            this.pnl.Controls.Add(lb_DataLine);
            // 패널을 프레임(폼)에 등록
            frm.Controls.Add(pnl);
            // 디폴트값으로는 패널을 숨긴다.
            this.pnl.Visible = false;    
        }
        /*
         * 키프레스 이벤트 자체가 발생하지 않아서 이 코드는 사용할 수 없다..
         * 
        public void tbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Microsoft.Xna.Framework.Input.Keys[] keys = Keyboard.GetState().GetPressedKeys();
            tbox1.Text += e.KeyChar
            
        }
         */
        // ==========================================================================
        // 마우스 이벤트 처리 함수
        // ==========================================================================
        // 마우스 휠 이벤트 처리 함수.
        private void frm_MouseWheel(object sender, MouseEventArgs e)
        {
            // 필드박스를 출력하는 좌표값이 클라이언트영역을 지나치게 벗어나버린경우
            // 휠값을 조정하지않도록 수정해야한다.

            mouseWheel += e.Delta / 12;
            //fbmouseY += mouseWheel;
            if (mouseWheel >= 10)
            {
                //int lastFbMouseY = fbmouseY;
                mouseWheel = 0;
                //fbmouseY += mouseWheel;
                //fbmouseY = lastFbMouseY;
            }
            else if (-mouseWheel + gd.Viewport.Height - 50 > 30 * bars * Convert.ToInt32(btn_Bardiv.Text))
            {
                mouseWheel = -(30 * bars * Convert.ToInt32(btn_Bardiv.Text) - gd.Viewport.Height + 50);
            }
        }

        private void frm_MouseMove(object sender, MouseEventArgs e)
        {
            fbmouseX = e.X-50;
            // 마우스휠 값을 좌표값에 포함시킴으로서 스크롤링이 가능하게 구현.
            fbmouseY = e.Y-50-mouseWheel;
        }

        private void frm_MouseUp(object sender, MouseEventArgs e)
        {
            int clientx = gd.Viewport.Width;
            int clienty = gd.Viewport.Height;
            int boxheight = 30;
            int boxwidth = (clientx - 50) / 11;
            // 마우스 입력에 따른 데이터를 필드박스처리를 하고 BMS포맷 데이터화 시킨다.
            // 선택된 웨이브자산이 있을 경우에만.
            if (cbox_MyWav.SelectedIndex != -1)
            {
                // 필드박스 영역이 50부터 시작이기때문에..
                if (e.X >= 50 && e.Button == MouseButtons.Left && field[fbmouseX / boxwidth, fbmouseY / boxheight] != true)
                {
                    field[fbmouseX / boxwidth, fbmouseY / boxheight] = true;
                    // 데이터화 시킨 정보를 dataStr에 저장
                    dataStr.Add(getDataForMouse(fbmouseX / boxwidth, fbmouseY / boxheight));
                    // dataStr을 활용해서 플레이어에 등록한다.
                    char[] seps = new char[1] { ':' };
                    string[] data = dataStr[dataStr.Count-1].Split(seps);
                    Player.AddBar(Convert.ToInt32(data[0].Substring(0, 3)), Convert.ToInt32(data[0].Substring(3, 1)), Convert.ToInt32(data[0].Substring(4, 1)), data[1]);
                    // 노트의 이름을 등록한다
                    blockName name = new blockName();
                    name.x = fbmouseX / boxwidth * boxwidth + 50;
                    name.y = fbmouseY / boxheight * boxheight + 50;
                    name.name = cbox_MyWav.Text;
                    blockNames.Add(name);
                    // 임시 데이터파일로 출력한다
                    FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
                    if (tempfile.Exists)
                        tempfile.Delete();
                    string temppath = Directory.GetCurrentDirectory() + "/temp.data";
                    ToScript(temppath, false);
                }
                /* 노트 삭제 기능은 미구현
                if (e.Button == MouseButtons.Right)
                    // 오른클릭을 하면 필드박스 데이터를 삭제
                    field[fbmouseX / boxwidth, fbmouseY / boxheight] = false;
                 
                if (e.Button == MouseButtons.Right)
                    dataStr = getDataForMouse(fbmouseX / boxwidth, fbmouseY / boxheight);
                 */
                isError = false;
                strError = "";
            }
            else
            {
                isError = true;
                strError = "-> There's no Wav resource in MyWav";
            }
        }

        // ==========================================================================
        // 버튼클릭 이벤트 처리 함수
        // ==========================================================================

        private void btn_Bardiv_Click(object sender, EventArgs e)
        {
            // 마디 나누기 버튼
            // 작업 도중에 마디를 변경할 경우 데이터소실 버그를 고쳐야한다.
            switch (barDivState)
            {
                case BarDivState.div_4:
                    btn_Bardiv.Text = "8";
                    barDivState = BarDivState.div_8;
                    judgeY = -60;
                    break;
                case BarDivState.div_8:
                    btn_Bardiv.Text = "16";
                    barDivState = BarDivState.div_16;
                    judgeY = -150;
                    break;
                case BarDivState.div_16:
                    btn_Bardiv.Text = "32";
                    barDivState = BarDivState.div_32;
                    judgeY = -350;
                    break;
                case BarDivState.div_32:
                    btn_Bardiv.Text = "4";
                    barDivState = BarDivState.div_4;
                    judgeY = 0;
                    break;
            }
            // 필드영역을 초기화한다.
            field = new bool[11, bars * Convert.ToInt32(btn_Bardiv.Text)];
        }

        private void btn_Play_Click(object sender, EventArgs e)
        {
            // 에디트 중인 곡을 재생시키는 버튼
            try
            {
                if (state == EditState.Pause || state == EditState.Stop)
                {
                    state = EditState.Playing;
                    FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
                    if (tempfile.Exists)
                        tempfile.Delete();
                    string temppath = Directory.GetCurrentDirectory() + "/temp.data";
                    ToScript(temppath, false);
                    // 데이터 백업
                    // 레퍼런스를 집어넣다보니까 이런 문제점이 생기는 듯 한데...값을 넣어야하는데
                    //this.oldPlayer = this.Player;
                    //oldBarMgr = Player.objBarMgr;
                    //oldBlockList = Player.BlockList;
                    // 플레이어의 재생상태와 오토모드를 설정 후 노래를 재생한다.
                    // 물론 이 컴포넌트의 업데이트 부분에서 플레이어의 업데이트함수가 있어야한다.
                    this.Player.Playing = true;
                    this.Player.autoMode = true;
                    this.Player.PlayMusic(game.gameTime);
                }
                isError = false;
            }
            catch (Exception e1)
            {
                isError = true;
                strError = "->" + e1.Source;
            }
        }

        private void btn_Pause_Click(object sender, EventArgs e)
        {
            // 에디트 중인 곡을 일시정지시키는 버튼
            try
            {
                if (state == EditState.Playing)
                {
                    state = EditState.Pause;
                    // 진행만 멈춘다
                    this.Player.Playing = false;
                }
                isError = false;
            }
            catch (Exception e1)
            {
                isError = true;
                strError = "->" + e1.Source;
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            // 에디트 중인 곡을 정지시키는 버튼
            try
            {
                state = EditState.Stop;
                // 정지버튼을 누르는 순간 백업된 플레이어를 임시 플레이어에 넣는다.
                this.Player.Playing = false;
                this.Player.autoMode = false;
                //this.Player = this.oldPlayer;
                //this.Player.objBarMgr = this.oldBarMgr;
                //this.Player.BlockList = this.oldBlockList;
                // 판정선 위치 리셋
                switch (barDivState)
                {
                    case BarDivState.div_4:
                        judgeY = 0;
                        break;
                    case BarDivState.div_8:
                        judgeY = -60;
                        break;
                    case BarDivState.div_16:
                        judgeY = -150;
                        break;
                    case BarDivState.div_32:
                        judgeY = -350;
                        break;
                }
                // 플레이어를 비우고 임시데이터를 파싱해온다.
                // 덤프파일이 없을 때 정지버튼을 누르게되면 웨이브리스트까지도 날라가버린다(인덱싱오류)
                // 치명적인 버그이다.
                Player.Dispose();
                Player.bmsParser.Parse(Directory.GetCurrentDirectory() + "/temp.data");
                isError = false;
            }
            catch(Exception e1)
            {
                isError = true;
                strError = "->" + e1.Source;
            }
        }

        private void btn_PlayWav_Click(object sender, EventArgs e)
        {
            // 콤보박스에서 선택된 음원을 재생하는 버튼
            try
            {
                // 음원파일의 재생시간이 긴 것들은 다음 재생을 하여도 멈추지않아 소리가 중첩된다
                // 처리해야함.
                // 정지이거나 일시정지 상태일 때만
                if (cbox_Wav.SelectedItem != null)
                    this.objWavList.PlayWav(cbox_Wav.SelectedIndex);
                // 플레이 버튼을 눌렀을 때도 덤프파일을 지워야 버그가 줄어든다
                FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
                if (tempfile.Exists)
                    tempfile.Delete();
                isError = false;
            }
            catch(Exception e1)
            {
                isError = true;
                strError = "->" + e1.Source;
            }
        }

        private void btn_AddToMyWav_Click(object sender, EventArgs e)
        {
            // Wav콤보박스에서 선택된 음원을 MyWav에 등록하는 버튼
            // 같은 음원을 등록하려고 하면 동작을 하지 않아야한다..
            // MyWav콤보박스 내부에 Wav콤보박스에서 선택된 아이템이 들어있는 경우에는 동작하지 않게 하는 루틴
            // 정상적이라면 파서 내부의 refBMSPlayer의 웨이브리스트를 검색해서 같은 음원파일이 등록되어있는지
            // 확인을 해야하지만 콤보박스 아이템을 옮김과 동시에 웨이브리스트에 등록도 같이 하기때문에
            // 이런 코드도 버그없이 동작하게된다..
            if (!cbox_MyWav.Items.Contains(cbox_Wav.SelectedItem))
            {
                // 데이터가 없는 경우 00으로 표시하기때문에 웨이브인덱스는 무조건 01부터 시작하여야 한다
                // 만약 00부터 시작하게된다면 해당 웨이브인덱스는 무시될 것이다.
                // 파싱해올 때 부터 00은 없는 null로 처리되기때문이다.
                //MessageBox.Show("this.parser.refBMSPlayer.objWavList.AddWav(" + parser.refBMSPlayer.objWavList.Count + ", \"SoundEffect/" + cbox_Wav.SelectedItem + "\")\n" + "cbox_MyWav.Items.Add(" + cbox_Wav.SelectedIndex + ")\n");
                this.Player.objWavList.AddWav(Player.objWavList.Count+1, "SoundEffect/" + cbox_Wav.SelectedItem);
                cbox_MyWav.Items.Add(cbox_Wav.SelectedItem);
            }
        }

        private void ToScript(string filepath, bool append)
        {
            // dataStr에 등록된 데이터들과 MyWav로 등록된 음원자산들을 스크립트로 출력한다
            // 캐릭터가 아닌 스트링으로 출력을 해야 이쁘게 출력할 수 가 있는데 문제는 아스키코드값을 활용할 수가 없다. \n같은.
            // char형으로 출력하지 않는다.
            StreamWriter stream = new StreamWriter(filepath, append, System.Text.Encoding.Default);
            stream.WriteLine("=========================================================================");
            stream.WriteLine("This is Written by BtheDJ : BMSEditor. and It can be used by only BTheDJ.");
            stream.WriteLine("=========================================================================");
            stream.WriteLine("");
            // 헤더 필드
            stream.WriteLine("*---------------------- HEADER FIELD");
            stream.WriteLine("");
            stream.WriteLine("#PLAYER " + Player.Player.ToString());
            stream.WriteLine("#GENRE " + Player.GENRE.ToString());
            stream.WriteLine("#TITLE " + Player.TITLE.ToString());
            stream.WriteLine("#BPM " + Player.BPM.ToString());
            stream.WriteLine("#PLAYLEVEL " + Player.PLAYLEVEL.ToString());
            stream.WriteLine("#RANK " + Player.RANK.ToString());
            stream.WriteLine("");
            // 웨이브리스트 정보 출력
            for (int i = 0; i < cbox_MyWav.Items.Count; i++)
                stream.WriteLine("#WAV" + BMSUtil.IntToExtHex(i + 1) + " " + cbox_MyWav.Items[i].ToString());
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("");
            // 데이터라인 출력
            stream.WriteLine("*---------------------- DATA FIELD");
            stream.WriteLine("");
            dataStr.Sort();
            for (int i = 0; i < dataStr.Count; i++)
            {
                stream.WriteLine("#" + dataStr[i]);
            }
            stream.WriteLine("");
            stream.WriteLine("=========================================================================");
            stream.WriteLine("                  Thanks for using BTheDJ : Editor.");
            stream.WriteLine("=========================================================================");
            stream.Close();
        }
        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                // Confirm 버튼
                // 스크립트로 출력해서 BMSFiles폴더에 저장하는 기능.
                // 임시 스크립트를 삭제한다
                isConfirmed = true;
                // 스크립트로 BMSFiles폴더에 출력한다
                string filepath = Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + tbox_FileName.Text + ".bms";
                ToScript(filepath, false);
                // 그리고 저장된 파일을 오픈한다.
                //FileInfo file = new FileInfo(Directory.GetCurrentDirectory() + "/Content/BMSFiles/" + tbox_FileName.Text + ".bms");
                //file.Open(FileMode.Open, FileAccess.Read);
                isError = false;
            }
            catch (Exception e1)
            {
                isError = true;
                strError = e1.Source;
            }
        }
        /* 인덱싱처리 문제가 심각하므로 기능을 제외하였다
        private void btn_SubMyWav_Click(object sender, EventArgs e)
        {
            // MyWav콤보박스에서 아이템을 삭제하는 버튼
            //MessageBox.Show("cbox_MyWav.Items.RemoveAt(" + cbox_MyWav.SelectedIndex + ")\n" + "this.parser.refBMSPlayer.objWavList.wavList.Remove(" + cbox_MyWav.SelectedIndex + ")");
            if (cbox_MyWav.SelectedItem != null)
            {
                // 콤보박스에서 아이템을 삭제하는 순간 선택된 인덱스는 해제되기때문에 플레이어의 웨이브리스트 인덱싱이 어긋난다.
                // 무조건 플레이어 먼저 지워주어야 제대로 삭제가 가능하다
                this.parser.refBMSPlayer.objWavList.wavList.Remove(cbox_MyWav.SelectedIndex);
                cbox_MyWav.Items.RemoveAt(cbox_MyWav.SelectedIndex);
                cbox_MyWav.Text = "";
            }
        }
         */
        private void btn_PlayMyWav_Click(object sender, EventArgs e)
        {
            // MyWav콤보박스에서 선택된 음원을 재생시키는 버튼
            //MessageBox.Show("this.parser.refBMSPlayer.objWavList.PlayWav(" + cbox_MyWav.SelectedIndex + ")");
            if (cbox_MyWav.SelectedItem != null)
                this.Player.objWavList.PlayWav(cbox_MyWav.SelectedIndex+1);
        }
        
        private void btn_AllDelete_Click(object sender, EventArgs e)
        {
            state = EditState.Stop;
            int clientx = gd.Viewport.Width;
            int boxwidth = (clientx - 50) / 11;
            //this.parser.refBMSPlayer.objWavList.wavList.Clear();
            this.Player.Playing = false;
            this.Player.BlockList.Clear();
            this.Player.objBarMgr.BarList.Clear();
            this.blockNames.Clear();
            lb_PlayTime.Text = "PlayTime   " + "0:0";
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < bars * Convert.ToInt32(btn_Bardiv.Text)-1; y++)
                {
                    field[x, y] = false;
                }
            }
            // 판정선 위치 리셋
            switch (barDivState)
            {
                case BarDivState.div_4:
                    judgeY = -0;
                    break;
                case BarDivState.div_8:
                    judgeY = -60;
                    break;
                case BarDivState.div_16:
                    judgeY = -150;
                    break;
                case BarDivState.div_32:
                    judgeY = -350;
                    break;
            }
            // 임시 데이터파일(temp.data)도 업데이트해준다
            //string temppath = Directory.GetCurrentDirectory() + "/temp.data";
            //ToScript(temppath, false);
            FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
            if (tempfile.Exists)
                tempfile.Delete();
            dataStr.Clear();
        }
         
        // ===============================================================================
        // 텍스트박스 키프레스 이벤트 처리 함수
        // ===============================================================================
        /*
        private void tbox_Bar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && e.KeyChar != Convert.ToChar(System.Windows.Forms.Keys.Back))
            {
                // 포트번호 입력 시 숫자와 백스페이스키만 유효하도록 한다
                e.Handled = true;
            }


        }
         */
        // ======================================================================================
        // 윈폼 패널상의 그리기이벤트 함수
        // ==================================================================================
        /* 네임스페이스 충돌문제로 SYSTEM.DRAWING 네임스페이스를 활용하기 곤란하다..
        private void frm_Paint(object sender, PaintEventArgs e)
        {
            
        }
         */
        // ================================================================================
        // 키보드 입력값을 리턴해주는 함수.
        public string GetKeyboardChars()
        {
            string str = null;
            Microsoft.Xna.Framework.Input.Keys[] pressedkeys = Keyboard.GetState().GetPressedKeys();
            // 입력받은 키가 딜레이매니저에 포함되어있는 값일 경우만 작동하게 하자
            for (int i = 0; i < pressedkeys.Length; i++)
            {
                char keynumber = (char)pressedkeys[i];
                if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.Back)
                    return null;
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.OemComma)
                    str += ",";
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.OemPeriod)
                    str += ".";
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.Delete)
                    return null;
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.Right)
                    return null;
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.Left)
                    return null;
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.F11)
                    return null;
                else if (pressedkeys[i] == Microsoft.Xna.Framework.Input.Keys.Enter)
                    return null;
                else
                    str += keynumber;
            }
            return str;
        }
        /// <summary>
        /// 이렇게 하면 게임 구성 요소가 스스로 자신을 업데이트할 수 있습니다.
        /// </summary>
        /// <param name="gameTime">타이밍 값의 스냅샷을 제공합니다.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: 여기에 업데이트 코드를 추가하십시오.
            try
            {
                // Game1 객체의 포커스가 에디터로 넘어오면 마우스커서를 표시한다
                if (game.state == Game1.Focus.Scene_Edit)
                    game.IsMouseVisible = true;
                // ==================================================================================
                // 키보드입력 처리 부분.
                // ==================================================================================
                // 기능키 처리
                if (!delayFunc.isDelay())
                {
                    // F11키로 패널을 조정.
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F11))
                        pnl.Visible = !pnl.Visible;
                    // ESC키 입력을 받을 시 다시 곡 선택화면으로 넘어간다.
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                    {
                        // 에디트 컴포넌트를 빠져나가면 초기화시킨다
                        // game의 포커스를 곡선택화면으로 돌리고 패널과 마우스커서를 숨긴다.
                        game.state = Game1.Focus.Scene_Select;
                        pnl.Visible = false;
                        game.IsMouseVisible = false;
                        isConfirmed = false;
                        // 에디트한 파일이 새로 추가되었을 수가 있으므로 최상위 클래스가 가지고 있는
                        // BMSFile 리스트를 비우고 새로 추가한다.
                        game.mSelect.UpdateSongLists();
                        FileInfo tempfile = new FileInfo(Directory.GetCurrentDirectory() + "/temp.data");
                        if (tempfile.Exists)
                            tempfile.Delete();
                        this.Dispose();
                    }
                    // 엔터키를 입력받을 시 컨트롤들에 입력받은 정보를 refBMSPlayer에 넘긴다
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
                    {
                        // 텍스트 박스들중 빈 칸이 하나라도 있으면 입력되지 않게 해야한다.
                        for (int i = 0; i < this.pnl.Controls.Count; i++)
                        {
                            // 해당 컨트롤이 텍스트박스이고 포커스가 잡혀있고 패널이 보이며 
                            // state가 Edit이고 텍스트박스가 비어있지않은경우
                            if (this.pnl.Controls[i].GetType().ToString() == "System.Windows.Forms.TextBox"
                                && this.pnl.Controls[i].Focused && this.pnl.Visible
                                && game.state == Game1.Focus.Scene_Edit && pnl.Controls[i].Text != "")
                            {
                                // 텍스트박스 이름을 기준으로 switch문을 통해 처리(포커스가잡힌 컨트롤별로 처리하기위함)
                                // 예외처리를 하자
                                try
                                {
                                    switch (pnl.Controls[i].Name)
                                    {
                                        case "tbPlayer":
                                            Player.Player = Convert.ToInt32(tbox_Player.Text);
                                            break;
                                        case "tbGenre":
                                            Player.GENRE = tbox_Genre.Text;
                                            break;
                                        case "tbTitle":
                                            Player.TITLE = tbox_Title.Text;
                                            break;
                                        case "tbArtist":
                                            Player.ARTIST = tbox_Artist.Text;
                                            break;
                                        case "tbBPM":
                                            Player.BPM = Convert.ToInt32(tbox_BPM.Text);
                                            break;
                                        case "tbPlayLevel":
                                            Player.PLAYLEVEL = Convert.ToInt32(tbox_PlayLevel.Text);
                                            break;
                                        case "tbRank":
                                            Player.RANK = Convert.ToInt32(tbox_Rank.Text);
                                            break;
                                        case "tbBar":
                                            // 이 부분에서 필드박스들을 생성하는 메소드를 사용하자(미구현)
                                            // (bars와 bardivstate를 활용해서 박스 각각의 좌표를 주어준다)
                                            // 마디를 지나치게 많이 그리게되면 성능이 저하된다...만개 정도?
                                            bars = Convert.ToInt32(tbox_Bar.Text);
                                            // 바뀐정보를 토대로 필드를 초기화
                                            // 초기화를 함과 동시에 기존 필드의 정보를 새 필드로 옮겨야한다.
                                            field = new bool[11, bars * Convert.ToInt32(btn_Bardiv.Text)];
                                            break;
                                    }
                                    // 정상처리를 하면 에러상태를 해제
                                    isError = false;
                                }
                                catch (Exception e)
                                {
                                    // 예외처리를 통해 창이 강제종료되는 문제를 없앤다.
                                    Console.WriteLine(e.Message);
                                    isError = true;
                                    // 한글폰트 처리가 덜되서 메시지는 입력되지않는다..
                                    // 현재는 어떤 종류의 exception이 발생했는지만을 표시해준다
                                    strError = " -> " + e.ToString();
                                }
                            }
                        }
                    }
                    // 일반 키를 입력받았을 때 처리
                    // 해당 루틴이 Func딜레이매니저루틴 내부에 위치하는 이유는
                    // 백스페이스나 스페이스, 엔터키 등을 func키로 분류해서 딜레이매니저를 분할해놓았기때문
                    if (!delayKeyboard.isDelay())
                    {
                        // 패널에 존재하는 모든 컨트롤들을 검색해서 텍스트박스이고 
                        // 포커스가 잡혀있으며 게임의 상태가 에디트, 패널이 보일 경우 값을 입력.
                        for (int i = 0; i < this.pnl.Controls.Count; i++)
                        {
                            if (this.pnl.Controls[i].GetType().ToString() == "System.Windows.Forms.TextBox"
                                && this.pnl.Controls[i].Focused && this.pnl.Visible
                                && game.state == Game1.Focus.Scene_Edit)
                                this.pnl.Controls[i].Text += GetKeyboardChars();
                        }
                    }
                }
                // ===================================================================================
                // 자체 업데이트 부분
                // ======================================================================================
                // 플레이타임 갱신
                double BarPerSecond = Player.BPM / 4 / 60;
                int playTime = (int)(Player.objBarMgr.Count / BarPerSecond);
                int m = playTime / 60;
                int s = playTime - m * 60;
                string strPlayTime = m.ToString() + ":" + s.ToString();
                lb_PlayTime.Text = "PlayTime   " + strPlayTime;
                // 내부 플레이어를 업데이트한다.
                if (state == EditState.Playing)
                    this.Player.Update(gameTime);
                else
                    this.Player.Playing = false;
                // 판정선 좌표 업데이트
                int clientx = gd.Viewport.Width;
                int clienty = gd.Viewport.Height;
                int boxheight = 30;
                int boxwidth = (clientx - 50) / 11;
                if (state == EditState.Playing && judgeY <= boxheight * bars * Convert.ToInt32(btn_Bardiv.Text) + 50)
                {
                    // 판정선은 상태가 playing 이면서 현재 좌표가 마디의 끝에 다다르지 않았을 경우에 업데이트.
                    // 초당 마디 수
                    BarPerSecond = Player.BPM / (4 * 60);
                    // 초당 진행되어야 하는 마디수 * 1회 업데이트 당 걸리는 시간
                    double dyt = (boxheight * Convert.ToInt32(btn_Bardiv.Text) * BarPerSecond) * gameTime.ElapsedGameTime.TotalSeconds;
                    judgeY += dyt;
                }
                if (judgeY >= boxheight * bars * Convert.ToInt32(btn_Bardiv.Text) + 50)
                {
                    state = EditState.Stop;
                    switch (barDivState)
                    {
                        case BarDivState.div_4:
                            judgeY = 0;
                            break;
                        case BarDivState.div_8:
                            judgeY = -60;
                            break;
                        case BarDivState.div_16:
                            judgeY = -150;
                            break;
                        case BarDivState.div_32:
                            judgeY = -350;
                            break;
                    }
                }
                isError = false;
            }
            catch (Exception e)
            {
                isError = true;
                strError = e.Source;
            }
            //this.oldPlayer.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //this.parser.refBMSPlayer.Draw(gameTime);
            try
            {
                spriteBatch.Begin();
                // 필드박스들을 출력
                // 클래스화 시키도록 하자...현재는 막코딩되어있다
                int clientx = gd.Viewport.Width;
                int clienty = gd.Viewport.Height;
                int boxheight = 30;
                int boxwidth = (clientx - 50) / 11;
                /*
                // 필드 작업 영역에 바탕색을 출력
                for (int i = 0; i < 12; i++)
                {
                    if (i >= 0 && i < 9)
                    {
                        // 사용자 채널 영역
                        if (i % 2 == 0) // 짝수
                            spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("FFFFFF"), new Vector2(50 + i * boxwidth, 50 + mouseWheel), new Rectangle(0, 0, 30, 30), Color.Gray, 0, Vector2.Zero, new Vector2((float)boxwidth / 30, (float)Convert.ToInt32(btn_Bardiv.Text) * bars), SpriteEffects.None, 1);
                        else if (i % 2 == 1) // 홀수
                            spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("FFFFFF"), new Vector2(50 + i * boxwidth, 50 + mouseWheel), new Rectangle(0, 0, 30, 30), Color.LightBlue, 0, Vector2.Zero, new Vector2((float)boxwidth / 30, (float)Convert.ToInt32(btn_Bardiv.Text) * bars), SpriteEffects.None, 1);
                    }
                    else
                        // 배경음 채널 영역
                        spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("FFFFFF"), new Vector2(50 + i * boxwidth, 50 + mouseWheel), new Rectangle(0, 0, 30, 30), Color.GreenYellow, 0, Vector2.Zero, new Vector2((float)boxwidth / 30, (float)Convert.ToInt32(btn_Bardiv.Text) * bars), SpriteEffects.None, 1);
                }
                 */
                // 필드 작업 영역을 출력
                for (int y = 0; y < bars * Convert.ToInt32(btn_Bardiv.Text); y++)
                {
                    for (int x = 0; x < 11; x++)
                    {
                        spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("SelectedTile"), new Rectangle(x * boxwidth + 50, y * boxheight + 50 + mouseWheel, boxwidth, boxheight), new Rectangle(2, 2, 16, 17), Color.White);
                        // 마우스를 클릭하여 필드박스를 true로 바꿀경우 해당 위치에 박스를 표시한다.
                        // 배경음 채널이 아니면서(x<=9) 짝수(x%2==1)인 경우는 파란색, 홀수인 경우는 흰색 출력.
                        if (field[x, y] == true)
                            if (x <= 8 && x % 2 == 1)
                                // 사용자 채널 짝수
                                spriteBatch.Draw(objImageResource_Edit2.getTexture2DByAssetName("blue"), new Vector2(boxwidth * x + 52, boxheight * y + 50 + mouseWheel), null, Color.White, 0, Vector2.Zero, new Vector2(1.05f, 1), SpriteEffects.None, 0);
                            else if (x <= 8 && x % 2 == 0)
                                // 사용자 채널 홀수
                                spriteBatch.Draw(objImageResource_Edit2.getTexture2DByAssetName("white"), new Vector2(boxwidth * x + 52, boxheight * y + 50 + mouseWheel), null, Color.White, 0, Vector2.Zero, new Vector2(1.03f, 0.9f), SpriteEffects.None, 0);
                            else
                                // 배경음 채널
                                spriteBatch.Draw(objImageResource_Edit2.getTexture2DByAssetName("white"), new Vector2(boxwidth * x + 52, boxheight * y + 50 + mouseWheel), null, Color.Yellow, 0, Vector2.Zero, new Vector2(1.03f, 0.9f), SpriteEffects.None, 0);
                    }
                }
                // 노트들의 이름을 출력
                for (int i = 0; i < blockNames.Count; i++)
                    spriteBatch.DrawString(font, blockNames[i].name, new Vector2(blockNames[i].x, blockNames[i].y + mouseWheel), Color.Black);
                // ================================================================
                // 가이드라인&정보 출력
                // ==================================================================
                // 채널 가이드 출력
                for (int i = 0; i < 11; i++)
                {
                    // spriteBatch.Draw(                     이미지자산,                                          출력위치,              이미지자산에서의 출력 텍셀좌표,색깔, 회전각도,스프라이트 원점,                출력배율,                                 알파블랜딩 효과, 레이어번호);
                    spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("defualt_SYSTEM"), new Vector2(boxwidth * 9 + 50 - 1, 50 + mouseWheel), new Rectangle(25, 10, 3, 1), Color.White, 0, Vector2.Zero, new Vector2(1, bars * boxheight * Convert.ToInt32(btn_Bardiv.Text)), SpriteEffects.None, 0);
                    if (i <= 8)
                        spriteBatch.DrawString(font, (i + 1).ToString(), new Vector2(i * boxwidth + 50 + 25, 10), Color.White);
                    else
                        spriteBatch.DrawString(font, "BGM", new Vector2(i * boxwidth + 50 + 20, 10), Color.White);
                }
                // 바 가이드 출력
                // 바의 갯수만큼 출력을 하는데 좌표는 박스의 갯수와 바 나누기 상태값을 곱한 것의 그것으로 한다
                for (int i = 0; i < bars; i++)
                {
                    spriteBatch.DrawString(font, "#" + i.ToString(), new Vector2(10, i * boxheight * Convert.ToInt32(btn_Bardiv.Text) + 45 + mouseWheel - 2), Color.White);
                    // spriteBatch.Draw(이미지자산, 출력위치, 이미지자산에서의 출력 텍셀좌표, 색깔, 회전각도, 스프라이트 원점, 출력배율, 알파블랜딩 효과, 레이어번호);
                    spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("defualt_SYSTEM"), new Vector2(50, i * boxheight * Convert.ToInt32(btn_Bardiv.Text) + 50 + mouseWheel - 2), new Rectangle(25, 10, 1, 3), Color.White, 0, Vector2.Zero, new Vector2(boxwidth * 11, 1), SpriteEffects.None, 0);
                }
                // 판정선 출력
                spriteBatch.Draw(objImageResource_Edit.getTexture2DByAssetName("defualt_SYSTEM"), new Vector2(50, (float)judgeY + mouseWheel), new Rectangle(25, 10, 1, 3), Color.Red, 0, Vector2.Zero, new Vector2(boxwidth * 11, 1), SpriteEffects.None, 0);
                //spriteBatch.DrawString(font, this.pnl.Controls[0].GetType().ToString(), new Vector2(10, 10), Color.White);
                // 에러가 발생했을 시 에러리포팅을 함.
                if (isError)
                    spriteBatch.DrawString(font, "ERROR OCCURED!!" + strError, new Vector2(10, game.graphics.PreferredBackBufferHeight - 20), Color.Red);
                if (isMessage)
                    spriteBatch.DrawString(font, "Message!!" + strError, new Vector2(10, game.graphics.PreferredBackBufferHeight - 40), Color.Blue);
                // 패널을 연 상태에서만 디버그정보를 출력
                if (pnl.Visible)
                {
                    // 디버깅 정보 출력.
                    //spriteBatch.DrawString(font, "Welcome to BMSEditor", new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(font, "Parser.refBMSPlayer.BlockList = " + Player.BlockList.Count, new Vector2(10, 30), Color.White);
                    spriteBatch.DrawString(font, "Parser.refBMSPlayer.objWavList = " + Player.objWavList.Count, new Vector2(10, 50), Color.White);
                    spriteBatch.DrawString(font, "Parser.refBMSPlayer.objBarMgr = " + Player.objBarMgr.Count, new Vector2(10, 70), Color.White);
                    int keys = 0;
                    for (int i = 0; i < Player.objBarMgr.Count; i++)
                    {
                        keys += Player.objBarMgr.BarList[i].getKeyList().Count;
                    }
                    spriteBatch.DrawString(font, "Parser.refBMSPlayer.objBarMgr.Keys = " + keys, new Vector2(10, 90), Color.White);
                    spriteBatch.DrawString(font, "field = " + field.Length, new Vector2(10, 110), Color.White);
                    spriteBatch.DrawString(font, "Bars = " + bars, new Vector2(10, 130), Color.White);
                    spriteBatch.DrawString(font, "Player = " + Player.Player, new Vector2(10, 150), Color.White);
                    spriteBatch.DrawString(font, "Genre = " + Player.GENRE, new Vector2(10, 170), Color.White);
                    spriteBatch.DrawString(font, "Title = " + Player.TITLE, new Vector2(10, 190), Color.White);
                    spriteBatch.DrawString(font, "Artist = " + Player.ARTIST, new Vector2(10, 210), Color.White);
                    spriteBatch.DrawString(font, "BPM = " + Player.BPM, new Vector2(10, 230), Color.White);
                    spriteBatch.DrawString(font, "PlayLevel = " + Player.PLAYLEVEL, new Vector2(10, 250), Color.White);
                    spriteBatch.DrawString(font, "Rank = " + Player.RANK, new Vector2(10, 270), Color.White);
                    spriteBatch.DrawString(font, "EditState = " + state.ToString(), new Vector2(10, 290), Color.White);
                    int mouseX = Mouse.GetState().X;
                    int mouseY = Mouse.GetState().Y;
                    spriteBatch.DrawString(font, "MouseCurser = (" + mouseX + "," + mouseY + ")", new Vector2(10, 310), Color.White);
                    spriteBatch.DrawString(font, "FbMouseCurser = (" + fbmouseX + "," + fbmouseY + ")", new Vector2(200, 310), Color.White);
                    spriteBatch.DrawString(font, "MouseBtnState = (" + "Left : " + Mouse.GetState().LeftButton.ToString() + ", Right : " + Mouse.GetState().RightButton.ToString() + ")", new Vector2(10, 330), Color.White);
                    //spriteBatch.DrawString(font, "MouseWheelState = " + Mouse.GetState().ScrollWheelValue, new Vector2(10, 350), Color.White);
                    // xna에서 제공하는 마우스휠 이벤트핸들러는 동작하지않으므로 윈폼 이벤트핸들러를 사용하자.
                    spriteBatch.DrawString(font, "MouseWheelState = " + mouseWheel, new Vector2(10, 350), Color.White);
                    spriteBatch.DrawString(font, "FieldBoxWidth = " + boxwidth, new Vector2(10, 370), Color.White);
                    spriteBatch.DrawString(font, "FieldBoxHeigth = " + boxheight, new Vector2(10, 390), Color.White);
                    spriteBatch.DrawString(font, "MousePositionTrace = " + "[" + fbmouseX / boxwidth + "," + fbmouseY / boxheight + "]" + " bar = " + fbmouseY / boxheight / Convert.ToInt32(btn_Bardiv.Text) + "+" + (fbmouseY / boxheight % Convert.ToInt32(btn_Bardiv.Text) + 1) + "/" + Convert.ToInt32(btn_Bardiv.Text), new Vector2(10, 410), Color.White);
                    spriteBatch.DrawString(font, "cbox_MyWav.SelectedIIndex = " + cbox_MyWav.SelectedIndex, new Vector2(10, 430), Color.White);
                    //if (dataStr.Count > 0)
                    //    spriteBatch.DrawString(font, "Data = #" + dataStr[dataStr.Count-1], new Vector2(10, 450), Color.White);
                    //spriteBatch.DrawString(font, "dataStr.Count = " + dataStr.Count, new Vector2(10, 470), Color.White);
                }
                isError = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isError = true;
                strError = e.Source;
            }
            finally
            {
                spriteBatch.End();
            }
            //this.Player.Draw(gameTime);
            //this.oldPlayer.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            // 초기화부분을 만들자...노가다인 듯 하다
            // 모든 컨트롤들과 맴버변수 상태 등등 모든 것을 초기화해야한다.
            base.Dispose(disposing);
        }
    }
}
