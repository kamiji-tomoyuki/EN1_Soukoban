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

    public GameObject clearPrefab;
    public GameObject wallPrefab;
    public GameObject particlePrefab;

    public GameObject clearText;

    //配列の宣言
    int[,] map;
    GameObject[,] field;

    GameObject obj;

    //Stack<Vector2Int[]> undoStack = new Stack<Vector2Int[]>();
    //Stack<Vector2Int[]> redoStack = new Stack<Vector2Int[]>();

    //1が格納されているインデックスを取得する処理
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
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
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        // 移動先が範囲外 or 移動先に壁がある場合 だったら移動不可
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0) || field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") { return false; }
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1) || field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") { return false; }

        // 移動先に箱がある場合
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;

            // 箱を移動する
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; } // 移動が成功しない場合、移動不可
        }

        //MoveScript
        Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
        field[moveFrom.y, moveFrom.x].GetComponent<MoveScript>().MoveTo(moveToPosition);

        Vector3 moveFromPosition = new Vector3(moveFrom.x, map.GetLength(0) - moveFrom.y, 0);
        Particle(moveFromPosition, 20);

        // プレイヤーの位置更新
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        // プレイヤーまたは箱の座標更新
        field[moveTo.y, moveTo.x].transform.position = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);

        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(index.x - map.GetLength(1) / 2 + 0.5f,
                            index.y - field.GetLength(0) / 2,
                            0);
    }

    bool IsCleared()//クリア条件
    {
        List<Vector3> ParticleIndex = new List<Vector3>();

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

                    ParticleIndex.Add(new Vector3(x,y + 1,0));
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
        for (int i = 0; i < goals.Count; i++)
        {
            Particle(ParticleIndex[i], 1);
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()//初期化
    {
        //スクリーン設定
        Screen.SetResolution(1280, 720, false);


        map = new int[,] {
            {4,4,4,4,4,4,4},
            {4,3,4,0,0,0,4},
            {4,0,0,2,4,0,4},
            {4,0,2,1,0,0,4},
            {4,4,0,4,4,0,4},
            {4,3,0,0,0,0,4},
            {4,4,4,4,4,4,4},
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
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                        clearPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.01f),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 4)
                {
                    field[y, x] = Instantiate(
                        wallPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0.01f),
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
    void Update()//メインループ
    {
        //右側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(playerIndex, playerIndex + new Vector2Int(1, 0));
        }

        //左側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        //上側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
        }

        //下側に数字を移動させる処理
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            //移動処理
            Vector2Int playerIndex = GetPlayerIndex();
            //移動処理を関数化
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
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

    void Particle(Vector3 position, int count) {
    for(int i = 0; i < count; i++)
        {
            GameObject ParticleInstance = Instantiate(
                particlePrefab,position, Quaternion.identity) as GameObject;
            Particle particle = ParticleInstance.GetComponent<Particle>();
            if(particle != null)
            {
                particle.Start();
            }


        }
    
    }
}
