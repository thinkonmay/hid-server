﻿using DevSim.Enums;
using System.Runtime.Serialization;

namespace DevSim.Models.RemoteControlDtos
{
    [DataContract]
    public class MouseDownDto : BaseDto
    {
        [DataMember(Name = "Button")]
        public ButtonCode Button { get; set; }

        [DataMember(Name = "DtoType")]
        public override BaseDtoType DtoType { get; init; } = BaseDtoType.MouseDown;

        [DataMember(Name = "PercentX")]
        public double PercentX { get; set; }

        [DataMember(Name = "PercentY")]
        public double PercentY { get; set; }
    }
}
