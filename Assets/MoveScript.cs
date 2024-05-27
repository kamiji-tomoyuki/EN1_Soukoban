using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    //�����܂łɂ����鎞��
    private float timeTaken = 0.2f;
    //�o�ߎ���
    private float timeElapsed;

    //�ړI�n
    private Vector3 destination;
    //�o���n
    private Vector3 origin;

    private void Start()
    {
        //�ړI�n�E�o���n�����ݒn�ŏ�����
        destination = transform.position;
        origin = destination;
    }

    public void MoveTo(Vector3 newDestination)
    {
        //�o�ߎ��Ԃ�������
        timeElapsed = 0;
        //�ړ����̉\��������̂ŁA���݈ʒu��Position�ɑO��ړ��̖ړI�n����
        origin = destination;
        transform.position = origin;
        //�V�����ړI�n����
        destination = newDestination;
    }

    private void Update()
    {
        //�ړI�n�ɓ������Ă����珈�����Ȃ�
        if(origin == destination) { return; }
        //���Ԍo�߂����Z
        timeElapsed += Time.deltaTime;
        //�o�ߎ��Ԃ��������Ԃ̉������Z�o
        float timeRate = timeElapsed / timeTaken;
        //�������Ԃ𒴂������ȂƂ��������ԑ����Ɋۂ߂�
        if(timeRate > 1) { timeRate = 1; }
        //�C�[�W���O
        float easing = timeRate;
        //���W���Z�o
        Vector3 currentPosition = Vector3.Lerp(origin, destination, easing);
        //�Z�o�������W��Position�ɑ��
        transform.position = currentPosition;
    }
}
