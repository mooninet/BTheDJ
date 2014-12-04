using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace BTheDJ.BMSData
{
    public class BMSParser
    {
        public BMSPlayer refBMSPlayer;
        /* BMS파일을 읽어들여서 한줄한줄 처리하는 루틴 */
        // filepath를 매개변수로 활용을 함으로써 하나의 파일만 읽을게 아니라
        // 여러개의 bms파일을 읽을 수 있게 만들 수 있다..
        public BMSParser(Game1 game)
        {
            refBMSPlayer = game.mPlayer;
        }
        // 해당 생성자는 임시 플레이(곡선택화면에서의 곡재생을 위한)를 위한 생성자이다.
        // 플레이어를 매개변수로 받아 파서 내부에 임시플레이어를 하나 생성하기 위한 생성자이다.
        public BMSParser(BMSPlayer player)
        {
            refBMSPlayer = player;
        }
        public void ParseHeader(string filepath)
        {
            // BMS파일의 헤더부분 중 곡 정보만 불러오는 함수(곡 선택부분에서 오버헤드를 줄이기 위함)
            StreamReader stream = new StreamReader(filepath, System.Text.Encoding.Default);
            string linedata;
            do
            {
                linedata = stream.ReadLine();
                Process(linedata);
            } while (!linedata.Contains("#RANK"));
            stream.Close();
        }
        
        public void Parse(string filepath)
        {
            // BMS파일의 내용 전체를 파싱하는 함수.
            /* 현재 위치 폴더 안의 파일들중 확장자가 bms인 파일들의 이름을 
               스트링타입 리스트에 저장하는 루틴
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            List<string> files = new List<string> { };
            foreach (FileInfo f in dir.GetFiles())
                if(f.Extension == ".bms")
                    files.Add(f.FullName);
            */
            StreamReader stream = new StreamReader(filepath, System.Text.Encoding.Default);
            string linedata;
            // do-while 문을 사용하는 이유는 linedata가 선언만 하고 할당되지않은 string변수이기때문에
            // while문을 사용하면 syntex애러가 발생한다.
            do
            {
                linedata = stream.ReadLine();
                Process(linedata);
            } while (linedata != null);
            stream.Close();
        }
        /* 읽어들인 BMS파일 데이터를 처리하는 루틴 */
        private void Process(string linedata)
        {
            // BMS파일의 명령어에 따라 작동
            // 읽은 데이터가 없다면 리턴
            if (linedata == null) return;
            // 데이터가 #으로 시작한다면
            if (linedata.StartsWith("#"))
            {
                // 공백문자나 콜론을 seperator로 설정
                char[] seps = new char[]{' ',':'};
                // StringList에 입력받은 한줄의 데이터를 seperator로 split해서 넣는다
                string[] StringList = linedata.Split(seps, 2);
                //int noteNum = 0;

                if (StringList[0].Equals("#PLAYER")) { refBMSPlayer.Player = int.Parse(StringList[1]); }
                else if (StringList[0].Equals("#GENRE")) { refBMSPlayer.GENRE = StringList[1]; }// 장르를 읽음
                else if (StringList[0].Equals("#TITLE")) { refBMSPlayer.TITLE = StringList[1]; }
                else if (StringList[0].Equals("#ARTIST")) { refBMSPlayer.ARTIST = StringList[1]; }
                else if (StringList[0].Equals("#BPM")) { refBMSPlayer.BPM = float.Parse(StringList[1]); }
                else if (StringList[0].Equals("#PLAYLEVEL")) { refBMSPlayer.PLAYLEVEL = int.Parse(StringList[1]); }
                else if (StringList[0].Equals("#RANK")) { refBMSPlayer.RANK = int.Parse(StringList[1]); }
                else if (StringList[0].Equals("#STAGEFILE")) { }
                else if (StringList[0].Equals("#TOTAL")) { }
                else if (StringList[0].Equals("#MIDIFILE")) { }
                else if (StringList[0].Equals("#VIDEOFILE")) { }
                else if (StringList[0].StartsWith("#WAV"))
                {
                    // WAV파일을 읽음
                    // ex) #WAV01 0도.wav -> WAV다음인 4번째 문자부터 2자리를 parse하는데
                    // 다음 2문자는 16진수 타입 데이터이기 때문에 numberstyles.hexnumber 메소드를 사용한다
                    int wavIndex = BTheDJ.BMSUtil.ExtHexToInt(StringList[0].Substring(4, 2));
                    //noteNum++;
                    // bms플레이어에서 wav파일을 추가하여 로드한다.(파일이름만 전해줘야한다)
                    char[] sep = new char[1] { '.' };
                    string[] filename = StringList[1].Split(sep);
                    refBMSPlayer.AddWav(wavIndex, @"SoundEffect/" + filename[0]);
                }
                else if (StringList[0].Equals("#BMP"))
                {
                    /*
                    // BMP를 읽음
                    int bitmapIndex = BMSUtil.ExtHexToInt(StringList[0].Substring(4, 2));
                    // 비트맵파일을 추가한다
                    refBMSPlayer.AddBmp(bitmapIndex, Directory.GetCurrentDirectory() + "/" + StringList[1]);
                     */
                }
                // 이 외에는 모두 데이터파일로 받아들인다 ex) #xxxyy:00000000000000000000001J00000000
                // xxx : 노트가 들어갈 마디의 번호. 0-999
                // yy : 채널 번호. 이 번호는 뒤에 따라오는 데이터가 어떤 내용을 가리키는지 나타냄
                else
                {
                    try
                    {
                        //refBMSPlayer.NoteNum = noteNum;
                        // 바넘버와 채널넘버를 알아와서 등록한다
                        int BarNum = GetBarNum(StringList[0]);
                        int ChannelNum = GetChannelNum(StringList[0]);

                        // 두자리의 채널넘버중 10의자리
                        int ChannelFirst = GetChannelFirst(StringList[0]);

                        // 두자리의 채널넘버중 1의자리
                        int ChannelSecond = GetChannelSecond(StringList[0]);
                        // BMSPlayer오브젝트의 AddBar메소드로 바를 등록.
                        refBMSPlayer.AddBar(BarNum, ChannelFirst, ChannelSecond, StringList[1]);
                        //int a = refBMSPlayer.objBarMgr.BarList.Count;
                    }
                    // 예외처리.
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        private int GetBarNum(string data)
        {
            return int.Parse(data.Substring(1,3));
        }
        private int GetChannelNum(string data)
        {
            return int.Parse(data.Substring(4, 2));
        }
        private int GetChannelFirst(string data)
        {
            return int.Parse(data.Substring(4, 1));
        }
        private int GetChannelSecond(string data)
        {
            return int.Parse(data.Substring(5, 1));
        }
    }
}
