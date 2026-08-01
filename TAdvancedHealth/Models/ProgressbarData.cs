

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Tavstal.TAdvancedHealth.Models
{
    public class ProgressbarData
    {
        public ProgressbarValue Food { get; set; } = new ProgressbarValue();
        public ProgressbarValue Water { get; set; } = new ProgressbarValue();
        public ProgressbarValue Virus { get; set; } = new ProgressbarValue();
        public ProgressbarValue Stamina { get; set; } = new ProgressbarValue();
        public ProgressbarValue Oxygen { get; set; } = new ProgressbarValue();
        public  ProgressbarValue Health { get; set; } = new ProgressbarValue();
        public ProgressbarValue Head { get; set; } = new ProgressbarValue();
        public ProgressbarValue Body { get; set; } = new ProgressbarValue();
        public ProgressbarValue LeftArm { get; set; } = new ProgressbarValue();
        public ProgressbarValue RightArm { get; set; } = new ProgressbarValue();
        public ProgressbarValue LeftLeg { get; set; } = new ProgressbarValue();
        public ProgressbarValue RightLeg { get; set; } = new ProgressbarValue();
    }
}
