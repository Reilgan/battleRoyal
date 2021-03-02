using battleRoyalServer.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static float interpolationStep = 0.5f;
    private Vector3 oldPosition;
    private Quaternion oldQuaterion;

    public string CharactedName { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = gameObject.transform.position;
        oldQuaterion = gameObject.transform.rotation;
        PhotonClient.Instanse.onReceiveMoveEventArgs += onReceiveMoveEventArgs;
    }

    private void onReceiveMoveEventArgs(object sender, MoveEventArgs e)
    {
        if (e.CharactedName == CharactedName)
        {
            float posX = e.PositionX;
            float posY = e.PositionY;
            float posZ = e.PositionZ;
            float rotX = e.RotationX;
            float rotY = e.RotationY;
            float rotZ = e.RotationZ;
            float rotW = e.RotationW;

            oldPosition = gameObject.transform.position;
            oldQuaterion = gameObject.transform.rotation;

            Vector3 position = new Vector3(posX, posY, posZ);
            Quaternion quaternion = new Quaternion();
            quaternion.Set(rotX, rotY, rotZ, rotW);

            gameObject.transform.rotation = quaternion;
            gameObject.transform.position = position;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Сообщать о перемещении можно только локальному пользователю
        if(CharactedName == PhotonClient.Instanse.CharactedName)
        {
            SendMovingToServer();
        }
    }

    void SendMovingToServer() 
    {
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;

        if (oldPosition != position ^ oldQuaterion != rotation)
        {
            Dictionary<byte, object> moveDict = new Dictionary<byte, object>();
            moveDict.Add((byte)ParameterCode.CharactedName, CharactedName);
            moveDict.Add((byte)ParameterCode.positionX, position.x);
            moveDict.Add((byte)ParameterCode.positionY, position.y);
            moveDict.Add((byte)ParameterCode.positionZ, position.z);
            moveDict.Add((byte)ParameterCode.rotationX, rotation.x);
            moveDict.Add((byte)ParameterCode.rotationY, rotation.y);
            moveDict.Add((byte)ParameterCode.rotationZ, rotation.z);
            moveDict.Add((byte)ParameterCode.rotationW, rotation.w);
            PhotonClient.Instanse.SendMovingToServer(moveDict);
            oldPosition = position;
            oldQuaterion = rotation;
        }
    }

}
