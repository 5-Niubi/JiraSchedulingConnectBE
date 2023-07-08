﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Algorithm.ScheduleResult
{
    public class TaskScheduleResultDTO
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int? duration { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public MileStoneScheduleResultDTO? mileStone { get; set; }
        public List<int>? taskIdPrecedences { get; set; }
        public WorkforceScheduleResultDTO? workforce { get; set; }
        public List<int>? equipmentId { get; set; } // Chua dung

    }
}
