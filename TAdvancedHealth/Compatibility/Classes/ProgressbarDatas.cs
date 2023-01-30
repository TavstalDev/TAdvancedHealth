using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class ProgressBarDatas
    {
        public float LastFood = 0;
        public float LastWater = 0;
        public float LastStamina = 0;
        public float LastVirus = 0;
        public float LastOxygen = 0;
        public float LastSimpleHealth = 0;
        public float LastHealthHead = 0;
        public float LastHealthBody = 0;
        public float LastHealthLeftArm = 0;
        public float LastHealthLeftLeg = 0;
        public float LastHealthRightArm = 0;
        public float LastHealthRightLeg = 0;
        public List<string> VisibleSimpleHealth = new List<string>();
        public List<string> VisibleFood = new List<string>();
        public List<string> VisibleWater = new List<string>();
        public List<string> VisibleVirus = new List<string>();
        public List<string> VisibleStamina = new List<string>();
        public List<string> VisibleOxygen = new List<string>();
        public List<string> VisibleHead = new List<string>();
        public List<string> VisibleBody = new List<string>();
        public List<string> VisibleLeftArm = new List<string>();
        public List<string> VisibleLeftLeg = new List<string>();
        public List<string> VisibleRightArm = new List<string>();
        public List<string> VisibleRightLeg = new List<string>();
    }
}
