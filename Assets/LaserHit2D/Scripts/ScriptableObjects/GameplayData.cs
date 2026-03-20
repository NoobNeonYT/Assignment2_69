using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LaserHit2D
{

    [CreateAssetMenu(fileName = "GameplayData", menuName = "CustomObjects/GameplayData", order = 1)]
    public class GameplayData : ScriptableObject
    {
        public int LevelNumber;



        public bool m_GameEnded = false;

        public int m_GameEndResult = -1;

    }


}