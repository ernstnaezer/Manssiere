namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class VertexBuffer : VertexBufferObject<Vector3> { }

    public class VertexBufferObject<T> : BufferObject<T> where T : struct
    {
        static int VertexPointerTypeSize(VertexPointerType vertexPointerType)
        {
            switch (vertexPointerType)
            {
                case VertexPointerType.Short: return sizeof(short);
                case VertexPointerType.Int: return sizeof(int);
                case VertexPointerType.HalfFloat: return Half.SizeInBytes;
                case VertexPointerType.Float: return sizeof(float);
                case VertexPointerType.Double: return sizeof(double);
                default: Debug.Assert(false); return 0;
            }
        }

        static VertexPointerType DefaultVertexPointerType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 4: return VertexPointerType.Short; // 2 * Short
                    case 6: return VertexPointerType.Short; // 3 * Short
                    case 8: return VertexPointerType.Float; // 2 * Float
                    case 12: return VertexPointerType.Float; // 3 * Float
                    case 16: return VertexPointerType.Double; // 2 * Double
                    case 24: return VertexPointerType.Double; // 3 * Double
                    case 48: return VertexPointerType.Double; // 4 * Double
                    default: Debug.Assert(false); return VertexPointerType.Float;
                }
            }
        }

        public VertexBufferObject() : this(0) { }
        public VertexBufferObject(int initialCapacity) : this (DefaultVertexPointerType, initialCapacity) { }
        public VertexBufferObject(VertexPointerType vertexPointerType) : this(vertexPointerType, 0) { }
        public VertexBufferObject(VertexPointerType vertexPointerType, int initialCapacity) : base(initialCapacity)
        {
            _vertexPointerType = vertexPointerType;
            _componentCount = StrideInBytes / VertexPointerTypeSize(vertexPointerType);
        }

        private readonly VertexPointerType _vertexPointerType;
        private readonly int _componentCount;

        public void Enable()
        {
            if (Count == 0)
            {
                GL.DisableClientState(EnableCap.VertexArray);
                return;
            }

            Update();
            Bind();

            GL.EnableClientState(EnableCap.VertexArray);
            GL.VertexPointer(_componentCount, _vertexPointerType, StrideInBytes, IntPtr.Zero);
        }
    }
}


