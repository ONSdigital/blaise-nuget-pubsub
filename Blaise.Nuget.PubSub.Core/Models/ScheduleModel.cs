using Blaise.Nuget.PubSub.Contracts.Enums;
using System;
namespace Blaise.Nuget.PubSub.Core.Models
{
    public class ScheduleModel
    {
        public int NumberOfMessages { get; set; }
        
        public int IntervalNumber { get; set; }

        public IntervalType IntervalType { get; set; }
    }
}
