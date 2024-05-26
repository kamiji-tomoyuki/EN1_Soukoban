using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

//Object Instantiate(Object original, Vector3 position, Quaternion rotation);

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;

    public GameObject clearText;

    //配列の宣言
    int[,] map;
    GameObject[,] field;
    GameObject obj;


    //出力処理
    void PrintArray()
    {


    }

    //1が格納されているインデックスを取得する処理
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.Length; y++)
        {
            for (int x = 0; x < field.Length; x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        //Vector2Int型の(-1,-1)の値を作成する
        return new Vector2Int(-1, -1);
    }

    //移動の可不可を判断して移動させる処理
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        //移動先が範囲外だったら移動不可
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        //移動先に2(箱)が居た時
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;

            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        //壁なら進まない
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
        }

        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(index.x - map.GetLength(1) / 2 + 0.5f,
                            index.y - field.GetLength(0) / 2,
                            0);
    }

    bool IsCleared()
    {
        //Vector2Int型の可変長配列の作成
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //格納場所か否かを判断
                if (map[y, x] == 3)
                {
                    //格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        //要素数はgoals.Countで取得
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                //一つでも箱が無かったら条件未達成
                return false;
            }
        }
        //条件未達成でなければ条件達成
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        map = new int[,] {
            {3,0,0,0,0 },
            {0,0,2,0,0 },
            {0,0,1,0,0 },
            {0,2,0,0,0 },
            {0,0,0,3,0 },

        };

        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];



        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                        playerPrefab,
                        IndexToPosition(new Vector2Int(x, y)),
                        Quaternion.identity
                        );
                }
            }
        }

        //PrintArray();
        string debugText = "";
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //変更。文字列に結合していく
                debugText += map[y, x].ToString() + ",";
            }
            debugText += "\n";//改行
        }
        //結合した文字列を出力
        Debug.Log(debugText);
    }



    //Update is called once per frame
    void Update()
    {
        //右側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            //PrintArray();
        }

        //左側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
            //PrintArray();
        }

        //上側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
            //PrintArray();
        }

        //下側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
            //PrintArray();
        }

        //リセット
        if (Input.GetKeyDown(KeyCode.R) && !IsCleared())
        {
            SceneManager.LoadScene(0);
        }

        if (IsCleared())
        {
            Debug.Log("Clear");
            clearText.SetActive(true);
        }
    }
}
