using System;

namespace MagBot_FFXIV_v02
{
    class Character
    {
        private IntPtr _basePointer;
        private IntPtr _namePointer;
        private IntPtr _levelPointer;
        private IntPtr _hpPointer;
        private IntPtr _maxHpPointer;
        private IntPtr _mpPointer;
        private IntPtr _maxMpPointer;
        private IntPtr _tpPointer;
        private IntPtr _maxTpPointer;
        private IntPtr _xCoordinatePointer;
        private IntPtr _yCoordinatePointer;
        private IntPtr _zCoordinatePointer;
        private IntPtr _facingAnglePointer;
        private float _x, _y, _z;
        private Waypoint _waypointLocation = new Waypoint(0, 0, 0);

        protected internal Character(int baseOffset)
        {
            LoadPointers(baseOffset);
        }

        private int _id;
        internal int ID
        {
            get
            {
                MemoryHandler.Instance.ReadInt(_basePointer, out _id);
                return _id;
            }
        }

        private string _name;
        internal string Name
        {
            get
            {
                MemoryHandler.Instance.ReadString(_namePointer, 15, out _name);
                return _name;
            }
        }

        private byte _level;
        internal int Level
        {
            get
            {
                MemoryHandler.Instance.ReadByte(_levelPointer, out _level);
                return Convert.ToInt32(_level);
            }
        }

        private int _hp;
        internal int HP
        {
            get
            {
                MemoryHandler.Instance.ReadInt(_hpPointer, out _hp);
                return _hp;
            }
        }

        private int _maxHP;
        internal int MaxHP
        {
            get
            {
                MemoryHandler.Instance.ReadInt(_maxHpPointer, out _maxHP);
                return _maxHP;
            }
        }

        private int _mp;
        internal int MP
        {
            get
            {
                MemoryHandler.Instance.ReadInt(_mpPointer, out _mp);
                return _mp;
            }
        }

        private int _maxMP;
        internal int MaxMP
        {
            get
            {
                MemoryHandler.Instance.ReadInt(_maxMpPointer, out _maxMP);
                return _maxMP;
            }
        }

        private short _tp;
        internal short TP
        {
            get
            {
                MemoryHandler.Instance.ReadShort(_tpPointer, out _tp);
                return _tp;
            }
        }

        private short _maxTP;
        internal short MaxTP
        {
            get
            {
                MemoryHandler.Instance.ReadShort(_maxTpPointer, out _maxTP);
                return _maxTP;
            }
        }

        public float XCoordinate
        {
            get
            {
                MemoryHandler.Instance.ReadFloat(_xCoordinatePointer, out _x);
                return (float)Math.Round(_x, 3);
            }
        }

        public float YCoordinate
        {
            get
            {
                MemoryHandler.Instance.ReadFloat(_yCoordinatePointer, out _y);
                return (float)Math.Round(_y, 3);
            }
        }

        public float ZCoordinate
        {
            get
            {
                MemoryHandler.Instance.ReadFloat(_zCoordinatePointer, out _z);
                return (float)Math.Round(_z, 3);
            }
        }

        public Waypoint WaypointLocation
        {
            get
            {
                _waypointLocation.X = XCoordinate;
                _waypointLocation.Y = YCoordinate;
                _waypointLocation.Z = ZCoordinate;
                return _waypointLocation;
            }
        }

        private float _facingAngle;
        internal float FacingAngle
        {
            get
            {
                MemoryHandler.Instance.ReadFloat(_facingAnglePointer, out _facingAngle);
                return (float)Math.Round(_facingAngle, 3);
            }
        }

        private void LoadPointers(int baseOffset)
        {
            //When a new character instance is created we create the pointers
            //This ensures that for targets (memorylocation varies), we use the right pointer
            var offsets = new int[2];
            offsets[0] = baseOffset;
            
            _basePointer = MemoryHandler.Instance.GetPointerFromOffsets(new[] {offsets[0]});

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["Name"][0];
            _namePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["Level"][0];
            _levelPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["HP"][0];
            _hpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["MaxHP"][0];
            _maxHpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["MP"][0];
            _mpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["MaxMP"][0];
            _maxMpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["TP"][0];
            _tpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["MaxTP"][0];
            _maxTpPointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["XCoordinate"][0];
            _xCoordinatePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["YCoordinate"][0];
            _yCoordinatePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["ZCoordinate"][0];
            _zCoordinatePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);

            offsets[1] = Globals.Instance.MemoryAdditionalOffsetDictionary["FacingAngle"][0];
            _facingAnglePointer = MemoryHandler.Instance.GetPointerFromOffsets(offsets);
        }
    }
}
