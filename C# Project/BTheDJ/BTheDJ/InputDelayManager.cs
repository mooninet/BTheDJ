using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BTheDJ
{
    public class InputDelayManager
        // 키보드 입력에 딜레이를 부여하는 클래스. Update함수 내부에서만 사용가능하다.
        // 타겟과 같은 키보드값이 입력되면 inputDelay를 계속 증가시켜서 일정치에 다다르면 입력을 받지 않는다.
    {
        byte inputDelay = 0; // 현재 딜레이 수치
        byte rank = 1; // 언제 딜레이를 넣을 건지 정하는 변수
        Keys[] targetKeys; // 딜레이를 부여할 키 입력값 배열
        public Keys[] keyList
        {
            get { return targetKeys; }
            set { }
        }

        public InputDelayManager(Keys[] targetKeys)
        {
            // TODO: 모든 파생 구성 요소를 이곳에서 생성하십시오.
            this.targetKeys = targetKeys;
        }
        public InputDelayManager(Keys[] targetKeys, byte rank)
        {
            this.targetKeys = targetKeys;
            this.rank = rank;
        }
       
        public bool isDelay()
        {
            //키보드 딜레이 검사
            if (targetKeys != null) // 타겟이 있다면
            {
                //딜레이를 써야할 상황인지를 저장하는 변수
                bool delayOn = false; // 키가 눌려져 있지 않으면 자동으로 false가 들어간다.
                //딜레이를 써야 할 키가 하나라도 눌려 있으면 딜레이 on
                foreach (Keys targetKey in targetKeys)
                    if (Keyboard.GetState().IsKeyDown(targetKey))
                        delayOn = true;
                //딜레이가 설정되어 있으면 딜레이를 걸어 연속 입력 방지
                if (delayOn)
                {
                    inputDelay++;
                    // 딜레이 수치를 200까지만 증가시킨다
                    inputDelay = inputDelay > 200 ? (byte)200 : inputDelay;

                    // 딜레이 수치가 rank 이하 일 경우는 딜레이 부여를 하지 않는다
                    if (inputDelay <= rank)
                        return false;
                        // rank 초과 일 경우에는 딜레이를 부여한다.
                    else
                        return true;
                }
                //딜레이가 설정되어 있지 않으면 딜레이 수치 초기화
                    // 
                else
                {
                    inputDelay = 0;
                    return false;
                }
            }
            else // 타겟이 없으면 딜레이 부여를 하지 않는다(무조건 false리턴)
                return false;
        }
    }
}
