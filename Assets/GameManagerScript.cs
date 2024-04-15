using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //�z��̐錾
    int[] map;

    //�o�͏���
    void PrintArray()
    {
        //�ǉ��A������̐錾�Ə�����
        string debugText = "";

        for (int i = 0; i < map.Length; i++)
        {
            //�ύX�B������Ɍ������Ă���
            debugText += map[i].ToString() + ",";
        }
        //����������������o��
        Debug.Log(debugText);
    }

    //1���i�[����Ă���C���f�b�N�X���擾���鏈��
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

    //�ړ��̉s�𔻒f���Ĉړ������鏈��
    bool MoveNumber(int number, int moveFrom, int moveTo)
    {
        if (moveTo < 0 || moveTo >= map.Length)
        {
            //�����Ȃ��������ɏ���
            return false;
        }
        //2������
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
        //�E���ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //�ړ�����
            int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(1, playerIndex, playerIndex + 1);
            PrintArray();
        }

        //�����ɐ������ړ������鏈��
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //�ړ�����
            int playerIndex = GetPlayerIndex();
            //�ړ��������֐���
            MoveNumber(1, playerIndex, playerIndex - 1);
            PrintArray();
        }
    }
}
