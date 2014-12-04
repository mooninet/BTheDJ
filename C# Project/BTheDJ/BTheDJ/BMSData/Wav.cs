using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace BTheDJ.BMSData
{
    public class Wav
    {
        public string Filepath;
        //ContentManager Content;
        protected SoundEffect wav;
        public SoundEffect Wave
        {
            get { return wav; }
            // 읽기전용 프로퍼티
            set { wav = value; }
        }
        //public Wav() { }
        // ContentManager 대신 Sounds클래스를 넘겨받아 그 안의 딕셔너리에서 데이터를 갖고오자.
        public Wav(string filepath, ContentManager contentManager)
        {
            //this.Content = Content;
            Filepath = filepath;
            //setWav(filepath);
            // 웨이브리스트를 생성할 때 예외처리를 위해 넣은 루틴.
            if (filepath == null)
                return;
            wav = contentManager.Load<SoundEffect>(filepath);
            //instWav = wav.CreateInstance();
            //contentManager.Dispose(); // game1클래스의 contentmanager를 dispose()해버리게 된다..
        }
        
        string getFilepath()
        {
            return Filepath;
        }
        /*
        void setWav(string filepath)
        {
            //Uri streaming = new Uri(filepath);
            //wav = Song.FromUri(" ", streaming);
            if (filepath == null) return;
            SoundEffect streaming = Content.Load<SoundEffect>(filepath);
            wav = streaming;
        }
        */ 
    }
}
