using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander.Persistence
{
    [DataContract(Name = "ControlsPersistence")]
    class ControlsPersistence
    {
        public ControlsPersistence() { }

        public ControlsPersistence(Keys thrust, Keys rotateLeft, Keys rotateRight) 
        {
            this.ThrustKey = thrust;
            this.RotateLeftKey = rotateLeft;
            this.RotateRightKey = rotateRight;
        }

        [DataMember()]
        public Keys ThrustKey { get; set; }
        [DataMember()]
        public Keys RotateLeftKey { get; set; }
        [DataMember()]
        public Keys RotateRightKey { get; set; }
    }
}
