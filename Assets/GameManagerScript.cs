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

    //�z��̐錾
    int[,] map;
    GameObject[,] field;
    GameObject obj;


    //�o�͏���
    void PrintArray()
    {


    }

    //1���i�[����Ă���C���f�b�N�X���擾���鏈��
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
        //Vector2Int�^��(-1,-1)�̒l���쐬����
        return new Vector2Int(-1, -1);
    }

    //�ړ��̉s�𔻒f���Ĉړ������鏈��
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        //�ړ��悪�͈͊O��������ړ��s��
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        //�ړ����2(��)��������
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;

            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        //�ǂȂ�i�܂Ȃ�
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
        //Vector2Int�^�̉ϒ��z��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //�i�[�ꏊ���ۂ��𔻒f
                if (map[y, x] == 3)
                {
                    //�i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        //�v�f����goals.Count�Ŏ擾
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                //��ł���������������������B��
                return false;
            }
        }
        //�������B���łȂ���Ώ����B��
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
                //�ύX�B������Ɍ������Ă���
                debugText += map[y, x].ToString() + ",";
            }
            debugText += "\n";//���s
        }
        //����������������o��
        Debug.Log(debugText);
    }



    //Update is called once per frame
    void Update()
    {
        //�E���ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            //PrintArray();
        }

        //�����ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
            //PrintArray();
        }

        //�㑤�ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
            //PrintArray();
        }

        //�����ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
            //PrintArray();
        }

        //���Z�b�g
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
