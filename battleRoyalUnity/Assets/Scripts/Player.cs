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
        Debug.Log(CharactedName);
        if (e.CharactedName == CharactedName)
        {
            float posX = e.PositionX;
            float posY = e.PositionY;
            float posZ = e.PositionZ;
            float rotX = e.RotationX;
            float rotY = e.RotationY;
            float rotZ = e.RotationZ;

            oldPosition = gameObject.transform.position;
            oldQuaterion = gameObject.transform.rotation;

            Vector3 position = new Vector3(posX, posY, posZ);
            Quaternion quaternion = new Quaternion();
            quaternion.SetEulerRotation(rotX, rotY, rotZ);

            gameObject.transform.rotation *= Quaternion.Lerp(oldQuaterion, quaternion, interpolationStep);
            gameObject.transform.position = Vector3.Lerp(oldPosition, position, interpolationStep);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        sendMovingToServer();
    }

    void sendMovingToServer() 
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
            moveDict.Add((byte)ParameterCode.rotationX, rotation.eulerAngles.x);
            moveDict.Add((byte)ParameterCode.rotationY, rotation.eulerAngles.y);
            moveDict.Add((byte)ParameterCode.rotationZ, rotation.eulerAngles.z);
            PhotonClient.Instanse.sendMovingToServer(moveDict);
            oldPosition = position;
            oldQuaterion = rotation;
        }
    }

}
