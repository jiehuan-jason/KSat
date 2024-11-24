using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSat
{
    class PassDataList
    {
        public SimpleSatData sat;
        public List<SatPassingData> pass_list;
        public GeodeticCoordinate location;
        public PassDataList(SimpleSatData sat, GeodeticCoordinate location)
        {
            this.sat = sat;
            this.location = location;
            //getPassDataList(sat.NoradNum);
        }
        public async Task getPassDataList(int noradNumber)
        {
            var TleList = await TLEData.getLocalTLEs();
            var sattle = TleList.FirstOrDefault(tle => tle.NoradNumber == noradNumber);

            // Alternatively get a specific satellite's TLE
            var sat = new Satellite(sattle.Line1,sattle.Line2);

            // 定义地面站位置
            var groundStation = new GroundStation(location);

            var observations = groundStation.Observe(sat, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromHours(72), TimeSpan.FromSeconds(10));
            var topo = groundStation.Observe(sat, DateTime.UtcNow + TimeSpan.FromHours(24));      // 打印过境信息 
            List<SatPassingData> sat_passing = new List<SatPassingData>();
            foreach (var observation in observations)
            {

                Debug.WriteLine($"最高仰角: {observation.MaxElevation} 度");
                Debug.WriteLine($"入境时间: {observation.Start.ToLocalTime()}");
                Debug.WriteLine($"出境时间: {observation.End.ToLocalTime()}");
                var start_topo = groundStation.Observe(sat, observation.Start);
                var end_topo = groundStation.Observe(sat, observation.End);
                var max_topo = groundStation.Observe(sat, observation.MaxElevationTime);
                Debug.WriteLine($"入境方位角: {start_topo.Azimuth} 度");
                Debug.WriteLine($"出境方位角: {end_topo.Azimuth} 度");
                Debug.WriteLine($"最高仰角方位角: {max_topo.Azimuth} 度");
                sat_passing.Add(new SatPassingData(start_topo.Azimuth, end_topo.Azimuth, observation.MaxElevation, max_topo.Azimuth, observation.Start.ToLocalTime(), observation.End.ToLocalTime()));
            }
            pass_list = sat_passing;
            //var observations = groundStation.Observe(sat, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromHours(24), TimeSpan.FromSeconds(10));
            //Debug.WriteLine(issTle);
        }
    }
}
