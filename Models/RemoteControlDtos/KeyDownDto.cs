﻿using DevSim.Enums;
using System.Runtime.Serialization;

namespace DevSim.Models.RemoteControlDtos
{
    [DataContract]
    public class KeyDownDto : BaseDto
    {
        [DataMember(Name = "Key")]
        public string Key { get; set; }

        [DataMember(Name = "DtoType")]
        public override BaseDtoType DtoType { get; init; } = BaseDtoType.KeyDown;
    }
}