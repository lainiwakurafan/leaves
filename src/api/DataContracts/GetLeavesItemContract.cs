using System;

namespace Leaves.Api
{
    public class GetLeavesItemContract
    {
        public string Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string GoogleCalendarLink { get; set; }
    }
}
