﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TalentShowWebApi.DataTransferObjects
{
    [DataContract]
    public class ContestantDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public PerformanceDto Performance { get; set; }
    }
}