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

    //�z��̐錾
    int[,] map;
    GameObject[,] field;

    GameObject obj;

    //Stack<Vector2Int[]> undoStack = new Stack<Vector2Int[]>();
    //Stack<Vector2Int[]> redoStack = new Stack<Vector2Int[]>();

    //1���i�[����Ă���C���f�b�N�X���擾���鏈��
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
        //Vector2Int�^��(-1,-1)�̒l���쐬����
        return new Vector2Int(-1, -1);
    }

    //�ړ��̉s�𔻒f���Ĉړ������鏈��
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        // �ړ��悪�͈͊O or �ړ���ɕǂ�����ꍇ ��������ړ��s��
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0) || field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") { return false; }
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1) || field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall") { return false; }

        // �ړ���ɔ�������ꍇ
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;

            // �����ړ�����
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; } // �ړ����������Ȃ��ꍇ�A�ړ��s��
        }

        //MoveScript
        Vector3 moveToPosition = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);
        field[moveFrom.y, moveFrom.x].GetComponent<MoveScript>().MoveTo(moveToPosition);

        Vector3 moveFromPosition = new Vector3(moveFrom.x, map.GetLength(0) - moveFrom.y, 0);
        Particle(moveFromPosition, 20);

        // �v���C���[�̈ʒu�X�V
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;

        // �v���C���[�܂��͔��̍��W�X�V
        field[moveTo.y, moveTo.x].transform.position = new Vector3(moveTo.x, map.GetLength(0) - moveTo.y, 0);

        return true;
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(index.x - map.GetLength(1) / 2 + 0.5f,
                            index.y - field.GetLength(0) / 2,
                            0);
    }

    bool IsCleared()//�N���A����
    {
        List<Vector3> ParticleIndex = new List<Vector3>();

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

                    ParticleIndex.Add(new Vector3(x,y + 1,0));
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
        for (int i = 0; i < goals.Count; i++)
        {
            Particle(ParticleIndex[i], 1);
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()//������
    {
        //�X�N���[���ݒ�
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
                //�ύX�B������Ɍ������Ă���
                debugText += map[y, x].ToString() + ",";
            }
            debugText += "\n";//���s
        }
        //����������������o��
        Debug.Log(debugText);
    }



    //Update is called once per frame
    void Update()//���C�����[�v
    {
        //�E���ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(playerIndex, playerIndex + new Vector2Int(1, 0));
        }

        //�����ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(playerIndex, playerIndex + new Vector2Int(-1, 0));
        }

        //�㑤�ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
        }

        //�����ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            //�ړ�����
            Vector2Int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
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
