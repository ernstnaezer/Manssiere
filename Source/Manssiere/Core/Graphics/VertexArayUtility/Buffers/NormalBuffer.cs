namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class NormalBuffer : NormalBufferObject<Vector3> { }

    public class NormalBufferObject<T> : BufferObject<T> where T : struct
    {
        static NormalPointerType DefaultNormalPointerType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 3: return NormalPointerType.Short; // 3 * Byte
                    case 6: return NormalPointerType.Short; // 3 * Short
                    case 12: return NormalPointerType.Float; // 3 * Float
                    case 24: return NormalPointerType.Double; // 3 * Double
                    default: Debug.Assert(false); return NormalPointerType.Float;
                }
            }
        }

        public NormalBufferObject() : this(0) { }
        public NormalBufferObject(int initialCapacity) : this (DefaultNormalPointerType, initialCapacity) { }
        public NormalBufferObject(NormalPointerType normalPointerType) : this(normalPointerType, 0) { }
        public NormalBufferObject(NormalPointerType normalPointerType, int initialCapacity): base(initialCapacity)
        {
            this.normalPointerType = normalPointerType;
        }

        readonly NormalPointerType normalPointerType;

        public void Enable()
        {
            if (Count == 0)
            {
                GL.DisableClientState(EnableCap.NormalArray);
                return;
            }

            Update();
            Bind();

            GL.EnableClientState(EnableCap.NormalArray);
            GL.NormalPointer(normalPointerType, StrideInBytes, IntPtr.Zero);
        }
    }
}


