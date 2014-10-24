using System;
using System.Windows.Forms;

namespace MagBot_FFXIV_v02
{
    public class Element
    {
        private readonly Label _label;
        private readonly IntPtr _pointer;
        private volatile float _value;
        private readonly string _elementType;

        private int _intOutput;
        private float _floatOutput;
        private short _shortOutput;

        public Element(Label label = default(Label), IntPtr pointer = default(IntPtr), float value = default(float), string elementType = default(string))
        {
            _label = label;
            _pointer = pointer;
            _value = value;
            _elementType = elementType;
        }

        //Accessor Properties
        public Label Label
        {
            get { return _label; }
        }

        public float Value
        {
            get
            {
                switch (_elementType)
                {
                    case "int":
                        MemoryHandler.Instance.ReadInt(_pointer, out _intOutput);
                        _value = _intOutput;
                        break;
                    case "float":
                        MemoryHandler.Instance.ReadFloat(_pointer, out _floatOutput);
                        _value = (float)Math.Round(_floatOutput, 0);
                        break;
                    case "ushort":
                        MemoryHandler.Instance.ReadShort(_pointer, out _shortOutput);
                        _value = _shortOutput;
                        break;
                }
                return _value;
            }
        }
    }
}