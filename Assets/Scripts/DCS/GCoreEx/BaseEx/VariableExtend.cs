using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Galaxy
{
    public class VariableHelper
    {
        public static string GetVariableBaseType(Type type)
        {
            //only editor
            if (type.IsSubclassOf(typeof(Variable)))
            {
                Variable variable = Activator.CreateInstance(type) as Variable;
                return variable.Type.FullName;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public class IntVariable : Variable<int>
    {
        public IntVariable() : base(0)
        {
        }
        public IntVariable(int value) : base(value)
        {
        }
    }

    public class UIntVariable : Variable<uint>
    {
        public UIntVariable() : base(0)
        {
        }
        public UIntVariable(uint value) : base(value)
        {
        }
    }

    public class FloatVariable : Variable<float>
    {
        public FloatVariable() : base(0.0f)
        {
        }
        public FloatVariable(float value) : base(value)
        {
        }
    }

    public class DoubleVariable : Variable<double>
    {
        public DoubleVariable() : base(0)
        {
        }
        public DoubleVariable(double value) : base(value)
        {
        }
    }

    public class StringVariable : Variable<string>
    {
        public StringVariable() : base("")
        {
        }
        public StringVariable(string value) : base(value)
        {
        }
    }

    public class Vector3Variable : Variable<Vector3>
    {
        public Vector3Variable() : base(Vector3.zero)
        {
        }
        public Vector3Variable(Vector3 value) : base(value)
        {
        }
    }

    public class QuaternionVariable : Variable<Quaternion>
    {
        public QuaternionVariable() : base(Quaternion.identity)
        {
        }
        public QuaternionVariable(Quaternion value) : base(value)
        {
        }
    }
}
