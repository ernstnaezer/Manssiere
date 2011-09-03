namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using OpenTK.Graphics.OpenGL;

    public class VertexAttribBuffer : VertexAttribBufferObject<uint> { }

    public class VertexAttribBufferObject<T> : BufferObject<T> where T : struct
    {
        static VertexAttribPointerType DefaultVertexAttribPointerType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 1: return VertexAttribPointerType.UnsignedByte;
                    case 2: return VertexAttribPointerType.UnsignedShort;
                    case 4: return VertexAttribPointerType.UnsignedInt;
                    case 8: return VertexAttribPointerType.Double;
                    default: throw new ArgumentException("Unsupported size");
                }
            }
        }

        public VertexAttribBufferObject() : this(0) { }
        public VertexAttribBufferObject(int initialCapacity) : this (DefaultVertexAttribPointerType, initialCapacity) { }
        public VertexAttribBufferObject(VertexAttribPointerType vertexAttribPointerType) : this(vertexAttribPointerType, 0) { }
        public VertexAttribBufferObject(VertexAttribPointerType vertexAttribPointerType, int initialCapacity) : base(initialCapacity)
        {
            _vertexAttribPointerType = vertexAttribPointerType;
        }

        private readonly VertexAttribPointerType _vertexAttribPointerType;
        private int _attributeLocation = -1;

        public int AttributeLocation
        {
            get { return _attributeLocation; }
            set { _attributeLocation = value; }
        }

        public void Enable()
        {
            if (_attributeLocation == -1) return;

            Update();

            if (Count == 0)
            {
                GL.DisableVertexAttribArray(_attributeLocation);
                return;
            }

            Bind();

            GL.EnableVertexAttribArray(_attributeLocation);
            GL.VertexAttribPointer(_attributeLocation, 1, _vertexAttribPointerType, false, StrideInBytes, IntPtr.Zero);
        }
    }
}


