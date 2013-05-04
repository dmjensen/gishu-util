// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;

namespace ManiaX.Beacons.DataStructs
{
    public class StateDistributionSnapshot : IEquatable<StateDistributionSnapshot>
    {
        public DateTime Timestamp { get; set; }

        public float Compiling { get; set; }
        public float CompileErrors { get; set; }
        public float NoCompileErrors { get; set; }
        public float Red { get; set; }
        public float Green { get; set; }

        public override string ToString()
        {
            return string.Format("Timestamp: {0}, C: {1}, CE: {2}, NCE: {3}, Red: {4}, Green: {5}", 
                Timestamp, Compiling, CompileErrors, NoCompileErrors, Red, Green);
        }

        public bool Equals(StateDistributionSnapshot other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Timestamp.Equals(Timestamp) && other.Compiling.Equals(Compiling) && other.CompileErrors.Equals(CompileErrors) && other.NoCompileErrors.Equals(NoCompileErrors) && other.Red.Equals(Red) && other.Green.Equals(Green);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StateDistributionSnapshot)) return false;
            return Equals((StateDistributionSnapshot) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Timestamp.GetHashCode();
                result = (result*397) ^ Compiling.GetHashCode();
                result = (result*397) ^ CompileErrors.GetHashCode();
                result = (result*397) ^ NoCompileErrors.GetHashCode();
                result = (result*397) ^ Red.GetHashCode();
                result = (result*397) ^ Green.GetHashCode();
                return result;
            }
        }
    }
}