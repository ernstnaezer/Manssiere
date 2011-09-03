namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class ColorBuffer : ColorBufferObject<uint> { }

    public class ColorBufferObject<T> : BufferObject<T> where T : struct
    {
        static int ColorPointerTypeSize(ColorPointerType colorPointerType)
        {
            switch (colorPointerType)
            {
                case ColorPointerType.Byte: return sizeof(sbyte);
                case ColorPointerType.UnsignedByte: return sizeof(byte);
                case ColorPointerType.Short: return sizeof(short);
                case ColorPointerType.UnsignedShort: return sizeof(ushort);
                case ColorPointerType.Int: return sizeof(int);
                case ColorPointerType.UnsignedInt: return sizeof(uint);
                case ColorPointerType.Float: return sizeof(float);
                case ColorPointerType.Double: return sizeof(double);
                case ColorPointerType.HalfFloat: return Half.SizeInBytes;
                default: Debug.Assert(false); return 0;
            }
        }

        static ColorPointerType DefaultColorPointerType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 3: return ColorPointerType.UnsignedByte;
                    case 4: return ColorPointerType.UnsignedByte;
                    case 6: return ColorPointerType.UnsignedShort;
                    case 8: return ColorPointerType.UnsignedShort;
                    case 12: return ColorPointerType.Float;
                    case 16: return ColorPointerType.Float;
                    case 24: return ColorPointerType.Double;
                    case 32: return ColorPointerType.Double;
                    default: Debug.Assert(false); return ColorPointerType.Float;
                }
            }
        }

        public ColorBufferObject() : this(0) { }
        public ColorBufferObject(int initialCapacity) : this (DefaultColorPointerType, initialCapacity) { }
        public ColorBufferObject(ColorPointerType colorPointerType) : this(colorPointerType, 0) { }
        public ColorBufferObject(ColorPointerType colorPointerType, int initialCapacity) : base(initialCapacity)
        {
            this.colorPointerType = colorPointerType;
            componentCount = StrideInBytes / ColorPointerTypeSize(colorPointerType);
        }

        readonly ColorPointerType colorPointerType = ColorPointerType.Float;
        readonly int componentCount;

        public bool IsSecondary
        {
            get { return isSecondary; }
            set { isSecondary = value; }
        }
        bool isSecondary;

        public void Enable()
        {
            if (Count == 0)
            {
                if (!isSecondary) GL.DisableClientState(EnableCap.ColorArray);
                else GL.DisableClientState(EnableCap.SecondaryColorArray);
                return;
            }

            Update();
            Bind();

            if (!isSecondary)
            {
                GL.EnableClientState(EnableCap.ColorArray);
                GL.ColorPointer(componentCount, colorPointerType, StrideInBytes, IntPtr.Zero);
            }
            else
            {
                GL.EnableClientState(EnableCap.SecondaryColorArray);
                GL.SecondaryColorPointer(componentCount, colorPointerType, StrideInBytes, IntPtr.Zero);
            }
        }
    }
}


