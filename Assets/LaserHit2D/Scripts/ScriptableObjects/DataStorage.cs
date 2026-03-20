using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace LaserHit2D
{
    [CreateAssetMenu(fileName = "DataStorage", menuName = "CustomObjects/DataStorage", order = 1)]
    public class DataStorage : ScriptableObject
    {
        public int LevelUnlocked;




        public void SaveData()
        {
            PlayerPrefs.SetInt("LevelUnlocked", LevelUnlocked);

            PlayerPrefs.Save();
        }

        public void LoadData()
        {
            LevelUnlocked = PlayerPrefs.GetInt("LevelUnlocked", 0);

        }

        public void ResetSaveData()
        {
            SaveData();
        }





        public bool CheckInternet()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                return true;



            return false;
        }

    }
}
