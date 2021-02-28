using battleRoyalServer.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleRoyalServer.Operations
{
    class Move:BaseOperations
    {
        Dictionary<byte, object> parameters;
        public Move(IRpcProtocol protocol, OperationRequest request) : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.CharactedName)]
        public string CharactedName { get; set; }

        [DataMember(Code = (byte)ParameterCode.positionX)]
        public string PositionX { get; set; }

        [DataMember(Code = (byte)ParameterCode.positionY)]
        public string PositionY { get; set; }

        [DataMember(Code = (byte)ParameterCode.positionZ)]
        public string PositionZ { get; set; }

        [DataMember(Code = (byte)ParameterCode.rotationX)]
        public string RotationX { get; set; }

        [DataMember(Code = (byte)ParameterCode.rotationY)]
        public string RotationY { get; set; }

        [DataMember(Code = (byte)ParameterCode.rotationZ)]
        public string RotationZ { get; set; }

        public Dictionary<byte, object> GetParametersForEvent()
        {
            parameters = new Dictionary<byte, object>();
            parameters.Add((byte)ParameterCode.CharactedName, CharactedName);
            parameters.Add((byte)ParameterCode.positionX, PositionX);
            parameters.Add((byte)ParameterCode.positionY, PositionY);
            parameters.Add((byte)ParameterCode.positionZ, PositionZ);
            parameters.Add((byte)ParameterCode.rotationX, RotationX);
            parameters.Add((byte)ParameterCode.rotationY, RotationY);
            parameters.Add((byte)ParameterCode.rotationZ, RotationZ);
            return parameters;
        }

    }
}
