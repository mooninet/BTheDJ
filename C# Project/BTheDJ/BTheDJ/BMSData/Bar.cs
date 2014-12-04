using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTheDJ.BMSData
{
    public class Bar
    {
        public enum _BLOCK_TYPE
        {
            BPM,
            BITMAP,
            WAV,
            KEY
        };

        // key타입 리스트 KeyList 생성.
        List<Key> KeyList = new List<Key> { };
        BarMgr refBarMgr;
        float BarHeight; // 바의 높이 -> 이걸 이용해서 각 바 마다 마디선을 긋도록 하자
        float y;
        public int BarNumber; // 바의 번호(인덱스)
        public Bar(BarMgr barMgr)
        {
            //if (BarNumber != 0)
            //    BarHeight = barMgr.GetBMSPlayer.BarHeight * BarNumber; // 마디선을 그리기위해 마디좌표를 가지고온다. 그리고 곱한다.
            //else
                BarHeight = barMgr.GetBMSPlayer.BarHeight;
            refBarMgr = barMgr;
        }
        public float BarY
        {
            get { return y; }
            set { y = value; }
        }
        public List<Key> getKeyList()
        {
            return KeyList;
        }
        //public Bar() { }
        public void AddKey(int Channelfirst, int Channelsecond, string Data)
        {
            Key key = new Key(Channelfirst, Channelsecond, Data);
            KeyList.Add(key);

            // BarNum을 이용해서 y를 구하여 블럭위치를 설정.
            // 데이터를 분석한다
            switch (Channelfirst)
            {
                case 0:
                    // default 채널
                    DefaultChannel(Channelsecond, Data);
                    break;
                case 1:
                    // 건반 채널
                    ChannelKeyboard(Channelsecond, Data);
                    break;
            }
        }

        public void DefaultChannel(int channelNum, string data) // 이벤트채널을 말한다.(채널번호중 앞번호가 0인 것)
        {
            // 이벤트 채널 넘버에 따라 동작(ChannelSecond)
            switch (channelNum)
            {
                    /*
                case 0:
                    // 동작하지않는다 ex) #xxx00:xxyyzz.....
                    break;
                     */
                case 1: // #xxx01
                    // 배경음채널
                    ChannelWave(data);
                    break;
                case 2:
                    // 마디 단축
                    break;
                case 3:
                    // bpm채널
                    ChannelBpm(data);
                    break;
                case 4:
                    // bga채널
                    ChannelBitmap(data);
                    break;
                case 5:
                    // bm98 확장채널
                    break;
                case 6:
                    // poor bga채널
                    break;
                default:
                    break;
            }
        }
        public void ChannelKeyboard(int channelNum, string data)
        {
            /* 사용자 채널을 분석해서 keynum값으로 넘겨주는 루틴이 필요 -> 필요없다. 데이터에 다 포함되어있다
               파싱한 데이터의 channel데이터를 1,2,3,4,5,6 등
               몇번째 행에 노트를 출력해야하는지 처리를 해야함 */
            BlockProcess(channelNum, data, _BLOCK_TYPE.KEY);
        }
         
        public void ChannelWave(string data)
        {
            BlockProcess(0, data, _BLOCK_TYPE.WAV);
        }

        public void ChannelBpm(string data)
        {
            BlockProcess(3, data, _BLOCK_TYPE.BPM);
        }

        public void ChannelBitmap(string data)
        {
            BlockProcess(4, data, _BLOCK_TYPE.BITMAP);
        }

        public void BlockProcess(int channelNum, string data, _BLOCK_TYPE type)
        {
            // 블럭의 데이터를 분석
            int n = data.Length / 2;

            for (int i = 0; i < n; i++)
            {
                int Num = BMSUtil.ExtHexToInt(data.Substring(i * 2, 2)); // WAV번호(데이터)

                // 0이라면 그냥 넘기자 넘기면 안될텐데...
                if (Num == 0) continue;

                // (바의 높이/n * i)는 마디 내부에서의 위치
                // 바의 높이는 노트간 간격을 조절한다...
                double y = -((BarHeight / (double)n * i) + (BarNumber * BarHeight));
                switch (type)
                {
                    case _BLOCK_TYPE.BPM:
                    // BPM블럭인경우 바매니저를 통해 현재 bmsplayer의 bpm블럭 추가 메소드를 사용한다
                    
                        break;
                    case _BLOCK_TYPE.BITMAP:
                    // bitmap 블럭
                             
                        break;
                    case _BLOCK_TYPE.WAV:
                    // wave 블럭
                        refBarMgr.GetBMSPlayer.AddWaveBlock(Num, (float)y);
                      break;
                    case _BLOCK_TYPE.KEY:
                    // keyboard 블럭
                      refBarMgr.GetBMSPlayer.AddKeyboardBlock(channelNum, Num, (float)y); // 채널번호, wavNumber, y좌표
                      break;
                }
            }
        }
        public void MoveDown(float dy)
        {
            if (y <= 450)
                y += dy;
        }
    }
}
