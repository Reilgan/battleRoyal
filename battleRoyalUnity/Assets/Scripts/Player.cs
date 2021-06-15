using gameServer.Common;
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

    public int Id { get; set; }

    public string Name { get; set; }

    public Dictionary<byte, object> PlayerInfo { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = gameObject.transform.position;
        oldQuaterion = gameObject.transform.rotation;
        GameClient.Instanse.onReceiveMoveEventArgs += onReceiveMoveEventArgs;
    }

    private void onReceiveMoveEventArgs(object sender, MoveEventArgs e)
    {
        if (e.Id == Id)
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
        if (Id == SinglePlayerStruct.Instanse.Id)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 position = new Vector3(gameObject.transform.position.x + 1,
                                               gameObject.transform.position.y,
                                               gameObject.transform.position.z);
                gameObject.transform.position = position;
            }
        }
    }

    void FixedUpdate()
    {
        // Сообщать о перемещении можно только локальному пользователю
        if(Id == SinglePlayerStruct.Instanse.Id)
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
            moveDict.Add((byte)ParameterCode.Id, Id);
            moveDict.Add((byte)ParameterCode.positionX, position.x);
            moveDict.Add((byte)ParameterCode.positionY, position.y);
            moveDict.Add((byte)ParameterCode.positionZ, position.z);
            moveDict.Add((byte)ParameterCode.rotationX, rotation.x);
            moveDict.Add((byte)ParameterCode.rotationY, rotation.y);
            moveDict.Add((byte)ParameterCode.rotationZ, rotation.z);
            moveDict.Add((byte)ParameterCode.rotationW, rotation.w);
            GameClient.Instanse.SendMovingToServer(moveDict);
            oldPosition = position;
            oldQuaterion = rotation;
        }
    }

}
