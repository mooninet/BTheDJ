using System;

namespace BTheDJ
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// 응용 프로그램의 진입점입니다.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

