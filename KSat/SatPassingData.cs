using SGPdotNET.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSat
{
    class SatPassingData
    {
        public Angle start_angle;
        public Angle end_angle;
        public Angle max_elevation;
        public Angle max_angle;
        public DateTime start_time;
        public DateTime end_time;
        public SatPassingData(Angle start_angle, Angle end_angle, Angle max_elevation, Angle max_angle, DateTime start_time, DateTime end_time)
        {
            this.start_angle = start_angle;
            this.end_angle = end_angle;
            this.max_elevation = max_elevation;
            this.max_angle = max_angle;
            this.start_time = start_time;
            this.end_time = end_time;
        }
    }
}
