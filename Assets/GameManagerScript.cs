using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //配列の宣言
    int[] map;

    //出力処理
    void PrintArray()
    {
        //追加、文字列の宣言と初期化
        string debugText = "";

        for (int i = 0; i < map.Length; i++)
        {
            //変更。文字列に結合していく
            debugText += map[i].ToString() + ",";
        }
        //結合した文字列を出力
        Debug.Log(debugText);
    }

    //1が格納されているインデックスを取得する処理
    int GetPlayerIndex()
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }

    //移動の可不可を判断して移動させる処理
    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        if (moveTo < 0 || moveTo >= map.Length)
        {
            //動けない条件を先に書く
            return false;
        }
        //2を押す
        if (map[moveTo] == 2) {
        int velocity = moveTo - moveFrom;

            bool sucess = MoveNumber(2, moveTo, moveTo + velocity);
            if(!sucess) { return false; }
        }
        map[moveTo] = number;
        map[moveFrom] = 0;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        map = new int[] { 0, 0, 0, 1, 2, 0, 0, 0, 0 };
        PrintArray();
    }

    // Update is called once per frame
    void Update()
    {
        //右側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //移動処理
            int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(1, playerIndex, playerIndex + 1);
            PrintArray();
        }

        //左側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //移動処理
            int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(1, playerIndex, playerIndex - 1);
            PrintArray();
        }
    }
}
