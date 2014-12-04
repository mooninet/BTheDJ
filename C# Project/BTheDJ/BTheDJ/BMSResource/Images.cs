using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BTheDJ.BMSResource
{
    public class Images
    {
        // 로드한 이미지들을 갖고있는 클래스. 로드단계에서 모두 실행되어야만 한다.
        // -> 파서클래스 안에 넣어선 안되고 Game1클래스 안에서 처리해야할듯..
        // 해당 폴더루트를 알아낸 다음 그 안의 파일들의 파일명으로 처리를 해야할 듯..
        // 생성됨과 동시에 파이프라인에 들어있는 이미지파일들을 모두 자신의 자산으로 로드를 한다. 자동으로!
        // 이미지폴더 내의 폴더명을 매개변수로 주어서 생성하면 된다.
        Dictionary<string, Texture2D> ImageList;
        public Images(ContentManager contentManager, string folderName) { Load(contentManager, folderName); }

        void Load(ContentManager contentManager, string folderName)
        {
            string rootDir = "";
            string filename = "";
            ImageList = new Dictionary<string, Texture2D>() { };
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            foreach (DirectoryInfo d in dir.GetDirectories())
                if (d.Name == "Content")
                    foreach (DirectoryInfo d1 in d.GetDirectories())
                        if (d1.Name == "Image")
                            foreach (DirectoryInfo d2 in d1.GetDirectories())
                                if (d2.Name == folderName)
                                {
                                    rootDir = "";
                                    rootDir += d1.Name + "/" + folderName + "/";
                                    char[] sep = new char[1] { '.' };
                                    string[] FileNames;
                                    foreach (FileInfo f in d2.GetFiles())
                                    {
                                        FileNames = f.Name.Split(sep);
                                        filename = rootDir + FileNames[0];
                                        Texture2D image = contentManager.Load<Texture2D>(filename);
                                        ImageList.Add(FileNames[0], image);
                                        filename = "";
                                    }
                                }
        }
        public Texture2D getTexture2DByAssetName(string filename)
        {
            return ImageList[filename];
        }
    }
}
