using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace BTheDJ.BMSData
{
    /* 재생할 웨이브파일을 리스트에 저장하는 클래스 */
    public class WavList : Wav
    {
        ContentManager Content;
        //public List<Wav> wavList;
        public Dictionary<int, Wav> wavList;
        //public Wav[] wavList;
        public WavList(ContentManager Content)
            :base(null, Content)
        {
            this.Content = Content;
            //wavList = new List<Wav>() { };
            wavList = new Dictionary<int, Wav> { };
        }
        //public WavList(int NoteNumber) 
        //{ 
        //    wavList = new List<Wav>(NoteNumber) { }; 
            //wavList = new Wav[NoteNumber];
        //}
        public int Count
        {
            get { return wavList.Count; }
            set { Count = value; }
        }

        public void AddWav(int index, string filepath)
        {
            // 재생은 block클래스에서 담당하도록 만든다
            Wav file = new Wav(filepath, Content);
            //wavList[wavNum] = file;
            wavList.Add(index, file);
        }

        public void PlayWav(int wavNum)
        {
            //MediaPlayer.Play(wavList[wavNum].Wave);
            //wavList[wavNum].Wave.Play(1.0f, 0.0f, 0.0f);
            // 인스턴스를 생성해서 음원을 재생하게되면 dispose할 시 자동으로 중지된다.
            SoundEffectInstance instWav = wavList[wavNum].Wave.CreateInstance();
            instWav.Play();
            //instWav.Dispose();
            //wavList[wavNum].Wave.Play();
        }
        /*
        public void StopWavs()
        {
            for(int i=0; i<wavList.Count; i++)
            {
                wavList[i].Wave.Stop(true);
            }
        }
         */
    }
}
